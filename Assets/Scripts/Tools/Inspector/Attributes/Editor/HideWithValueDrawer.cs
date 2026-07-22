using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(HideWithValueAttribute))]
public class HideWithValueDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        HideWithValueAttribute hideAttribute = attribute as HideWithValueAttribute;
        bool enable = !GetCondition(hideAttribute, property);
        if (enable)
        {
            EditorGUI.PropertyField(position, property, label, true);
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        //-2 instead of 0 because it's the space between two lines, prevents from getting a small gap between two variables if there's a hidden one between them
        float height = -2;
        HideWithValueAttribute hideAttribute = attribute as HideWithValueAttribute;
        bool enable = !GetCondition(hideAttribute, property);
        if (enable)
        {
            height = EditorGUI.GetPropertyHeight((property), true);
        }
        return height;

    }

    private bool GetCondition(HideWithValueAttribute hideAtt, SerializedProperty property)
    {
        //Handle primary property
        SerializedProperty sourcePropertyValue = null;
        //Get the full relative property path of the sourcefield so we can have nested hiding.Use old method when dealing with arrays
        string propertyPath = property.propertyPath; //returns the property path of the property we want to apply the attribute to
        string conditionPath = propertyPath.Replace(property.name, hideAtt.DynamicValueName); //changes the path to the conditionalsource property path
        sourcePropertyValue = property.serializedObject.FindProperty(conditionPath);

        //if the find failed->fall back to the old system
        if (sourcePropertyValue == null)
        {
            //original implementation (doens't work with nested serializedObjects)
            sourcePropertyValue = property.serializedObject.FindProperty(hideAtt.DynamicValueName);
        }
        
        
        switch (hideAtt.Type)
        {
            case HideWithValueAttribute.ValueType.Bool:
                return ConditionBool(hideAtt, sourcePropertyValue);
            case HideWithValueAttribute.ValueType.Enum:
                return ConditionEnum(hideAtt, sourcePropertyValue);
            default:
                return false;
        }
    }
    
    private bool ConditionBool(HideWithValueAttribute hideAtt, SerializedProperty sourcePropertyValue)
    {
        
        if (hideAtt.HideIfTrue)
            return sourcePropertyValue.boolValue;
        else 
            return !sourcePropertyValue.boolValue;
    }

    private bool ConditionEnum(HideWithValueAttribute hideAtt, SerializedProperty sourcePropertyValue)
    {
        bool hidden = false;
        
        if (hideAtt.HideIfEqual)
        {
            foreach (int i in hideAtt.ValuesToCompare)
            {
                if (sourcePropertyValue.enumValueIndex == i)
                {
                    hidden = true;
                    break;
                }
                hidden = false;
            }
        }
        else
        {
            foreach (int i in hideAtt.ValuesToCompare)
            {
                if (sourcePropertyValue.enumValueIndex == i)
                {
                    hidden = false;
                    break;
                }
                hidden = true;
            }
        }

        return hidden;
    }
}
