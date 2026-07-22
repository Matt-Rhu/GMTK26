using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[Serializable]
[PostProcess(typeof(PostProcessSpeedBlurRenderer), PostProcessEvent.BeforeTransparent, "Matt-Rhu/Post Process Speed Blur")]
public sealed class PostProcessSpeedBlur : PostProcessEffectSettings
{
    [Header("Blur")]
    [Range(1, 1.5f)] public FloatParameter scale = new FloatParameter { value = 0.5f };
    [Range(1, 30)] public IntParameter samples = new IntParameter { value = 3 };
    
    [Header("Falloff")]
    [Range(0.01f, 1)] public FloatParameter minWeight = new FloatParameter { value = 0.2f };
    [Range(0.01f, 2)] public FloatParameter weightPower = new FloatParameter { value = 1 };

    [Header("Mask")] 
    [Range(-1, 1)] public FloatParameter maskSize = new FloatParameter { value = 1 };
    [Range(0, 2)] public FloatParameter maskSoftness = new FloatParameter { value = 1 };
    [Range(0, 1)] public FloatParameter maskRoundness = new FloatParameter { value = 1 };
    public BoolParameter debugMask = new BoolParameter { value = false };
    
    [Header("Depth")]
    public BoolParameter affectedByDepth = new BoolParameter { value = false };
    public FloatParameter depthMult = new FloatParameter { value = 30000 };
    public FloatParameter depthWeight = new FloatParameter { value = 2};
    
    [Header("Weird")]
    public Vector2Parameter offset = new Vector2Parameter { value = new Vector2(0f, 0f) };
    [Range(-0.1f, 0.1f)] public FloatParameter rotate = new FloatParameter { value = 0 };
}

public sealed class PostProcessSpeedBlurRenderer : PostProcessEffectRenderer<PostProcessSpeedBlur>
{
    private static readonly int Scale = Shader.PropertyToID("_Scale");
    private static readonly int Samples = Shader.PropertyToID("_Samples");
    private static readonly int MinWeight = Shader.PropertyToID("_MinWeight");
    private static readonly int WeightPower = Shader.PropertyToID("_WeightPower");
    private static readonly int ScreenRatio = Shader.PropertyToID("_ScreenRatio");
    private static readonly int MaskRoundness = Shader.PropertyToID("_MaskRoundness");
    private static readonly int Offset = Shader.PropertyToID("_Offset");
    private static readonly int MaskSize = Shader.PropertyToID("_MaskSize");
    private static readonly int MaskSoftness = Shader.PropertyToID("_MaskSoftness");
    private static readonly int Rotate = Shader.PropertyToID("_Rotate");
    private static readonly int DepthMult = Shader.PropertyToID("_DepthMult");
    private static readonly int DepthWeight = Shader.PropertyToID("_DepthWeight");

    public override void Render(PostProcessRenderContext context)
    {
        var sheet = context.propertySheets.Get(Shader.Find("Hidden/Matt-Rhu/Speed Blur Post Process"));
        
        sheet.properties.SetFloat(Scale, 1 - settings.scale.value / 2);
        sheet.properties.SetInt(Samples, settings.samples.value);
        
        sheet.properties.SetFloat(MinWeight, settings.minWeight.value);
        sheet.properties.SetFloat(WeightPower, settings.weightPower.value);
        
        sheet.properties.SetFloat(ScreenRatio, (float)context.width / context.height);
        sheet.properties.SetFloat(MaskRoundness, settings.maskRoundness.value);
        sheet.properties.SetFloat(MaskSize, settings.maskSize.value);
        sheet.properties.SetFloat(MaskSoftness, settings.maskSoftness.value);
        
        if (settings.debugMask.value) sheet.EnableKeyword("DEBUG_MASK");
        else sheet.DisableKeyword("DEBUG_MASK");
        
        if (settings.affectedByDepth.value) sheet.EnableKeyword("DEPTH");
        else sheet.DisableKeyword("DEPTH");
        
        sheet.properties.SetVector(Offset, new Vector2(0.5f, 0.5f) + settings.offset.value/10);
        sheet.properties.SetFloat(Rotate, settings.rotate);
        
        sheet.properties.SetFloat(DepthMult, settings.depthMult);
        sheet.properties.SetFloat(DepthWeight, settings.depthWeight);
        
        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
    }
}