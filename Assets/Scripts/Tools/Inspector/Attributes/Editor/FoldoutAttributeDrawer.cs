using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(FoldoutAttribute))]
public class FoldoutAttributeDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        //Draw foldout and store its state in the dictionary
        bool expanded = EditorPrefs.GetBool(Key(property), true);
        GUIStyle boldFoldoutStyle = new GUIStyle(EditorStyles.foldout);
        boldFoldoutStyle.fontStyle = FontStyle.Bold;
        expanded = EditorGUI.Foldout(
            new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight),
            expanded,
            label,
            true, 
            boldFoldoutStyle
        );
        EditorPrefs.SetBool(Key(property), expanded); //save to editor prefs so it persists between restarts

        //If not collapsed, draw the property field normally
        if (EditorPrefs.GetBool(Key(property), true))
        {
            EditorGUI.indentLevel++;
            position.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(position, property, GUIContent.none, true);
            EditorGUI.indentLevel--;
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (!EditorPrefs.GetBool(Key(property), true))
            return EditorGUIUtility.singleLineHeight;

        return EditorGUIUtility.singleLineHeight + EditorGUI.GetPropertyHeight(property, true);
    }

    private static string Key(SerializedProperty property)
    {
        return $"Foldout_{property.serializedObject.targetObject.GetInstanceID()}_{property.propertyPath}";
    }
}