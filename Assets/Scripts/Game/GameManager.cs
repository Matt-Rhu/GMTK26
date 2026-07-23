using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public bool TacticalPause { get; private set; }
    public float RemainingTime { get; private set; }
    public int PlayerScore { get; private set; }
    public int OpponentScore { get; private set; }


    public delegate void SimpleEvent();
    public static event SimpleEvent OnLose;
    public static event SimpleEvent OnWin;
    
    [Header("Game Settings")] 
    [SerializeField] private float allocatedTime = 30;
    [Space]
    [SerializeField] private int playerStartScore;
    [SerializeField] private int opponentStartScore;

    
    
    private void Awake()
    {
        instance = this;
        Inputs.Gameplay.ToggleTacticalPause.performed += _ => ToggleTacticalPause();
        
        RemainingTime = allocatedTime;
        PlayerScore = playerStartScore;
        OpponentScore = opponentStartScore;
    }

    private void Update()
    {
        if (TacticalPause) return;
        RemainingTime -= Time.deltaTime;
        RemainingTime = Mathf.Clamp(RemainingTime, 0, Mathf.Infinity);
        if (RemainingTime <= 0)
            Lose();
    }

    public void AddScore(int amount)
    {
        PlayerScore += amount;
        if (PlayerScore > OpponentScore)
            Win();
    }

    public void ToggleTacticalPause()
    {
        TacticalPause = !TacticalPause;
    }
    
    private void Win()
    { 
        OnWin?.Invoke();  
    }

    public void Lose()
    {
        OnLose?.Invoke();
    }
}
