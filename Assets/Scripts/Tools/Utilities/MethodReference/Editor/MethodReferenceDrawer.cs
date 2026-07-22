using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(MethodReference))]
public class MethodReferenceDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
        
        SerializedProperty nameProp = property.FindPropertyRelative("name");
        
        Rect dropdownRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        
        
        var list = MethodNames(property);

        if (list.Length == 0)
        {
            EditorGUI.HelpBox(dropdownRect, "No methods with Referenceable attribute were found", MessageType.Warning);
            EditorGUI.EndProperty();
            return;
        }
        
        int currentIndex = Mathf.Max(0, Array.IndexOf(list, nameProp.stringValue));
        int newIndex = EditorGUI.Popup(dropdownRect, label.text, currentIndex, list);

        if (newIndex != currentIndex)
            nameProp.stringValue = list[newIndex];
        
        if (nameProp.stringValue == string.Empty)
            nameProp.stringValue = list[0];
        
        
        EditorGUI.EndProperty();
    }

    private string[] MethodNames(SerializedProperty property)
    {
        var methods = RhuInspector.GetMethods(property.serializedObject.targetObject);
        List<string> names = new List<string>();

        foreach (var m in methods)
        {
            var attr = m.GetCustomAttribute<ReferenceableAttribute>();
            if (attr == null) continue;
            
            names.Add(m.Name);
        }

        return names.ToArray();
    }
}
