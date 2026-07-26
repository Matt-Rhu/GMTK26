using System;
using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;

public class Goal : MonoBehaviour
{
    public static Goal instance;

    [SerializeField] private float ballSpeedReduction = 0.3f;
    
    [FoldHeader("Sounds")]
    [SerializeField] private SoundReference hoop;
    [SerializeField] private SoundReference miss;

    private void Awake()
    {
        instance = this;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Ball")) return;
        
        SendBallBack();

        var score = Ball.instance.BallScore;
        GameManager.instance.AddScore(score);

        if (score > 0)
            RuntimeManager.PlayOneShot(hoop.Ref);
        else
            RuntimeManager.PlayOneShot(miss.Ref);
    }

    private void SendBallBack()
    {
        Ball.instance.Pass(Ball.instance.transform.position + BounceDirection());
    }

    private Vector3 BounceDirection()
    {
        var newDir = Vector3.Reflect(Ball.instance.Velocity(), -Vector3.forward);
        return newDir * ballSpeedReduction;
    }
}
