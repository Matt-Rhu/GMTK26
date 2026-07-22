using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[Serializable]
[PostProcess(typeof(PostProcessGodRaysRenderer), PostProcessEvent.BeforeTransparent, "Matt-Rhu/Post Process God Rays")]
public sealed class PostProcessGodRays : PostProcessEffectSettings
{
    [Header("Rays")]
    [Range(0, 1f)] public FloatParameter intensity = new FloatParameter { value = 0.5f };
    [Range(1, 2f)] public FloatParameter scale = new FloatParameter { value = 0.5f };
    [Range(1, 100)] public IntParameter samples = new IntParameter { value = 3 };
    public TextureParameter mask = new TextureParameter { value = null };
    
    [Header("Falloff")]
    [Range(0.01f, 1)] public FloatParameter minWeight = new FloatParameter { value = 0.2f };
    [Range(0.01f, 3)] public FloatParameter weightPower = new FloatParameter { value = 1 };

    [Header("Threshold")] 
    [Range(0.3f, 2)] public FloatParameter threshold = new FloatParameter { value = 1 };
    [Range(0.1f, 10)] public FloatParameter pow = new FloatParameter { value = 3 };
    
    [Header("Weird")]
    public Vector2Parameter offset = new Vector2Parameter { value = new Vector2(0f, 0f) };
}

public sealed class PostProcessGodRaysRenderer : PostProcessEffectRenderer<PostProcessGodRays>
{
    private static readonly int Scale = Shader.PropertyToID("_Scale");
    private static readonly int Samples = Shader.PropertyToID("_Samples");
    private static readonly int MinWeight = Shader.PropertyToID("_MinWeight");
    private static readonly int WeightPower = Shader.PropertyToID("_WeightPower");
    private static readonly int Offset = Shader.PropertyToID("_Offset");
    private static readonly int Intensity = Shader.PropertyToID("_Intensity");

    public override void Render(PostProcessRenderContext context)
    {
        var sheet = context.propertySheets.Get(Shader.Find("Hidden/Matt-Rhu/God Rays Post Process"));
        
        sheet.properties.SetFloat(Intensity, settings.intensity.value);
        sheet.properties.SetFloat(Scale, 1 - settings.scale.value / 2);
        sheet.properties.SetInt(Samples, settings.samples.value);
        
        sheet.properties.SetFloat(MinWeight, settings.minWeight.value);
        sheet.properties.SetFloat(WeightPower, settings.weightPower.value);
        
        sheet.properties.SetFloat("_ThresholdDiv", 1 - settings.threshold.value * 3);
        sheet.properties.SetFloat("_ThresholdPow", settings.pow.value);
        sheet.properties.SetFloat("_ThresholdMult", settings.threshold.value * 5);
        
        Texture texParameter = settings.mask.value;
        if ( texParameter == null)
        {
            texParameter = RuntimeUtilities.whiteTexture;
        }
        sheet.properties.SetTexture("_Mask", texParameter);
        
        sheet.properties.SetVector(Offset, new Vector2(0.5f, 0.5f) + settings.offset.value/30);
        
        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
    }
}