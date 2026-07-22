using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(SceneReference))]
public class SceneReferenceDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
        
        SerializedProperty nameProp = property.FindPropertyRelative("name");
        
        Rect dropdownRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        
        
        var list = GetScenesList();
        
        int currentIndex = Mathf.Max(0, Array.IndexOf(list, nameProp.stringValue));
        int newIndex = EditorGUI.Popup(dropdownRect, label.text, currentIndex, list);

        if (newIndex != currentIndex)
            nameProp.stringValue = list[newIndex];
        
        
        EditorGUI.EndProperty();
    }

    private string[] GetScenesList()
    {
        var scenes = EditorBuildSettings.scenes;
        List<string> sceneNames = new List<string>();

        foreach (var scene in scenes)
        {
            string path = scene.path;
            string name = System.IO.Path.GetFileNameWithoutExtension(path);
            sceneNames.Add(name);
        }

        return sceneNames.ToArray();
    }
}
