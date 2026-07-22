using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[Serializable]
[PostProcess(typeof(PostProcessLevelsRenderer), PostProcessEvent.AfterStack, "Matt-Rhu/Levels")]
public sealed class LevelsPP : PostProcessEffectSettings
{
    [Range(0, 1)] public FloatParameter minOld = new FloatParameter { value = 0 };
    [Range(0, 1)] public FloatParameter maxOld = new FloatParameter { value = 1 };
    [Range(0, 1)] public FloatParameter minNew = new FloatParameter { value = 0 };
    [Range(0, 1)] public FloatParameter maxNew = new FloatParameter { value = 1 };

    public BoolParameter preventInversion = new BoolParameter { value = true };
}

public sealed class PostProcessLevelsRenderer : PostProcessEffectRenderer<LevelsPP>
{
    private static readonly int MinOld = Shader.PropertyToID("_MinOld");
    private static readonly int MaxOld = Shader.PropertyToID("_MaxOld");
    private static readonly int MinNew = Shader.PropertyToID("_MinNew");
    private static readonly int MaxNew = Shader.PropertyToID("_MaxNew");


    public override void Render(PostProcessRenderContext context)
    {
        var sheet = context.propertySheets.Get(Shader.Find("Hidden/Matt-Rhu/Levels"));

        float miO = settings.minOld.value;
        float maO = settings.maxOld.value;
        float miN = settings.minNew.value;
        float maN = settings.maxNew.value;

        if (settings.preventInversion.value)
        {
            sheet.properties.SetFloat(MinOld, Mathf.Clamp(miO, 0, maO));
            sheet.properties.SetFloat(MaxOld, Mathf.Clamp(maO, miO, 1));
            sheet.properties.SetFloat(MinNew, Mathf.Clamp(miN, 0, maN));
            sheet.properties.SetFloat(MaxNew,Mathf.Clamp(maN, miN, 1));
        }
        else
        {
            sheet.properties.SetFloat(MinOld, miO);
            sheet.properties.SetFloat(MaxOld, maO);
            sheet.properties.SetFloat(MinNew, miN);
            sheet.properties.SetFloat(MaxNew, maN);
        }
        
        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
    }
}