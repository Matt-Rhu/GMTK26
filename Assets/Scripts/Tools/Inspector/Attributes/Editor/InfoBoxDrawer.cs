using UnityEditor;
using UnityEngine;

namespace Modules.CustomAttributeEditor
{
    [CustomPropertyDrawer(typeof(InfoBoxAttribute))]
    public class InfoBoxDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            InfoBoxAttribute help = (InfoBoxAttribute)attribute;

            string message = help.Message;
            float helpBoxHeight = EditorStyles.helpBox.CalcHeight(new GUIContent(message), position.width);

            Rect helpRect = new (position.x, position.y, position.width, helpBoxHeight);
            EditorGUI.HelpBox(helpRect, message, MessageType.Info);

            if (!help.ShowProperty) return;
            position.y += helpBoxHeight + 2;
            EditorGUI.PropertyField(position, property, label, true);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            InfoBoxAttribute help = (InfoBoxAttribute)attribute;
            float helpBoxHeight = EditorStyles.helpBox.CalcHeight(new GUIContent(help.Message), EditorGUIUtility.currentViewWidth);
            float fieldHeight = EditorGUI.GetPropertyHeight(property, label, true);

            return helpBoxHeight + (help.ShowProperty ? (2 + fieldHeight) : 0);
        }
    }
}