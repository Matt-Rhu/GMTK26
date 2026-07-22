using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[Serializable]
[PostProcess(typeof(PostProcessDistortRenderer), PostProcessEvent.AfterStack, "Matt-Rhu/Distort")]
public sealed class DistortPP : PostProcessEffectSettings
{
    [Range(-100f, 100f), Tooltip("Total distortion amount.")]
    public FloatParameter intensity = new FloatParameter { value = 0f };
    
    [Range(0, 0.1f), Tooltip("Size of the smoothing on the edges")]
    public FloatParameter edgeSmoothing = new FloatParameter { value = 0.05f };
}

public sealed class PostProcessDistortRenderer : PostProcessEffectRenderer<DistortPP>
{
    private static readonly int DistortionCenterScale = Shader.PropertyToID("_Distortion_CenterScale");
    private static readonly int DistortionAmount = Shader.PropertyToID("_Distortion_Amount");
    private static readonly int EdgeSmooth = Shader.PropertyToID("_EdgeSmooth");

    public override void Render(PostProcessRenderContext context)
    {
        var sheet = context.propertySheets.Get(Shader.Find("Hidden/Matt-Rhu/Distort"));
        
        //Unity's default distortion THINGS, stripped down
        float amount = 1.6f * Math.Max(Mathf.Abs(settings.intensity.value), 1f);
        float theta = Mathf.Deg2Rad * Math.Min(160f, amount);
        float sigma = 2f * Mathf.Tan(theta * 0.5f);
        var p1 = new Vector4(settings.intensity.value >= 0f ? theta : 1f / theta, sigma, 1f, settings.intensity.value);

        if (settings.intensity.value == 0) sheet.DisableKeyword("DISTORT");
        else sheet.EnableKeyword("DISTORT");
        sheet.properties.SetVector(DistortionAmount, p1);
        sheet.properties.SetFloat(EdgeSmooth, settings.edgeSmoothing.value);
        
        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
    }
}