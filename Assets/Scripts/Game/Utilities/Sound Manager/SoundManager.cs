using System;
using System.Collections.Generic;
using FMOD;
using FMOD.Studio;
using FMODUnity;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;
using STOP_MODE = FMOD.Studio.STOP_MODE;

public class SoundManager : DontDestroySingleton<SoundManager>
{
    private Dictionary<string, EventInstance> sounds = new();
    [SerializeField] private List<string> instances = new();

    [SerializeField] private SoundOnStart[] soundsOnSceneLoad;
    
    private int instanceIndex;

    [Header("Global Sound Refs")] 
    public SoundReference animSkipSound;
    public SoundReference hoverOrSelectSound, pressSound;

    //If we end up having a lot of sounds and levels and it gets annoying to handle them all one by one, maybe consider a tag system, with like all the environment sounds and stuff

    protected override void Awake()
    {
        base.Awake();

        // SetGlobalParameter("var_sfx", Save.General.SFXSoundSlider);
        // SetGlobalParameter("var_music", Save.General.musicSoundSlider);
        // SetGlobalParameter("var_general", Save.General.generalSoundSlider);
    }

    private void Start()
    {
        SceneManager.sceneLoaded += OnStart;
        OnStart(SceneManager.GetActiveScene(), 0);
    }

    private void OnStart(Scene s, LoadSceneMode l)
    {
        if (l == LoadSceneMode.Single) return;
        
        foreach (var sos in soundsOnSceneLoad)
            foreach (var sc in sos.scenes)
                if (sc)
                    StartInstance(sos.sound);
    }

    public void StartInstance(string instanceName, string path, Transform t = null)
    {
        if (sounds.ContainsKey(instanceName)) return;
        
        EventInstance i = RuntimeManager.CreateInstance(path);

        if (t)
            RuntimeManager.AttachInstanceToGameObject(i,t);
        i.start();
        
        sounds.Add(instanceName, i);
        instances.Add(instanceName);
        i.release();
    }
    public void StartInstance(string instanceName, EventReference reference, Transform t = null)
    {
        if (sounds.ContainsKey(instanceName)) return;
        
        EventInstance i = RuntimeManager.CreateInstance(reference);

        if (t)
            RuntimeManager.AttachInstanceToGameObject(i,t);
        i.start();

        sounds.Add(instanceName, i);
        instances.Add(instanceName);
        i.release();

        
    }
    public void StartInstance(SoundReference s, Transform t = null)
    {
        if (sounds.ContainsKey(s.name)) return;
        
        EventInstance i = RuntimeManager.CreateInstance(s.Ref);

        if (t)
            RuntimeManager.AttachInstanceToGameObject(i,t);
        i.start();
        
        sounds.Add(s.name, i);
        instances.Add(s.name);
        i.release();
    }

    public string StartInstanceAdditive(SoundReference s, Transform t = null)
    {
        string instanceName = s.name;
        if (sounds.ContainsKey(instanceName))
        {
            instanceName += (" " + instanceIndex);
            instanceIndex++;
        }

        EventInstance i = RuntimeManager.CreateInstance(s.Ref);

        if (t)
            RuntimeManager.AttachInstanceToGameObject(i,t);
        i.start();

        sounds.Add(instanceName, i);
        instances.Add(instanceName);
        i.release();

        return instanceName;
    }

    public void PlayOneShotWithParameter(string path, string parameterName, float value, Transform t = null)
    {
        EventInstance eventInstance = RuntimeManager.CreateInstance(path);
        eventInstance.setParameterByName(parameterName, value);
        if (t) eventInstance.set3DAttributes(t.To3DAttributes());
        eventInstance.start();
        eventInstance.release();
    }
    public void PlayOneShotWithParameter(SoundReference s, int paramIndex, float value, Transform t = null)
    {
        EventInstance eventInstance = RuntimeManager.CreateInstance(s.Ref);
        eventInstance.setParameterByName(s.parameters[paramIndex], value);
        if (t) eventInstance.set3DAttributes(t.To3DAttributes());
        eventInstance.start();
        eventInstance.release();
    }
    public void PlayOneShotWithParameter(SoundReference s, float value, Transform t = null)
    {
        EventInstance eventInstance = RuntimeManager.CreateInstance(s.Ref);
        eventInstance.setParameterByName(s.parameters[0], value);
        if (t) eventInstance.set3DAttributes(t.To3DAttributes());
        eventInstance.start();
        eventInstance.release();
    }

    public void PlayOneShotWithParameters(SoundReference s, float[] parameterValues, Transform t = null)
    {
        EventInstance eventInstance = RuntimeManager.CreateInstance(s.Ref);
        
        for (var i = 0; i < s.parameters.Length; i++)
        {
            if (i >= parameterValues.Length) break;
            eventInstance.setParameterByName(s.parameters[i], parameterValues[i]);
        }

        if (t) eventInstance.set3DAttributes(t.To3DAttributes());
        
        eventInstance.start();
        eventInstance.release();
    }

    public void SetInstanceParameter(string instanceName, string parameterName, float value)
    {
        if (sounds.ContainsKey(instanceName))
            sounds[instanceName].setParameterByName(parameterName, value);
    }
    public void SetInstanceParameter(SoundReference s, float value)
    {
        if (sounds.ContainsKey(s.name))
            sounds[s.name].setParameterByName(s.parameters[0], value);
    }
    public void SetInstanceParameter(SoundReference s, int paramIndex, float value)
    {
        if (sounds.ContainsKey(s.name))
            sounds[s.name].setParameterByName(s.parameters[paramIndex], value);
    }

    public void SetGlobalParameter(string parameterName, float value)
    {
        RuntimeManager.StudioSystem.setParameterByName(parameterName, value);
    }
    
    //No need for that yet, but will probably be useful when we have different levels with different ambiances etc
    public void ReplaceInstance(string instanceName, string path, Transform t = null, bool allowFadeout = true)
    {
        if (sounds.ContainsKey(instanceName))
            StopInstance(instanceName, allowFadeout);
        
        StartInstance(instanceName, path, t);
    }
    public void ReplaceInstance(SoundReference s, Transform t = null, bool allowFadeout = true)
    {
        if (sounds.ContainsKey(s.name))
            StopInstance(s.name, allowFadeout);
        
        StartInstance(s, t); 
    }
    
    public void StopInstance(string instanceName, bool allowFadeout = true)
    {
        if (!sounds.ContainsKey(instanceName)) return;
        
        if (allowFadeout)
            sounds[instanceName].stop(STOP_MODE.ALLOWFADEOUT);
        else 
            sounds[instanceName].stop(STOP_MODE.IMMEDIATE);
            
        sounds.Remove(instanceName);
        instances.Remove(instanceName);
    }
    public void StopInstance(SoundReference reference, bool allowFadeout = true)
    {
        StopInstance(reference.name, allowFadeout);
    }

    public void StopAll(bool allowFadeout = true)
    {
        List<string> toStop = new();
        
        foreach (var v in sounds)
            toStop.Add(v.Key);
        
        foreach (string s in toStop)
            StopInstance(s, allowFadeout);
    }
    
    public void StopAllExcept(string[] soundsToKeep, bool allowFadeout = true)
    {
        List<string> toStop = new();
        
        foreach (var v in sounds)
        {
            bool doNotStop = false;
            foreach (string s in soundsToKeep)
            {
                if (s == v.Key) doNotStop = true;
                break;
            }

            if (!doNotStop) toStop.Add(v.Key);
        }
        
        foreach (string s in toStop)
            StopInstance(s, allowFadeout);
    }
    public void StopAllExcept(SoundReference[] soundsToKeep, bool allowFadeout = true)
    {
        List<string> toStop = new();
        
        foreach (var v in sounds)
        {
            bool doNotStop = false;
            foreach (SoundReference s in soundsToKeep)
            {
                if (s.name == v.Key) doNotStop = true;
                break;
            }

            if (!doNotStop) toStop.Add(v.Key);
        }
        
        foreach (string s in toStop)
            StopInstance(s, allowFadeout);
    }
    public void StopAllExcept(string soundToKeep, bool allowFadeout = true)
    {
        List<string> toStop = new();
        
        foreach (var v in sounds)
            if (soundToKeep != v.Key) 
                toStop.Add(v.Key);

        foreach (string s in toStop)
            StopInstance(s, allowFadeout);
    }
    

    public bool HasInstance(string instanceName)
    {
        return sounds.ContainsKey(instanceName);
    }


    [Serializable]
    public struct SoundOnStart
    {
        public SceneReference[] scenes;
        public SoundReference sound;
    }
}