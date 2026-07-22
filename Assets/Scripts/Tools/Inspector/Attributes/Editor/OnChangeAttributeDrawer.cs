using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(OnChangeAttribute))] 
public class OnChangeAttributeDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        //draw property normally and check for changes
        EditorGUI.BeginChangeCheck();
        EditorGUI.PropertyField(position, property, label, true);
        property.serializedObject.ApplyModifiedProperties(); // need to apply changes in order to detect changes (wow)
        if (EditorGUI.EndChangeCheck())
            CallMethod(property); // call function if changes there are
    }
    
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, label, true);
    }
    
    private void CallMethod(SerializedProperty property)
    {
        var targets = property.serializedObject.targetObjects;
        OnChangeAttribute onChange = (OnChangeAttribute)attribute;

        foreach (var target in targets)
        {
            if (TryGetMethod(onChange.MethodName, target, out var method))
                method.Invoke(target, null);
            else
                Debug.LogWarning($"Method '{onChange.MethodName}' not found on {target}");
        }
    }

    private static bool TryGetMethod(string methodName, Object target, out MethodInfo method)
    {
        string key = target.GetType().FullName + "." + methodName;

        if (EditorStorage.OnChangeMethods.TryGetValue(key, out method)) return method != null;
        method = target.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
        EditorStorage.OnChangeMethods.Add(key, method);

        return method != null;
    }
}