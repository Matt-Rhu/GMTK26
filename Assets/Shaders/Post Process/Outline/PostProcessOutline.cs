using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[Serializable]
[PostProcess(typeof(PostProcessOutlineRenderer), PostProcessEvent.BeforeStack, "Matt-Rhu/Post Process Outline")]
public sealed class PostProcessOutline : PostProcessEffectSettings
{
    [ColorUsage(false, true)] public ColorParameter color = new ColorParameter { value = Color.black };
    public IntParameter scale = new IntParameter { value = 1 };
    [Range(0,1)] public IntParameter edgesAroundColour = new IntParameter { value = 0 };
    [Header("Scale Noise")] 
    [Range(0, 1)] public FloatParameter noiseAmount = new FloatParameter { value = 0 };
    public FloatParameter noiseScale = new FloatParameter { value = 5 };
    public FloatParameter noiseSpeed = new FloatParameter { value = 5 };
    [Header("Scale with Distance")] 
    public FloatParameter distanceLength = new FloatParameter { value = 10 };
    [Range(0, 1)] public FloatParameter distanceOffset = new FloatParameter { value = 0 };
    public FloatParameter distancePower = new FloatParameter { value = 1 };
    [Header("Thresholds")]
    public FloatParameter depthThreshold = new FloatParameter { value = 1.5f };
    [Range(0, 1)] public FloatParameter normalThreshold = new FloatParameter { value = 0.4f };
    [Range(0, 1)] public FloatParameter colorThreshold = new FloatParameter { value = 0.4f };
    [Range(0, 1)] public FloatParameter depthNormalThreshold = new FloatParameter { value = 0.5f };
    public FloatParameter depthNormalThresholdScale = new FloatParameter { value = 7 };
}

public sealed class PostProcessOutlineRenderer : PostProcessEffectRenderer<PostProcessOutline>
{
    private static readonly int Scale = Shader.PropertyToID("_Scale");
    private static readonly int DepthThreshold = Shader.PropertyToID("_DepthThreshold");
    private static readonly int NormalThreshold = Shader.PropertyToID("_NormalThreshold");
    private static readonly int ClipToView = Shader.PropertyToID("_ClipToView");
    private static readonly int DepthNormalThreshold = Shader.PropertyToID("_DepthNormalThreshold");
    private static readonly int DepthNormalThresholdScale = Shader.PropertyToID("_DepthNormalThresholdScale");
    private static readonly int Color1 = Shader.PropertyToID("_Color");
    private static readonly int DistanceLength = Shader.PropertyToID("_DistanceLength");
    private static readonly int DistanceOffset = Shader.PropertyToID("_DistanceOffset");
    private static readonly int DistancePower = Shader.PropertyToID("_DistancePower");
    private static readonly int NoiseScale = Shader.PropertyToID("_NoiseScale");
    private static readonly int NoiseAmount = Shader.PropertyToID("_NoiseAmount");
    private static readonly int NoiseSpeed = Shader.PropertyToID("_NoiseSpeed");
    private static readonly int EdgesAroundColor = Shader.PropertyToID("_EdgesAroundColor");
    private static readonly int ColorThreshold = Shader.PropertyToID("_ColorThreshold");

    public override void Render(PostProcessRenderContext context)
    {
        var sheet = context.propertySheets.Get(Shader.Find("Hidden/Matt-Rhu/Outline Post Process"));
        sheet.properties.SetColor(Color1, settings.color);
        sheet.properties.SetFloat(Scale, settings.scale);
        sheet.properties.SetInt(EdgesAroundColor, settings.edgesAroundColour);
        sheet.properties.SetFloat(NoiseAmount, settings.noiseAmount);
        sheet.properties.SetFloat(NoiseScale, settings.noiseScale);
        sheet.properties.SetFloat(NoiseSpeed, settings.noiseSpeed);
        sheet.properties.SetFloat(DistanceLength, settings.distanceLength);
        sheet.properties.SetFloat(DistanceOffset, settings.distanceOffset);
        sheet.properties.SetFloat(DistancePower, settings.distancePower);
        sheet.properties.SetFloat(DepthThreshold, settings.depthThreshold);
        sheet.properties.SetFloat(NormalThreshold, settings.normalThreshold);
        sheet.properties.SetFloat(ColorThreshold, settings.colorThreshold);
        sheet.properties.SetFloat(DepthNormalThreshold, settings.depthNormalThreshold);
        sheet.properties.SetFloat(DepthNormalThresholdScale, settings.depthNormalThresholdScale);
        
        Matrix4x4 clipToView = GL.GetGPUProjectionMatrix(context.camera.projectionMatrix, true).inverse;
        sheet.properties.SetMatrix(ClipToView, clipToView);
        
        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
    }
}