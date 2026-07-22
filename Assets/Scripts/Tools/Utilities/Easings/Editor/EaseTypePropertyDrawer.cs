using Codice.Client.BaseCommands;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(Ease.Type))]
public class EaseTypePropertyDrawer : PropertyDrawer
{
    private const float curveHeight = 80;
    private const int steps = 50;
    private readonly Color color = Color.yellow;

    private float previousY;
    private Vector3[] curvePoints;

    private string name;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        name = property.name;
        
        //Enum and curve rects
        float lineHeight = EditorGUIUtility.singleLineHeight;
        Rect enumRect = new Rect(
            position.x,
            position.y,
            position.width,
            lineHeight
        );
        float curveWidth = position.width - EditorGUIUtility.labelWidth;
        Rect curveRect = new Rect(
            position.x + EditorGUIUtility.labelWidth,
            position.y + lineHeight + EditorGUIUtility.standardVerticalSpacing,
            curveWidth,
            curveHeight
        );
        
        //draw collapse triangle and set expanded state
        Rect foldoutRect = new Rect(
            position.x + EditorGUIUtility.labelWidth - 14,
            position.y,
            14,
            lineHeight
        );
        property.isExpanded = EditorGUI.Foldout(foldoutRect, property.isExpanded, GUIContent.none, true);

   
        //draw enum and check for value changes
        EditorGUI.BeginChangeCheck();
        EditorGUI.PropertyField(enumRect, property, label);
        if (previousY != position.y || EditorGUI.EndChangeCheck() || curvePoints == null)
        {
            if (property.isExpanded) //don't recalculate if curve isn't displayed anyway
            {
                previousY = position.y;
                
                //get enum value and recalculate curve
                var enumValue = (Ease.Type)property.intValue;
                curvePoints = RecalculateCurve(enumValue, curveRect);
            } 
        }
        
        //draw curve if not collapsed
        if (property.isExpanded)
            DrawCurvePreview(curveRect, curvePoints);

        EditorGUI.EndProperty();
    }
    
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float lineHeight = EditorGUIUtility.singleLineHeight;
        float spacing = EditorGUIUtility.standardVerticalSpacing;

        return lineHeight + spacing + (property.isExpanded ? curveHeight : 0);
    }

    private static Vector3[] RecalculateCurve(Ease.Type type, Rect rect)
    {
        Vector3[] points = new Vector3[steps];
            
        for (int i = 0; i < steps; i++)
        {
            float t = i / (float)steps;
            float value = Ease.OfType(t, type);

            Vector3 point = new Vector3(Mathf.Lerp(rect.x, rect.xMax, t), Mathf.Lerp(rect.yMax, rect.y, Mathf.InverseLerp(0, 1, value)), 0);
            points.SetValue(point, i);
        }

        return points;
    }
    
    private void DrawCurvePreview(Rect rect, Vector3[] points)
    {
        EditorGUI.DrawRect(rect, new Color(0.1647f, 0.1647f, 0.1647f));

        Handles.BeginGUI();
        Handles.color = color;

        for (int i = 1; i < points.Length; i++)
            Handles.DrawLine(points[i-1], points[i]);
        
        Handles.EndGUI();
    }
}