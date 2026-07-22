using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FMODUnity;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public abstract class UIAnimSequencePlayer : UIAnimations
{
    [SerializeField] private bool ignoreSkipsIfPaused;
    
    protected Dictionary<int, OngoingSequence> ongoingSequences = new Dictionary<int, OngoingSequence>();

    private IEnumerator Animate(AnimSequence seq, Action callWhenFinished = null, int overlapIndex = 0)
    {
        if (seq.globalDurationMod <= 0)
        {
            seq.globalDurationMod = 1f;
            Debug.LogWarning("Duration modifier was set to 0. Changed it to 1 instead");
        }

        //play anims in the sequence | due to new system, starting them all at once so they're all skippable
        float totalDelay = seq.delayBefore;
        foreach (var a in seq.anims)
        {
            foreach (var i in PlayAnim(a, seq.appearing, totalDelay, seq.globalDurationMod, seq.globalDelayMod)) 
                ongoingSequences[overlapIndex].animIndices.Add(i);
            totalDelay += a.timeUntilNext * seq.globalDelayMod;
        }

        yield return Wait(totalDelay + seq.anims[^1].duration);
        callWhenFinished?.Invoke();
        
        ongoingSequences.Remove(overlapIndex);
        if (seq.sound) SoundManager.instance.StopInstance(seq.sound);
    }
    
    protected void StartSequence(AnimSequence seq, Action callWhenFinished = null, int overlapIndex = 0)
    {
        if (!gameObject.activeSelf) return;
        if (ongoingSequences.ContainsKey(overlapIndex)) return;
            
        OngoingSequence newSeq = new OngoingSequence
        {
            callWhenFinished = callWhenFinished,
            animIndices = new List<int>(),
        };

        if (seq.sound)
        {
            SoundManager.instance.StartInstance(seq.sound);
            newSeq.sound = seq.sound.name;
        }
        
        ongoingSequences.Add(overlapIndex, newSeq);
        ongoingSequences[overlapIndex].coroutine = StartCoroutine(Animate(seq, callWhenFinished, overlapIndex));
        StartCoroutine(SkipDelay(seq, overlapIndex));
    }
    
    
    public void SkipSequence(InputAction.CallbackContext ctx = new())
    {
        if(ongoingSequences.Count == 0) return;
        //if(ignoreSkipsIfPaused && GameManager.instance.PauseOrFreecam()) return;

        bool stoppedSound = false;
        List<int> toRemove = new List<int>();
        foreach (var oseq in ongoingSequences)
        {
            if (!oseq.Value.canSkip) continue;

            StopCoroutine(oseq.Value.coroutine);
            SkipAnim(oseq.Value.animIndices.ToArray());
            if (oseq.Value.sound != null)
            {
                SoundManager.instance.StopInstance(oseq.Value.sound);
                stoppedSound = true;
            }
            toRemove.Add(oseq.Key);
        }
        StartCoroutine(FinishSkip(toRemove));
        if (stoppedSound) RuntimeManager.PlayOneShot(SoundManager.instance.animSkipSound.Ref);
    }

    private IEnumerator FinishSkip(List<int> toRemove)
    {
        yield return new WaitForEndOfFrame();
        
        foreach (int key in toRemove)
        {
            ongoingSequences[key].callWhenFinished?.Invoke();
            ongoingSequences.Remove(key);
        }
    }
    
    private IEnumerator SkipDelay(AnimSequence seq, int overlapIndex)
    {
        ongoingSequences[overlapIndex].canSkip = false;
        
        if (!seq.allowSkip) yield break;

        yield return Wait(seq.timeBeforeCanSkip + seq.delayBefore);
        
        if (!ongoingSequences.TryGetValue(overlapIndex, out var sequence)) yield break;
        sequence.canSkip = true;
    }

    [Serializable]
    public class AnimForSequence : Anim
    {
        [Tooltip("Starts from beginning of anim, not end.")] public float timeUntilNext;
    }
    
    [Serializable]
    public class OngoingSequence
    {
        public Coroutine coroutine;
        public Action callWhenFinished;
        public bool canSkip;
        public List<int> animIndices;
        public string sound;
    }

    [Serializable]
    public class AnimSequence
    {
        public string name;
        [Tooltip("Multiplies the duration of all the animations that have one")] public float globalDurationMod = 1;
        [Tooltip("Multiplies the delay between all the animations")] public float globalDelayMod = 1;
        public float delayBefore = 0;
        public bool appearing;
        public bool allowSkip = true;
        [HideWithValue("allowSkip")] public float timeBeforeCanSkip = 0.15f;
        public SoundReference sound;
        public AnimForSequence[] anims;
    }
}