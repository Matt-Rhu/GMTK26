using UnityEngine;
using UnityEngine.Events;
using FMODUnity;

public class CallOnEnable : MonoBehaviour
{
    public bool onlyOnce;
    private bool didOnce;
    public UnityEvent callOnEnable;
    public UnityEvent callOnDisable;
    public SoundReference soundOnEnable;
    public SoundReference soundOnDisable;


    private void OnEnable()
    {
        if (onlyOnce)
        {
            if (didOnce) return;
            if (soundOnEnable) RuntimeManager.PlayOneShot(soundOnEnable.Ref);
            callOnEnable.Invoke();
            didOnce = true;
        }
        else
        {
            if (soundOnEnable) RuntimeManager.PlayOneShot(soundOnEnable.Ref);
            callOnEnable.Invoke();
        }
    }

    private void OnDisable()
    {
        if (onlyOnce)
        {
            if (didOnce) return;
            if (soundOnDisable) RuntimeManager.PlayOneShot(soundOnDisable.Ref);
            callOnDisable.Invoke();
            didOnce = true;
        }
        else
        {
            if (soundOnDisable) RuntimeManager.PlayOneShot(soundOnDisable.Ref);
            callOnDisable.Invoke();
        }
    }
}