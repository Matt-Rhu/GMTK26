using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Method)]
public class ButtonAttribute : Attribute
{
    public readonly string Name;
    public readonly string Condition;
    public readonly float Height;
    public readonly bool BeforeVars;
    public readonly bool AskConfirmation;

    public ButtonAttribute(string name = null, bool beforeVars = false, float height = 40, string condition = null, bool askConfirmation = false)
    {
        Name = name;
        Condition = condition;
        Height = height;
        BeforeVars = beforeVars;
        AskConfirmation = askConfirmation;
    }
    
    //different order for convenience
    public ButtonAttribute(string name = null, bool askConfirmation = false, string condition = null, bool beforeVars = false, float height = 40)
    {
        Name = name;
        Condition = condition;
        Height = height;
        BeforeVars = beforeVars;
        AskConfirmation = askConfirmation;
    }
    
    public ButtonAttribute(string name = null, string condition = null, bool askConfirmation = false, bool beforeVars = false, float height = 40)
    {
        Name = name;
        Condition = condition;
        Height = height;
        BeforeVars = beforeVars;
        AskConfirmation = askConfirmation;
    }

    public ButtonAttribute(string name = null)
    {
        Name = name;
        Condition = null;
        Height = 40;
        BeforeVars = false;
        AskConfirmation = false;
    }
    
    public ButtonAttribute()
    {
        Name = null;
        Condition = null;
        Height = 40;
        BeforeVars = false;
        AskConfirmation = false;
    }
}