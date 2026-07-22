using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public abstract class UIAnimations : MonoBehaviour
{
    [SerializeField] private bool unscaledTimeAnims = true;
    
    protected readonly Dictionary<int, OngoingAnim> ongoingAnims = new Dictionary<int, OngoingAnim>();
    private int animID = 0;

    #region CallAnimation

    //needs to return an array in case anim is a batch anim
    public int[] PlayAnim(Anim a, bool on, float delayBefore = 0, float durationMod = 1, float batchDelayMod = 1)
    {
        if (!gameObject.activeSelf) return null;
        
        if (a.batchAnim) 
            return PlayBatchAnim(a, on, delayBefore, durationMod, batchDelayMod);
        else 
            return new[] { PlaySingleAnim(a, on, a.duration * durationMod, delayBefore) };
    }
    
    // ReSharper disable Unity.PerformanceAnalysis
    private int PlaySingleAnim(Anim a, bool on, float dur, float delay, GameObject go = null)
    {
        OngoingAnim oa = new OngoingAnim
        {
            anim = a,
            on = on,
            go = go??a.go
        };
        animID++;

        if (!go) go = a.go;
        
        if (dur <= 0 && a.animType != AnimType.Instant && !a.batchAnim)
        {
            dur = 1f;
            Debug.LogWarning("Duration or multiplier for " + go.name + "'s animation was set to 0. Changed duration to 1 instead");
        }
        
        switch (a.animType)
        {
            case AnimType.Instant:
                oa.coroutine = StartCoroutine(Instant(animID, go, on, delay));
                break;
            case AnimType.Flicker:
                oa.coroutine = StartCoroutine(Flicker(animID, go, a.nbFlickers, dur, on, delay));
                break;
            case AnimType.ShaderAnim:
                oa.coroutine = StartCoroutine(ShaderAnim(animID, go, dur, a.shaderVarName, on, delay));
                break;
            case AnimType.Translate:
                oa.coroutine = StartCoroutine(Translate(animID, go, dur, a.offPosition, on, a.curve, delay));
                oa.originalPos = go.transform.localPosition;
                break;
            case AnimType.Fade:
                if (!go.TryGetComponent(out Graphic img))
                    if (go.GetComponentInChildren<Graphic>()) img = go.GetComponentInChildren<Graphic>(); 
                if (img == null)
                {
                    Debug.LogWarning(go.name + " has no component of type Graphic for Fade anim. Doing Instant instead.");
                    oa.coroutine = StartCoroutine(Instant(animID, go, on, delay));
                    break;
                }
                oa.coroutine = StartCoroutine(Fade(animID, img, go,dur, a.targetAlpha, on, delay));
                oa.originalAlpha = a.targetAlpha;
                break;
            case AnimType.Flash:
                if (go.TryGetComponent(out Graphic gr))
                {
                    oa.coroutine = StartCoroutine(Flash(animID, gr, dur, a.highlightColour, a.curve, on, delay));
                    oa.originalColour = gr.color;
                }
                else
                {
                    Debug.LogWarning(go.name + " has no component of type Graphic for Flash anim. Doing Instant instead.");
                    oa.coroutine = StartCoroutine(Instant(animID, go, on, delay));
                }
                break;
        }
        
        ongoingAnims.Add(animID, oa);
        return animID;
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private int[] PlayBatchAnim(Anim batch, bool on, float delayBefore, float durationMod = 1, float delayMod = 1)
    {
        if (batch.animType is AnimType.Translate)
        {
            Debug.LogWarning(name + ": batch animation isn't compatible with Translate anim type. Type for " + batch.go.name + " set to Instant instead.");
            batch.animType = AnimType.Instant;
        }
        
        batch.go.SetActive(true);
        
        Transform parent = batch.go.transform;
        int count = parent.childCount;
        int[] c = new int[count];
        
        if (batch.firstObjectDelayMod == 0) batch.firstObjectDelayMod = 1;
        if (batch.lastObjectDelayMod == 0) batch.lastObjectDelayMod = 1;
    
        float totalDelay = delayBefore;
        for (int i = 0; i < count; i++)
        {
            GameObject g;
            if (batch.childrenOrderAscending)
                g = parent.GetChild(i).gameObject;
            else
                g = parent.GetChild(count - 1 - i).gameObject;

            if ((g.activeSelf && on) || (!g.activeSelf && !on)) continue;
            
            c.SetValue(PlaySingleAnim(batch, on, batch.durationPerChild * durationMod, totalDelay, g), i); 
            
            float delay = batch.interval * delayMod;
            if (i == 0) delay *= batch.firstObjectDelayMod;
            else if (i == count - 2) delay *= batch.lastObjectDelayMod;
            totalDelay += delay;
        }
        
        return c;
    }

    #endregion

    protected float DeltaTime()
    {
        if (unscaledTimeAnims) 
            return Time.unscaledDeltaTime;
        return Time.deltaTime;
    }
    
    protected object Wait(float duration)
    {
        if (unscaledTimeAnims)
            return new WaitForSecondsRealtime(duration);
        return new WaitForSeconds(duration);
    }
    
    #region AnimationFunctions
    
    public IEnumerator Flash(int id, Graphic graph, float duration, Color highlightColour, AnimationCurve curve, bool on = true, float delay = 0)
    {
        yield return Wait(delay);
        
        graph.gameObject.SetActive(true);
        
        float t = 0;
        Color col = graph.color;
        
        while (t < 1)
        {
            t += DeltaTime() / duration;
            t = Mathf.Clamp01(t);
            if (!on) t = 1 - t;
            
            graph.color = Color.Lerp(col, highlightColour, curve.Evaluate(t));

            yield return null;
        }
        
        graph.gameObject.SetActive(on);
        ongoingAnims.Remove(id);
    }
    
    private IEnumerator Translate(int id, GameObject go, float duration, Vector2 offset, bool on, AnimationCurve curve, float delay = 0)
    {
        yield return Wait(delay);
        
        RectTransform tr = go.GetComponent<RectTransform>();

        Vector3 defaultPos = tr.localPosition;
        Vector3 start;
        Vector3 end;
        if (on)
        {
            end = defaultPos;
            start = new Vector3(offset.x, offset.y, defaultPos.z);
        }
        else
        {
            start = defaultPos;
            end = new Vector3(offset.x, offset.y, defaultPos.z);
        }

        go.SetActive(true);
        float t = 0;

        while (t < 1)
        {
            t += DeltaTime() / duration;
            t = Mathf.Clamp01(t);

            tr.localPosition = Vector3.Lerp(start, end, curve.Evaluate(t));

            yield return null;
        }
        
        go.SetActive(on);
        tr.localPosition = defaultPos;
        ongoingAnims.Remove(id);
    }

    private IEnumerator Fade(int id, Graphic img, GameObject go, float duration, float targetAlpha, bool on, float delay = 0)
    {
        yield return Wait(delay);

        float t = 0;
        float a;
        Color col = img.color; 
        var startA = col.a;
        
        img.gameObject.SetActive(true);
        go.SetActive(true);
        
        while (t < 1)
        {
            t += DeltaTime() / duration;
            t = Mathf.Clamp01(t);
            float easedT = Ease.OfType(t, Ease.Type.InOutCubic);

            if (on)
                a = Mathf.Lerp(0, targetAlpha, easedT);
            else
                a = Mathf.Lerp(startA, 0, easedT);

            img.color = new Color(col.r, col.g, col.b, a);

            yield return null;
        }
        
        go.SetActive(on);
        img.color = new Color(col.r, col.g, col.b, targetAlpha);
        ongoingAnims.Remove(id);
    }

    private IEnumerator ShaderAnim(int id, GameObject go, float duration, string timeVar, bool on, float delay = 0)
    {
        yield return Wait(delay);
        
        Graphic img;
        if (!go.TryGetComponent(out img))
        {
            img = go.GetComponentInChildren<Graphic>();
        }
        if (!img)
        {
            Debug.LogWarning("No Graphic component found on " + go.name + " for Shader animation");
            yield break;
        }
        //make an instance because sharedMaterial doesn't exist for Images
        var mat = Instantiate(img.material);
        img.material = mat;

        timeVar ??= "_T";

        go.SetActive(true);
        
        float t = 0;

        while (t < 1)
        {
            t += DeltaTime() / duration;
            t = Mathf.Clamp01(t);
            
            if (on)
                mat.SetFloat(timeVar, t);
            else
                mat.SetFloat(timeVar, 1 - t);
        
            yield return null;
        }
        
        go.SetActive(on);
        ongoingAnims.Remove(id);
    }
    
    private IEnumerator Flicker(int id, GameObject go, int nbFlicker, float duration, bool on, float delay = 0)
    {
        yield return Wait(delay);
        
        for (int i = 0; i < nbFlicker; i++)
        {
            go.SetActive(!go.activeSelf);
            yield return Wait(duration / nbFlicker);
        }

        go.SetActive(on);
        ongoingAnims.Remove(id);
    }
    
    private IEnumerator Instant(int id, GameObject go, bool on, float delay = 0)
    {
        yield return Wait(delay);
        
        go.SetActive(on);
        ongoingAnims.Remove(id);
    }
    
    #endregion

    #region CallSkip

    public void SkipAllAnims()
    {
        List<int> toSkip = new List<int>();
        //I always forget not to modify a collection while iterating through it!!
        foreach (var oa in ongoingAnims)
            toSkip.Add(oa.Key);

        foreach (var id in toSkip)
            SkipAnim(id);
    }

    public void SkipAnim(int[] ids)
    {
        foreach (var i in ids)
            SkipAnim(i);
    }
    
    public void SkipAnim(int id)
    {
        if (!gameObject.activeSelf) return;
        
        if (!ongoingAnims.Remove(id, out var oa)) return;

        StopCoroutine(oa.coroutine);
        Anim a = oa.anim;
        
        switch (oa.anim.animType)
        {
            //no need to do anything to Instant and Flicker, the SetActive below is enough
            case AnimType.ShaderAnim:
                SkipShaderAnim(oa.go, a.shaderVarName);
                break;
            case AnimType.Fade:
                SkipFadeAnim(oa.go, oa.originalAlpha);
                break;
            case AnimType.Translate:
                SkipTranslateAnim(oa.go, oa.originalPos);
                break;
            case AnimType.Flash:
                SkipFlashAnim(oa.go, oa.originalColour);
                break;
        }
        oa.go.SetActive(oa.on);
    }
    
    #endregion
    
    #region SkipFunctions

    protected void SkipFlashAnim(GameObject go, Color defaultColour)
    {
        if (!go.TryGetComponent(out Graphic img)) return;

        img.color = defaultColour;
    }

    protected void SkipShaderAnim(GameObject go, string timeVar)
    {
        //less in-depth checking and no instantiation because supposedly all that's already good if we're skipping an ongoing anim
        if (!go.TryGetComponent(out Graphic img)) return;
        Material mat = img.material;
        
        mat.SetFloat(timeVar, 1);
    }

    protected void SkipFadeAnim(GameObject go, float targetAlpha)
    {
        if (!go.TryGetComponent(out Graphic img)) return;
        Color col = img.color;
        img.color = new Color(col.r, col.g, col.b, targetAlpha);
    }

    protected void SkipTranslateAnim(GameObject go, Vector3 defaultPos)
    {
        go.transform.localPosition = defaultPos;
    }

    #endregion
    
    [Serializable]
    public class Anim
    {
        public GameObject go;
        public AnimType animType;
        [HideWithValue("animType", 0, true)] [HideWithValue("batchAnim", true)] public float duration;
        [HideWithValue("animType", 1, false)] [Range(2, 11)] public int nbFlickers;
        [HideWithValue("animType", 2, false)] public string shaderVarName;
        [HideWithValue("animType", 3, false)] public float targetAlpha;
        [FormerlySerializedAs("translateOffset")] [HideWithValue("animType", 4, false)] public Vector2 offPosition;
        [HideWithValue("animType", 5, false)] public Color highlightColour;
        [HideWithValue("animType", new []{4, 5, 6}, false)] public AnimationCurve curve;
        [Tooltip("Do selected animation for all children of given game object at an interval")] public bool batchAnim;
        [HideWithValue("animType", 0, true)] [HideWithValue("batchAnim")] public float durationPerChild;
        [HideWithValue("batchAnim")] public float interval;
        [HideWithValue("batchAnim")] public float firstObjectDelayMod;
        [HideWithValue("batchAnim")] public float lastObjectDelayMod;
        [HideWithValue("batchAnim")] public bool childrenOrderAscending;
    }

    protected struct OngoingAnim
    {
        public Coroutine coroutine;
        public Anim anim;
        public GameObject go;
        public bool on;
        
        public float originalAlpha;
        public Vector3 originalPos;
        public Color originalColour;
    }
}

public enum AnimType
{
    Instant = 0,
    Flicker,
    ShaderAnim,
    Fade,
    Translate,
    Flash, 
}