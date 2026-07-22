using System;
using UnityEngine;

public struct Ease
{
    public enum Type
    {
        [InspectorName("Linear")] Linear = 0,
        
        [InspectorName("In/Sine")] InSine = 1,
        [InspectorName("In/Quad")] InQuad = 2,
        [InspectorName("In/Cubic")] InCubic = 3,
        [InspectorName("In/Quart")] InQuart = 4,
        [InspectorName("In/Quint")] InQuint = 5,
        [InspectorName("In/Expo")] InExpo = 6,
        [InspectorName("In/Circ")] InCirc = 7,
        
        [InspectorName("Out/Sine")] OutSine = 11,
        [InspectorName("Out/Quad")] OutQuad = 12,
        [InspectorName("Out/Cubic")] OutCubic = 13,
        [InspectorName("Out/Quart")] OutQuart = 14,
        [InspectorName("Out/Quint")] OutQuint = 15,
        [InspectorName("Out/Expo")] OutExpo = 16,
        [InspectorName("Out/Circ")] OutCirc = 17,
        
        [InspectorName("InOut/Sine")] InOutSine = 21,
        [InspectorName("InOut/Quad")] InOutQuad = 22,
        [InspectorName("InOut/Cubic")] InOutCubic = 23,
        [InspectorName("InOut/Quart")] InOutQuart = 24,
        [InspectorName("InOut/Quint")] InOutQuint = 25,
        [InspectorName("InOut/Expo")] InOutExpo = 26,
        [InspectorName("InOut/Circ")] InOutCirc = 27
    }

    public static float OfType(float input, Type t)
    {
        return t switch
        {
            Type.Linear => input,
            
            Type.InSine => InSine(input),
            Type.InQuad => InQuad(input),
            Type.InCubic => InCubic(input),
            Type.InQuart => InQuart(input),
            Type.InQuint => InQuint(input),
            Type.InExpo => InExpo(input),
            Type.InCirc => InCirc(input),
            
            Type.OutSine => OutSine(input),
            Type.OutQuad => OutQuad(input),
            Type.OutCubic => OutCubic(input),
            Type.OutQuart => OutQuart(input),
            Type.OutQuint => OutQuint(input),
            Type.OutExpo => OutExpo(input),
            Type.OutCirc => OutCirc(input),
            
            Type.InOutSine => InOutSine(input),
            Type.InOutQuad => InOutQuad(input),
            Type.InOutCubic => InOutCubic(input),
            Type.InOutQuart => InOutQuart(input),
            Type.InOutQuint => InOutQuint(input),
            Type.InOutExpo => InOutExpo(input),
            Type.InOutCirc => InOutCirc(input),
            
            _ => input
        };
    }
    
    // SINE
    public static float InSine(float input)
    {
        input = Mathf.Clamp01(input);
        return 1f - Mathf.Cos(input * Mathf.PI * 0.5f);
    }
    public static float OutSine(float input)
    {
        input = Mathf.Clamp01(input);
        return Mathf.Sin(input * Mathf.PI * 0.5f);
    }
    public static float InOutSine(float input)
    {
        input = Mathf.Clamp01(input);
        return -(Mathf.Cos(Mathf.PI * input) - 1f) * 0.5f;
    }

    //QUAD
    public static float InQuad(float input)
    {
        input = Mathf.Clamp01(input);
        return input * input;
    }
    public static float OutQuad(float input)
    {
        input = Mathf.Clamp01(input);
        return 1f - (1f - input) * (1f - input);
    }
    public static float InOutQuad(float input)
    {
        input = Mathf.Clamp01(input);
        if (input < 0.5f) return 2f * input * input;
        return 1f - Mathf.Pow(-2f * input + 2f, 2f) * 0.5f;
    }

    // CUBIC
    public static float InCubic(float input)
    {
        input = Mathf.Clamp01(input);
        return input * input * input;
    }
    public static float OutCubic(float input)
    {
        input = Mathf.Clamp01(input);
        return 1f - Mathf.Pow(1f - input, 3f);
    }
    public static float InOutCubic(float input)
    {
        input = Mathf.Clamp01(input);
        if (input < 0.5f) return 4f * input * input * input;
        return 1f - Mathf.Pow(-2f * input + 2f, 3f) * 0.5f;
    }

    // QUART
    public static float InQuart(float input)
    {
        input = Mathf.Clamp01(input);
        return input * input * input * input;
    }
    public static float OutQuart(float input)
    {
        input = Mathf.Clamp01(input);
        return 1f - Mathf.Pow(1f - input, 4f);
    }
    public static float InOutQuart(float input)
    {
        input = Mathf.Clamp01(input);
        if (input < 0.5f) return 8f * input * input * input * input;
        return 1f - Mathf.Pow(-2f * input + 2f, 4f) * 0.5f;
    }

    // QUINT
    public static float InQuint(float input)
    {
        input = Mathf.Clamp01(input);
        return input * input * input * input * input;
    }
    public static float OutQuint(float input)
    {
        input = Mathf.Clamp01(input);
        return 1f - Mathf.Pow(1f - input, 5f);
    }
    public static float InOutQuint(float input)
    {
        input = Mathf.Clamp01(input);
        if (input < 0.5f) return 16f * input * input * input * input * input;
        return 1f - Mathf.Pow(-2f * input + 2f, 5f) * 0.5f;
    }

    // EXPO
    public static float InExpo(float input)
    {
        input = Mathf.Clamp01(input);
        if (input == 0f) return 0f;
        return Mathf.Pow(2f, 10f * input - 10f);
    }
    public static float OutExpo(float input)
    {
        input = Mathf.Clamp01(input);
        if (input == 1f) return 1f;
        return 1f - Mathf.Pow(2f, -10f * input);
    }
    public static float InOutExpo(float input)
    {
        input = Mathf.Clamp01(input);
        if (input == 0f) return 0f;
        if (input == 1f) return 1f;
        if (input < 0.5f)
            return Mathf.Pow(2f, 20f * input - 10f) * 0.5f;
        return (2f - Mathf.Pow(2f, -20f * input + 10f)) * 0.5f;
    }

    // CIRC
    public static float InCirc(float input)
    {
        input = Mathf.Clamp01(input);
        return 1f - Mathf.Sqrt(1f - input * input);
    }
    public static float OutCirc(float input)
    {
        input = Mathf.Clamp01(input);
        return Mathf.Sqrt(1f - Mathf.Pow(input - 1f, 2f));
    }
    public static float InOutCirc(float input)
    {
        input = Mathf.Clamp01(input);
        if (input < 0.5f)
            return (1f - Mathf.Sqrt(1f - Mathf.Pow(2f * input, 2f))) * 0.5f;
        return (Mathf.Sqrt(1f - Mathf.Pow(-2f * input + 2f, 2f)) + 1f) * 0.5f;
    }
}