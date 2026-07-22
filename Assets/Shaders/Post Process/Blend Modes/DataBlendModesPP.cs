using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using System;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Blend Modes Data", menuName = "Blend Modes Post Proc", order = 0)] [Serializable]
public class DataBlendModesPP : ScriptableObject
{
    public List<BlendLayer> blendLayers;
}

[Serializable]
public class BlendLayer
{
    [Tooltip("- NO BLEND: Does nothing. \n" +
             "- DARKEN: Keeps the darkest colour \n" +
             "- MULTIPLY: Blends hues, darker image. \n" +
             "- COLOUR BURN: Very dark, high contrast, bright areas of the base image pop out. \n" +
             "- LINEAR BURN: Similar to Colour Burn, bright areas not as bright. \n" +
             "- LIGHTEN: Keeps the lightest colour. \n" +
             "- SCREEN: Blends hues, lighter image. \n" +
             "- COLOUR DODGE: Blends colour based on blend layer lightness, lighter image. \n" +
             "- LINEAR DODGE: Basically Add. \n" +
             "- OVERLAY: Combination of Screen and Multiply, lighter where the base image is light, darker where it is dark. \n" +
             "- SOFT LIGHT: Similar to Hard Light but more subtle, less contrasted. \n" +
             "- HARD LIGHT: Similar to Overlay but checking blend colour's lightness instead, high contrast. \n" +
             "- VIVID LIGHT: Combination of Colour Burn and Colour Dodge, lighter where the base image is light, darker where it is dark. \n" +
             "- LINEAR LIGHT: Combination of Linear Burn and Linear dodge. \n" +
             "- PIN LIGHT: Combination of Lighten and Darken, lighter where the base image is light, darker where it is dark. \n" +
             "- HUE: Hue from blend layer, Saturation and Value from base layer. \n" +
             "- SATURATION: Saturation from blend layer, Hue and Value from base layer. \n" +
             "- COLOUR: Hue and Saturation from blend layer, Value from base layer. \n" +
             "- VALUE: Value from blend layer, Hue and Saturation from base layer.")] 
    public BlendMode blendMode = 0;
    public BlendType blendType = 0;
    
    public Texture blendTexture = null;
    [ColorUsage(false, false)] 
    public Color blendColour = Color.black;
    
    [Range(0, 1)] public float strength = 1;
}
