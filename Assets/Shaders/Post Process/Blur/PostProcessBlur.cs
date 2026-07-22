using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

[Serializable]
[PostProcess(typeof(PostProcessBlurRenderer), PostProcessEvent.AfterStack, "Matt-Rhu/Post Process Blur")]
public sealed class PostProcessBlur : PostProcessEffectSettings
{
    [Range(0, 0.1f)] public FloatParameter blurSize = new FloatParameter {value = 0};
    [Range(2, 100)] public IntParameter blurSamples = new IntParameter {value = 10};
    public TextureParameter mask = new TextureParameter {value = null};
    [Tooltip("Me when the blur is gaussian")] public BoolParameter gaussian = new BoolParameter {value = false};
    [Range(0, 0.1f)] public FloatParameter standardDeviation = new FloatParameter { value = 0.02f };
}

public sealed class PostProcessBlurRenderer : PostProcessEffectRenderer<PostProcessBlur>
{
    private static readonly int BlurSize = Shader.PropertyToID("_BlurSize");
    private static readonly int BlurSamples = Shader.PropertyToID("_BlurSamples");
    private static readonly int Mask = Shader.PropertyToID("_Mask");
    private static readonly int StandardDeviation = Shader.PropertyToID("_StandardDeviation");

    public override void Render(PostProcessRenderContext context)
    {
        var sheet = context.propertySheets.Get(Shader.Find("Hidden/Matt-Rhu/Blur Post Process"));
        
        sheet.properties.SetFloat(BlurSize, settings.blurSize);
        sheet.properties.SetInt(BlurSamples, settings.blurSamples);
        sheet.properties.SetFloat(StandardDeviation, settings.standardDeviation);
        
        //get a fucking white texture as a default goddammit
        Texture texParameter = settings.mask.value;
        if ( texParameter == null)
        {
            texParameter = RuntimeUtilities.whiteTexture;
        }
        sheet.properties.SetTexture(Mask, texParameter);
        
        //set shader keyword for gaussian blur
        if (settings.gaussian) sheet.EnableKeyword("GAUSS");
        else sheet.DisableKeyword("GAUSS");
        
        //send vertical blur pass to temporary render tex, apply horizontal blur pass on tex, send result to screen
        var temporaryTexture = RenderTexture.GetTemporary(context.width, context.height);
        context.command.BlitFullscreenTriangle(context.source, temporaryTexture, sheet, 0);
        context.command.BlitFullscreenTriangle(temporaryTexture, context.destination, sheet, 1);
        RenderTexture.ReleaseTemporary(temporaryTexture);
    }
    
}