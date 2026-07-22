using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[Serializable]
[PostProcess(typeof(BlendModesPPRenderer), PostProcessEvent.AfterStack, "Matt-Rhu/Blend Modes")]
public sealed class BlendModesPP : PostProcessEffectSettings
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
    public BlendModeParameter blendMode = new BlendModeParameter() { value = 0 };
    public BlendTypeParameter blendType = new BlendTypeParameter() { value = 0 };
    
    public TextureParameter blendTexture = new TextureParameter() { value = null };
    [ColorUsage(false, false)] 
    public ColorParameter blendColour = new ColorParameter() { value = Color.black };
    
    [Range(0, 1)] public FloatParameter strength = new FloatParameter() { value = 1 };
}


[Serializable]
public sealed class BlendModeParameter : ParameterOverride<BlendMode> {}
[Serializable]
public sealed class BlendTypeParameter : ParameterOverride<BlendType> {}


public sealed class BlendModesPPRenderer : PostProcessEffectRenderer<BlendModesPP>
{
    private static readonly int Tex = Shader.PropertyToID("_Tex");
    private static readonly int Colour = Shader.PropertyToID("_Colour");
    private static readonly int Strength = Shader.PropertyToID("_Strength");
    private static readonly int BlendType = Shader.PropertyToID("_BlendType");

    public override void Render(PostProcessRenderContext context)
    {
        var sheet = context.propertySheets.Get(Shader.Find("Hidden/Matt-Rhu/Blend Modes"));
        
        sheet.properties.SetInt(BlendType, (int)settings.blendType.value);
        
        Texture texParameter = settings.blendTexture.value;
        if (texParameter == null) texParameter = RuntimeUtilities.blackTexture;
        sheet.properties.SetTexture(Tex, texParameter);
        
        sheet.properties.SetVector(Colour, settings.blendColour.value);
        sheet.properties.SetFloat(Strength, settings.strength.value);
        
        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, (int)settings.blendMode.value);
    }
}