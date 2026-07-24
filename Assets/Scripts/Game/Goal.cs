using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Ball")) return;
        
        GameManager.instance.AddScore(Ball.instance.BallScore);
    }
}
