using System;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.Rendering.PostProcessing;

[Serializable]
[PostProcess(typeof(PostProcessHueRenderer), PostProcessEvent.BeforeStack, "Matt-Rhu/Hue Gradient")]
public sealed class HuePP : PostProcessEffectSettings
{
    [Range(0, 1)] public FloatParameter intensity = new FloatParameter() { value = 0.5f };
    [Range(-1, 1)] public FloatParameter scale = new FloatParameter() { value = 0.5f };
    public FloatParameter offset = new FloatParameter() { value = 0 };
    [Range(0.1f, 3)] public FloatParameter power = new FloatParameter() { value = 1 };
}

public sealed class PostProcessHueRenderer : PostProcessEffectRenderer<HuePP>
{
    public override void Render(PostProcessRenderContext context)
    {
        var sheet = context.propertySheets.Get(Shader.Find("Hidden/Matt-Rhu/Hue Gradient"));
        
        sheet.properties.SetFloat("_Intensity", settings.intensity.value);
        sheet.properties.SetFloat("_Scale", settings.scale.value);
        sheet.properties.SetFloat("_Offset", settings.offset.value);
        sheet.properties.SetFloat("_Pow", settings.power.value);
        
        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
    }
}