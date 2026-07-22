using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.Serialization;

[Serializable]
[PostProcess(typeof(PostProcessCRTRenderer), PostProcessEvent.AfterStack, "Matt-Rhu/CRT")]
public sealed class CRTPP : PostProcessEffectSettings
{
    [Range(0, 0.003f)] public FloatParameter blurAmount = new FloatParameter() { value = 0 };
    [ColorUsage(false, true)] public ColorParameter bloomColour = new ColorParameter() { value = Color.white };
    
    [Header("Scan Lines")]
    public FloatParameter linesTiling = new FloatParameter() { value = 100 };
    [Range(0, 1)] public FloatParameter linesOpacity = new FloatParameter() { value = 0.5f };
    public FloatParameter linesSpeed = new FloatParameter() { value = 10 };

    [Header("Dots")] 
    public IntParameter dotsTiling = new IntParameter() { value = 100 };
    [Range(0, 1)] public FloatParameter dotsSize = new FloatParameter() { value = 0.2f };
    [Range(0, 1)] public FloatParameter dotsSoftness = new FloatParameter() { value = 0.7f };
    [Range(0, 1)] public FloatParameter dotsOpacity = new FloatParameter() { value = 0.8f };
    [Range(0, 2)] public FloatParameter dotsRatio = new FloatParameter() { value = 1 };

    [Header("Waves")] 
    public FloatParameter wavesTiling = new FloatParameter() { value = 25 };
    [Range(0, 1)] public FloatParameter wavesIntensity = new FloatParameter() { value = 0.1f };
    public FloatParameter wavesSpeed = new FloatParameter() { value = 0.2f };
    
    [Header("Distortion")]
    [FormerlySerializedAs("intensity")] [Range(-100f, 100f), Tooltip("Total distortion amount.")]
    public FloatParameter distortIntensity = new FloatParameter { value = 0f };
    [Range(0, 0.1f), Tooltip("Size of the smoothing on the edges")]
    public FloatParameter edgeSmoothing = new FloatParameter { value = 0.05f };

    public FloatParameter unscaledTime = new FloatParameter() { value = 0 };
}

public sealed class PostProcessCRTRenderer : PostProcessEffectRenderer<CRTPP>
{
    public override void Render(PostProcessRenderContext context)
    {
        var sheet = context.propertySheets.Get(Shader.Find("Hidden/Matt-Rhu/CRT"));
        
        sheet.properties.SetFloat("_BlurAmount", settings.blurAmount.value);    
        sheet.properties.SetColor("_BloomColour", settings.bloomColour.value);
        sheet.properties.SetFloat("_LinesTiling", settings.linesTiling.value);
        sheet.properties.SetFloat("_LinesOpacity", settings.linesOpacity.value);
        sheet.properties.SetFloat("_LinesSpeed", settings.linesSpeed.value);
        sheet.properties.SetFloat("_PointsSize", settings.dotsSize.value);
        sheet.properties.SetFloat("_PointsSoftness", settings.dotsSoftness.value);
        sheet.properties.SetFloat("_PointsOpacity", settings.dotsOpacity.value);
        sheet.properties.SetFloat("_PointsRatio", settings.dotsRatio.value);
        sheet.properties.SetInt("_PointsTiling", settings.dotsTiling.value);
        sheet.properties.SetFloat("_WavesSpeed", settings.wavesSpeed.value);
        sheet.properties.SetFloat("_WavesStrength", settings.wavesIntensity.value);
        sheet.properties.SetFloat("_WavesTiling", settings.wavesTiling.value);
        sheet.properties.SetFloat("_UnscaledTime", settings.unscaledTime.value);
        
        //Unity's default distortion THINGS, stripped down
        float amount = 1.6f * Math.Max(Mathf.Abs(settings.distortIntensity.value), 1f);
        float theta = Mathf.Deg2Rad * Math.Min(160f, amount);
        float sigma = 2f * Mathf.Tan(theta * 0.5f);
        var p1 = new Vector4(settings.distortIntensity.value >= 0f ? theta : 1f / theta, sigma, 1f, settings.distortIntensity.value);

        if (settings.distortIntensity.value == 0) sheet.DisableKeyword("DISTORT");
        else sheet.EnableKeyword("DISTORT");
        sheet.properties.SetVector("_Distortion_Amount", p1);
        sheet.properties.SetFloat("_EdgeSmooth", settings.edgeSmoothing.value);
        
        var temporaryTexture = RenderTexture.GetTemporary(context.width, context.height);
        context.command.BlitFullscreenTriangle(context.source, temporaryTexture, sheet, 0);
        context.command.BlitFullscreenTriangle(temporaryTexture, context.destination, sheet, 1);
        RenderTexture.ReleaseTemporary(temporaryTexture);
    }
}