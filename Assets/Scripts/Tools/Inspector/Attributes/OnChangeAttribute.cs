using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
public class OnChangeAttribute : PropertyAttribute
{
    public readonly string MethodName;
    
    public OnChangeAttribute(string methodName)
    {
        MethodName = methodName;
    }
}
