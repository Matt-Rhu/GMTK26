using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using System.Reflection;
using Codice.Client.BaseCommands;
using Attribute = System.Attribute;
using Object = UnityEngine.Object;

[CustomEditor(typeof(MonoBehaviour), true), CanEditMultipleObjects]
public class RhuInspector : Editor
{
    public override void OnInspectorGUI()
    {
        DrawRhuInspector();
    }
    
    //If I want this to be compatible with custom inspectors, they need to inherit from it, so this is my virtual equivalent to DrawDefaultInspector()
    public virtual void DrawRhuInspector()
    {
        InspectorWithButtonAttribute();
    }

    private void InspectorWithButtonAttribute()
    {
        //Draw buttons set to come before vars
        DrawButtons(GetMethods(target), true);
        
        DrawInspector();
        
        //And those that come after
        DrawButtons(GetMethods(target), false);
    }

    private void DrawInspector()
    {
        serializedObject.Update();
        
        DrawProperties();
        
        serializedObject.ApplyModifiedProperties();
    }
    
    private void DrawProperties()
    {
        SerializedProperty property = serializedObject.GetIterator();
        bool enterChildren = true;
        bool expanded = true;
        
        while (property.NextVisible(enterChildren))
        {
            enterChildren = false;

            // draw script field disabled
            if (property.name == "m_Script")
            {
                using (new EditorGUI.DisabledScope(true))
                    EditorGUILayout.PropertyField(property, true);
                continue;
            }
            
            if (TryGetAttribute<FoldHeaderAttribute>(property, out var attribute)) //draw fold header and get its value
                expanded = DrawFoldHeader(property, attribute.Header);
            else //normal draw
                if (expanded) EditorGUILayout.PropertyField(property, true);
        }
    }

    private bool DrawFoldHeader(SerializedProperty property, string label)
    {
        string key = Key(property);
        
        //Draw foldout and get its value
        bool expanded = EditorPrefs.GetBool(key, true);
        EditorGUILayout.Space();
        GUIStyle bigStyle = new GUIStyle(EditorStyles.foldout)
        {
            fontStyle = FontStyle.Bold,
            fontSize = EditorStyles.standardFont.fontSize + 1
        };
        bool newExpanded = EditorGUILayout.Foldout(expanded, label, true, bigStyle);

        //Store value if different
        if (newExpanded != expanded)
            EditorPrefs.SetBool(key, newExpanded);

        //Show or not based on value
        if (newExpanded)
            EditorGUILayout.PropertyField(property, true);

        return expanded;
    }

    private void DrawButtons(MethodInfo[] methods, bool beforeVars)
    {
        //Spacing for legibility
        if (methods.Length > 0 && !beforeVars) GUILayout.Space(10);
        
        foreach (MethodInfo method in methods)
        {
            ButtonAttribute button = method.GetCustomAttribute<ButtonAttribute>();
            
            //Draw button if method has the attribute, condition is valid, and if we're at the right before/after position
            if (button != null && beforeVars == button.BeforeVars && GetCondition(button, targets))
            {
                //use method name if no name was given
                string buttonName = string.IsNullOrEmpty(button.Name) ? method.Name : button.Name;

                //button
                if (GUILayout.Button(buttonName, GUILayout.MaxHeight(button.Height)))
                    if (!button.AskConfirmation)
                        InvokeForAll(method, targets);
                    else //ask for confirmation if asked to in attribute parameters
                        if (EditorUtility.DisplayDialog("Please Confirm :]", "Are you sure you want to " + button.Name + "?", "Yes", "No"))
                            InvokeForAll(method, targets);
            }
        }
        
        //also spacing, but if before vars
        if (methods.Length > 0 && beforeVars) GUILayout.Space(10);
    }

    private bool TryGetAttribute<T>(SerializedProperty property, out T attribute) where T : Attribute
    {
        attribute = (T)GetField(property)?.GetCustomAttribute(typeof(T));
        return attribute != null;
    }

    private FieldInfo GetField(SerializedProperty property)
    {
        string key = target.GetType().FullName + "." + property.propertyPath;

        if (EditorStorage.RhuInspectorFields.TryGetValue(key, out FieldInfo field))
            return field;
        field = target.GetType().GetField(property.propertyPath, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
        EditorStorage.RhuInspectorFields.Add(key, field);
        return field;
    }

    public static MethodInfo[] GetMethods(Object[] targets)
    {
        List<MethodInfo> methods = new List<MethodInfo>();
        foreach (var t in targets)
            methods.AddRange(GetMethods(t));
        return methods.ToArray();
    }
    
    public static MethodInfo[] GetMethods(Object target)
    {
        string key = target.GetType().FullName;

        if (EditorStorage.RhuInspectorMethods.TryGetValue(key, out var methodList))
            return methodList.ToArray();
        MethodInfo[] methods = target.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
        EditorStorage.RhuInspectorMethods[key] = methods.ToList();
        return methods;
    }

    public static void InvokeForAll(MethodInfo method, object[] targets)
    {
        foreach (var t in targets)
            method.Invoke(t, null);
    }

    private static bool GetCondition(ButtonAttribute button, Object[] targets)
    {
        foreach (var t in targets)
            if (!GetCondition(button, t))
                return false;
        return true;
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private static bool GetCondition(ButtonAttribute button, Object target)
    {
        //return true if there's no condition
        if (button.Condition == null) return true;
        
        //get property, return its value if it's a bool
        var propertyInfo = target.GetType().GetProperty(button.Condition, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        if (propertyInfo != null)
        {
            if (propertyInfo.GetValue(target) is bool)
                return (bool)propertyInfo.GetValue(target);
            Debug.LogWarning("Property '" + button.Condition + "' is not a bool");
            return true;
        }
        
        //if not a property, get field, return its value if it's a bool
        var fieldInfo = target.GetType().GetField(button.Condition, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        if (fieldInfo != null)
        {
            if (fieldInfo.FieldType == typeof(bool))
                return (bool)fieldInfo.GetValue(target);
            Debug.LogWarning("Field '" + button.Condition + "' is not a bool");
            return true;
        }
        
        //if can't find anything, return true
        Debug.LogWarning("Couldn't find button condition '" + button.Condition + "' in " + target.name);
        return true;
    }
    
    private static string Key(SerializedProperty property)
    {
        var target = property.serializedObject.targetObject;
        return $"Foldout_{target.GetType().FullName}_{property.propertyPath}";
    }
}