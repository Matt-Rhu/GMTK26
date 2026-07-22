using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[Serializable]
[PostProcess(typeof(PostProcessDitherRenderer), PostProcessEvent.BeforeStack, "Matt-Rhu/Dithering")]
public sealed class DitherPP : PostProcessEffectSettings
{
    [Range(0, 1)] public FloatParameter spread = new FloatParameter { value = 0.5f };

    public BoolParameter colourCompress = new BoolParameter { value = true };
    [Range(2, 32)] public IntParameter redColourCount = new IntParameter { value = 8 };
    [Range(2, 32)] public IntParameter greenColourCount = new IntParameter { value = 8 };
    [Range(2, 32)] public IntParameter blueColourCount = new IntParameter { value = 8 };

    [Range(0, 2)] public IntParameter bayerLevel = new IntParameter() { value = 0 };

    [Range(1, 8)] public IntParameter downSamples = new IntParameter() { value = 0 };

    public BoolParameter pointFilterDown = new BoolParameter { value = false };
}

public sealed class PostProcessDitherRenderer : PostProcessEffectRenderer<DitherPP>
{
    
    public override void Render(PostProcessRenderContext context)
    {
        var sheet = context.propertySheets.Get(Shader.Find("Hidden/Dither"));

        //set shader vars
        sheet.properties.SetFloat("_Spread", settings.spread.value);
        if (settings.colourCompress.value)
        {
            sheet.properties.SetInt("_RedColorCount", settings.redColourCount.value);
            sheet.properties.SetInt("_GreenColorCount", settings.greenColourCount.value);
            sheet.properties.SetInt("_BlueColorCount", settings.blueColourCount.value);
        }
        else
        {
            sheet.properties.SetInt("_RedColorCount", 0);
        }
        sheet.properties.SetInt("_BayerLevel", settings.bayerLevel.value);

        int width = context.width;
        int height = context.height;
        
        RenderTexture[] textures = new RenderTexture[8];
        
        var currentSource = context.source;
        
        //downscaling
        for (int i = 0; i < settings.downSamples.value; i++)
        {
            width /= 2;
            height /= 2;
    
            if (height < 2) break;
    
            var currentDestination = textures[i] = RenderTexture.GetTemporary(width, height, 0, context.sourceFormat);
        
            if (settings.pointFilterDown)
                context.command.BlitFullscreenTriangle(currentSource, currentDestination, sheet, 1);
            else
                context.command.BuiltinBlit(currentSource, currentDestination);
    
            currentSource = currentDestination;
        }
        
        //dither
        RenderTexture dither = RenderTexture.GetTemporary(width, height, 0, context.sourceFormat);
        context.command.BlitFullscreenTriangle(currentSource, dither, sheet, 0);
        
        context.command.BlitFullscreenTriangle(dither, context.destination, sheet, 1);
        
        RenderTexture.ReleaseTemporary(dither);
        for (int i = 0; i < settings.downSamples.value; i++)
        {
            RenderTexture.ReleaseTemporary(textures[i]);   
        }
    }
}