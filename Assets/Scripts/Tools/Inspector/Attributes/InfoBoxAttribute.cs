using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoBoxAttribute : PropertyAttribute
{
    public string Message;
    public bool ShowProperty;

    public InfoBoxAttribute(string message, bool showProperty = true)
    {
        Message = message;
        ShowProperty = showProperty;
    }
}
