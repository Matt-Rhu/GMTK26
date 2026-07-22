using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = true, Inherited =true)]
public class HideWithValueAttribute : PropertyAttribute
{
    public enum ValueType
    {
        Bool,
        Enum
    }
    public ValueType Type;
    
    public readonly string DynamicValueName;
    
    public readonly bool HideIfTrue;
    
    public readonly List<int> ValuesToCompare = new();
    public readonly bool HideIfEqual;
    
    
    public HideWithValueAttribute(string enumName, int valueToCompare, bool hideIfEqual)
    {
        Type = ValueType.Enum;
        
        DynamicValueName = enumName;
        ValuesToCompare.Add(valueToCompare);
        HideIfEqual = hideIfEqual;
    }
    
    public HideWithValueAttribute(string enumName, int[] valuesToCompare, bool hideIfEqual)
    {
        Type = ValueType.Enum;
        
        DynamicValueName = enumName;
        HideIfEqual = hideIfEqual;

        foreach (int i in valuesToCompare)
        {   
            ValuesToCompare.Add(i);
        }
    }
    
    public HideWithValueAttribute(string boolName, bool hideIfTrue)
    {
        Type = ValueType.Bool;
        
        DynamicValueName = boolName;
        HideIfTrue = hideIfTrue;
    }
    
    public HideWithValueAttribute(string boolName)
    {
        Type = ValueType.Bool;
        
        DynamicValueName = boolName;
        HideIfTrue = false;
    }
}
