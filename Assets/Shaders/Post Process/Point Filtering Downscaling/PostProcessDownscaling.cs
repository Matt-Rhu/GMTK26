using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

[Serializable]
[PostProcess(typeof(PostProcessDownscalingRenderer), PostProcessEvent.AfterStack, "Matt-Rhu/Post Process Downscaling")]
public sealed class PostProcessDownscaling : PostProcessEffectSettings
{
    [Range(2, 1080)] public IntParameter resolution = new IntParameter { value = 360 };
    // [Range(0, 8)]
    // public IntParameter downSamples = new IntParameter {value = 0};
}

public sealed class PostProcessDownscalingRenderer : PostProcessEffectRenderer<PostProcessDownscaling>
{
    public override void Render(PostProcessRenderContext context)
    {
        var sheet = context.propertySheets.Get(Shader.Find("Hidden/Matt-Rhu/Downscaling Post Process"));
        
        float pixelHeight = settings.resolution.value;
        float pixelWidth = pixelHeight * context.width / context.height;
        
        var temporaryTexture = RenderTexture.GetTemporary((int)pixelWidth, (int)pixelHeight);
        temporaryTexture.filterMode = FilterMode.Point;
        
        context.command.BlitFullscreenTriangle(context.source, temporaryTexture, sheet, 0);
        context.command.Blit(temporaryTexture, context.destination);
        RenderTexture.ReleaseTemporary(temporaryTexture);
        
        // var sheet = context.propertySheets.Get(Shader.Find("Hidden/Matt-Rhu/Downscaling Post Process"));
        //
        // int width = context.width;
        // int height = context.height;
        //
        //
        // RenderTexture[] textures = new RenderTexture[8];
        //
        // RenderTargetIdentifier currentSource = context.source;
        //
        // for (int i = 0; i < settings.downSamples.value; ++i) 
        // {
        //     width /= 2;
        //     height /= 2;
        //
        //     if (height < 2)
        //         break;
        //
        //     RenderTexture currentDestination = textures[i] = RenderTexture.GetTemporary(width, height, 0, context.sourceFormat);
        //     textures[i].filterMode = FilterMode.Point;
        //     context.command.BlitFullscreenTriangle(currentSource, currentDestination, sheet, 0);
        //     currentSource = currentDestination;
        // }
        //
        // context.command.Blit(currentSource, context.destination);
        //
        // for (int i = 0; i < settings.downSamples.value; ++i) 
        // {
        //     RenderTexture.ReleaseTemporary(textures[i]);
        // }
    }
}