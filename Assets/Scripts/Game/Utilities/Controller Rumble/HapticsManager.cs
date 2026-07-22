using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

public class HapticsManager : MonoBehaviour
{
    public static HapticsManager instance;
    
    private Gamepad pad;
    
    private Coroutine CurrentRumbleCoroutine;
    
    public bool RumbleCoroutineRunning;
    public float ElapsedTime;
    private bool canRumble = true;
    
    [System.Serializable]
    public struct HapticsStruct
    {
        public float highFrequency;
        public float lowFrequency;
        public float delay;
        public float duration;
    }
    
    public List<HapticsStruct> hapticsList = new List<HapticsStruct>();

    public Vector2 baseHaptics = Vector2.zero;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void EventRumblePulse(HapticsOptions haptics)
    {
        if (ControllerDetection.instance.ControlType != ControlType.Controller)
        {
            //Debug.Log("cancelled haptics due to wrong control type");
            return;
        }
        
        HapticsStruct currentHaptics = new HapticsStruct();
        currentHaptics.highFrequency = haptics.highFrequency;
        currentHaptics.lowFrequency = haptics.lowFrequency;
        currentHaptics.delay = haptics.delay;
        currentHaptics.duration = haptics.duration;
        ManagePusle(currentHaptics);
    }
    
    public void RumblePulse(HapticsOptions haptics, float variableDuration = 0)
    {
        if (ControllerDetection.instance.ControlType != ControlType.Controller)
        {
            //Debug.Log("cancelled haptics due to wrong control type");
            return;
        }
        
        HapticsStruct currentHaptics = new HapticsStruct();
        currentHaptics.highFrequency = haptics.highFrequency;
        currentHaptics.lowFrequency = haptics.lowFrequency;
        currentHaptics.delay = haptics.delay;
        if (variableDuration == 0) currentHaptics.duration = haptics.duration;
        else currentHaptics.duration = variableDuration;
        ManagePusle(currentHaptics);
    }
    
    public void RumblePulse(float highFrequency, float lowFrequency, float delay, float duration)
    {
        if (ControllerDetection.instance.ControlType != ControlType.Controller) return;
        
        HapticsStruct currentHaptics = new HapticsStruct();
        currentHaptics.highFrequency = highFrequency;
        currentHaptics.lowFrequency = lowFrequency;
        currentHaptics.delay = delay;
        currentHaptics.duration = duration;
        ManagePusle(currentHaptics);
    }


    public void ManagePusle(HapticsStruct haptics)
    {
        if (!canRumble) return;
        
        if (haptics.delay != 0)
        {
            StartCoroutine(DelayRumble(haptics));
            return;
        }
        
        pad = Gamepad.current;


        if (pad != null)
        {
            //Debug.Log("rumble added for " + haptics.duration + " seconds");

            //if(RumbleCoroutineRunning && CurrentRumbleCoroutine == null) Debug.LogError("motherfucker its happening");

            if (!RumbleCoroutineRunning)
            {
                //Debug.Log("starting rumble");
                hapticsList.Add(haptics);
                ElapsedTime = 0;
                RumbleCoroutineRunning = true;
                pad.SetMotorSpeeds(haptics.lowFrequency, haptics.highFrequency);
                //Debug.Log("SMS1 :" + haptics.lowFrequency + "-" + haptics.highFrequency);
                //Debug.Log(haptics);

                CurrentRumbleCoroutine = StartCoroutine(ManageRumble(haptics.duration, pad));
            }
            else
            {
                //setting duration right with timing at which it arrived in list
                haptics.duration += ElapsedTime;
                
                //case where new haptics takes the lead
                if (hapticsList[0].highFrequency < haptics.highFrequency)
                {
                    pad.SetMotorSpeeds(haptics.lowFrequency, haptics.highFrequency);
                    //Debug.Log("SMS2 :" + haptics.highFrequency + "-" + haptics.lowFrequency);
                    if(CurrentRumbleCoroutine != null) StopCoroutine(CurrentRumbleCoroutine);
                    CurrentRumbleCoroutine = StartCoroutine(ManageRumble(haptics.duration, pad));
                }
                
                bool hapticsInserted = false;
                //inserting new haptics in list if need be
                for (int i = 0; i < hapticsList.Count; i++)
                {
                    if (haptics.highFrequency > hapticsList[i].highFrequency)
                    {
                        //hapticsList.RemoveAt(hapticsList.Count - 1);
                        hapticsList.Insert(i, haptics);
                        hapticsInserted = true;
                        break;
                    }
                }
                //case where new haptics falls at end of list
                if(!hapticsInserted)hapticsList.Add(haptics);
            }
        }
    }

    private IEnumerator DelayRumble(HapticsStruct haptics)
    {
        float timer = 0;
        while (timer < haptics.delay)
        {
            timer += Time.unscaledDeltaTime;
            yield return null;
        }
        haptics.delay = 0;
        ManagePusle(haptics);
        yield return null;
    }

    
    private IEnumerator ManageRumble(float duration, Gamepad pad)
    {
        while (ElapsedTime < duration)
        {
            ElapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }
        //Debug.Log("next rumble");
        CheckNextRumble();
    }

    private void CheckNextRumble()
    {
        if(hapticsList.Count > 0) hapticsList.RemoveAt(0);
        else
        {
            Debug.LogError("what the scallops this shouldn't be happening, tell me if this results in a bug");
            return;
        }
        if (hapticsList.Count == 0)
        {
            pad.SetMotorSpeeds(baseHaptics.x, baseHaptics.y);
            //Debug.Log("SMS3 :" + baseHaptics.x + "-" + baseHaptics.y);

            RumbleCoroutineRunning = false;
            ElapsedTime = 0;
            //Debug.Log("rumbling over");
            return;
        }
        else if(pad != null)
        {
            //Debug.Log("setting next rumble for " + (ElapsedTime - hapticsList[0].duration) + " seconds");
            pad.SetMotorSpeeds(hapticsList[0].lowFrequency, hapticsList[0].highFrequency);
            //Debug.Log("SMS4 :" + hapticsList[0].lowFrequency + "-" + hapticsList[0].highFrequency);

            CurrentRumbleCoroutine = StartCoroutine(ManageRumble(hapticsList[0].duration, pad));
        }

    }

    public void SetBaseHaptics(HapticsOptions haptics = null)
    {
        if (ControllerDetection.instance.ControlType != ControlType.Controller) return;
        
        if (haptics != null) baseHaptics = new Vector2(haptics.lowFrequency, haptics.highFrequency);
        else baseHaptics = Vector2.zero;
        if (hapticsList.Count == 0)
        {
            pad = Gamepad.current;
            //pad?.SetMotorSpeeds(baseHaptics.x, baseHaptics.y);
            //Debug.Log("SMS5 :" + baseHaptics.x + "-" + baseHaptics.y);

        }
    }
    
    public void StopHaptics(float delay = 0, bool deactivateRumble = false, float delayAfterWhichReactivateRumble = 0)
    {
        StartCoroutine(Stop(delay));
        if (deactivateRumble)
        {
            canRumble = false;
            //Debug.Log("stop rumble");
            //if (delayAfterWhichReactivateRumble != 0) StartCoroutine(ReactivateRumbleRoutine(delayAfterWhichReactivateRumble));
        }
    }

    private IEnumerator Stop(float delay)
    {
        if(delay != 0) yield return new WaitForSecondsRealtime(delay);
        hapticsList.Clear();
        baseHaptics = Vector2.zero;
        if(CurrentRumbleCoroutine != null) StopCoroutine(CurrentRumbleCoroutine);
        RumbleCoroutineRunning = false;
        ElapsedTime = 0;
        pad = Gamepad.current;
        pad?.SetMotorSpeeds(0, 0);
        //Debug.Log("SMS6 :" + 0 + "-" + 0);
    }

    private IEnumerator ReactivateRumbleRoutine(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        ReactivateRumble();
    }

    public void ReactivateRumble()
    {
        //Debug.Log("can rumble again");
        ElapsedTime = 0;
        canRumble = true;
    }

    private void OnApplicationQuit()
    {
        StopHaptics();
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus) StopHaptics();
    }
}
