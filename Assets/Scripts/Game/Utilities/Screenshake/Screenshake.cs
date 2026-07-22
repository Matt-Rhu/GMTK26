using System;
using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

public class Screenshake : MonoBehaviour
{
    public static Screenshake instance;
    private Quaternion shake;
    
    [SerializeField] private Transform cam;

    [SerializeField] private float resetSpeed = 10;

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        ResetCamRotation();
    }

    private void ResetCamRotation(float speedModifier = 1f)
    {
        cam.localRotation = Quaternion.Lerp(cam.transform.localRotation, Quaternion.Euler(Vector3.zero), Time.unscaledDeltaTime * resetSpeed * speedModifier);
    }
    
    private IEnumerator Shake(float duration, float speed, float magnitude, AnimationCurve falloff)
    {
        float t = 0;

        float rndDuration = Random.Range(duration * 0.9f, duration * 1.1f);
        
        float speedX = Random.Range(speed * 0.9f, speed * 1.1f);
        float speedY = Random.Range(speed * 0.8f, speed * 1.1f);

        int signX = 1;
        int signY = 1;
        if (Random.Range(0, 2) == 0) signX = -1;
        if (Random.Range(0, 2) == 0) signY = -1;

        while (t < 1)
        {
            t += Time.unscaledDeltaTime / rndDuration;

            float shakeX = signX * Mathf.Lerp(-magnitude / 2, magnitude / 2, Mathf.PingPong(t * speedX * ((1 + (1 - falloff.Evaluate(t))) / 2), 1)) * falloff.Evaluate(t);
            float shakeY = signY * Mathf.Lerp(-magnitude / 2, magnitude / 2, Mathf.PingPong(t * speedY * ((1 + (1 - falloff.Evaluate(t))) / 2 * 1f), 1)) * falloff.Evaluate(t);

            shake = Quaternion.Euler(shakeX, shakeY, 0);

            Quaternion rot = cam.localRotation;
            rot = new Quaternion(rot.x + shake.x, rot.y + shake.y, rot.z, rot.w);
            cam.localRotation = rot;

            yield return null;
        }
    }
    
    public void CallShake(ScreenshakeData shaq)
    {
        // if (PlayerHealth.instance.Dead) return;
        
        float duration = shaq.duration;
        float speed = shaq.speed;
        float magnitude = shaq.magnitude;
        AnimationCurve falloff = shaq.falloff;
        
        StartCoroutine(Shake(duration, speed, magnitude, falloff));
    }
    public void CallShake(float duration, float speed, float magnitude, AnimationCurve falloff)
    {
        // if (PlayerHealth.instance.Dead) return;
        
        StartCoroutine(Shake(duration, speed, magnitude, falloff));
    }

    public void StopShake()
    {
        StopAllCoroutines();
    }
}
