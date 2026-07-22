using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[Serializable]
[PostProcess(typeof(MultiBlendModesPPRenderer), PostProcessEvent.AfterStack, "Matt-Rhu/Multi Blend Modes")]
public sealed class MultiBlendModesPP : PostProcessEffectSettings
{
    [Serializable]
    public sealed class BlendModesData : ParameterOverride<DataBlendModesPP> {}

    public BlendModesData blendModesData = new BlendModesData() {value = null};
}

public sealed class MultiBlendModesPPRenderer : PostProcessEffectRenderer<MultiBlendModesPP>
{
    private static readonly int Tex = Shader.PropertyToID("_Tex");
    private static readonly int Colour = Shader.PropertyToID("_Colour");
    private static readonly int Strength = Shader.PropertyToID("_Strength");
    private static readonly int BlendType = Shader.PropertyToID("_BlendType");

    public override void Render(PostProcessRenderContext context)
    {
        var sheet = context.propertySheets.Get(Shader.Find("Hidden/Matt-Rhu/Blend Modes"));
        List<BlendLayer> layers = null;
        if (settings.blendModesData.value != null)
        {
             layers = settings.blendModesData.value.blendLayers;
        }

        if (layers == null || layers.Count == 0)
        {
            context.command.Blit(context.source, context.destination);
            return;
        }

        var TempTex = RenderTexture.GetTemporary(context.width, context.height);
        
        for (int i = 0; i < layers.Count; i++)
        {
           
            sheet.properties.SetInt(BlendType, (int)layers[i].blendType);
        
            Texture texParameter = layers[i].blendTexture;
            if (texParameter == null) texParameter = RuntimeUtilities.blackTexture;
            sheet.properties.SetTexture(Tex, texParameter);
        
            sheet.properties.SetVector(Colour, layers[i].blendColour);
            sheet.properties.SetFloat(Strength, layers[i].strength);

            if (layers.Count > 1)
            {
                if (i == 0)
                {
                    context.command.BlitFullscreenTriangle(context.source, TempTex, sheet, (int)layers[i].blendMode);
                }
                else if (i == layers.Count - 1)
                {
                    context.command.BlitFullscreenTriangle(TempTex, context.destination, sheet, (int)layers[i].blendMode);
                }
                else
                {
                    var nextTempTex = RenderTexture.GetTemporary(context.width, context.height);
                    context.command.BlitFullscreenTriangle(TempTex, nextTempTex, sheet, (int)layers[i].blendMode);
                    TempTex = nextTempTex;
                }
            }
            else
            {
                context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, (int)layers[i].blendMode);
            }
            
        }
        RenderTexture.ReleaseTemporary(TempTex);
    }
}