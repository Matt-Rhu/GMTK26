using System;
using UnityEngine;

[Serializable] 
public enum BlendMode
{
    [InspectorName("No Blend")] NoBlend = 0,
    [InspectorName("Darker/Darken")] Darken,
    [InspectorName("Darker/Multiply")] Multiply,
    [InspectorName("Darker/Colour Burn")] ColourBurn,
    [InspectorName("Darker/Linear Burn")] LinearBurn,
    [InspectorName("Lighter/Lighten")] Lighten,
    [InspectorName("Lighter/Screen")] Screen,
    [InspectorName("Lighter/Colour Dodge")] ColourDodge,
    [InspectorName("Lighter/Linear Dodge")] LinearDodge,
    [InspectorName("Contrasted/Overlay")] Overlay,
    [InspectorName("Contrasted/Soft Light")] SoftLight,
    [InspectorName("Contrasted/Hard Light")] HardLight,
    [InspectorName("Contrasted/Vivid Light")] VividLight,
    [InspectorName("Contrasted/Linear Light")] LinearLight,
    [InspectorName("Contrasted/Pin Light")] PinLight,
    [InspectorName("HSV/Hue")] Hue,
    [InspectorName("HSV/Saturation")] Saturation,
    [InspectorName("HSV/Colour")] Colour,
    [InspectorName("HSV/Value")] Value
}
[Serializable]
public enum BlendType
{
    WithColour = 0,
    WithTexture,
    WithItself
}
