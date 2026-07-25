using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
    public static Goal instance;

    [SerializeField] private float ballSpeedReduction = 0.3f;

    private void Awake()
    {
        instance = this;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Ball")) return;
        
        SendBallBack();
        GameManager.instance.AddScore(Ball.instance.BallScore);
    }

    private void SendBallBack()
    {
        var newDir = Vector3.Reflect(Ball.instance.Velocity(), -Vector3.forward);
        Ball.instance.Pass(Ball.instance.transform.position + newDir * ballSpeedReduction);
    }
}
