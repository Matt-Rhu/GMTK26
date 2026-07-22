using System;
using UnityEngine;

[Serializable]
public class MethodReference
{
    [SerializeField] private string name;

    public static implicit operator string(MethodReference reference)
    {
        return reference.name;
    }
}
