using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public bool TacticalPause { get; private set; }
    public bool OutOfTime { get; private set; }
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
    [Space] 
    public int inZoneScore = 2;
    public int outZoneScore = 3;


    private bool canReload;
    
    
    private void Awake()
    {
        instance = this;
        Inputs.Gameplay.ToggleTacticalPause.performed += _ => ToggleTacticalPause();
        Inputs.Gameplay.Reload.performed += _ => ReloadScene();
        
        RemainingTime = allocatedTime;
        PlayerScore = playerStartScore;
        OpponentScore = opponentStartScore;
        
        Time.timeScale = 1;
    }

    private void Update()
    {
        if (TacticalPause) return;
        RemainingTime -= Time.deltaTime;
        RemainingTime = Mathf.Clamp(RemainingTime, 0, Mathf.Infinity);
        if (RemainingTime <= 0)
        {
            OutOfTime = true;
            Lose();
            //TODO: will need to cleanup the timeout so units and the player can't act when it hits 0, but it's not a defeat until the ball stops moving
        }
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
    
    public void Win()
    { 
        canReload = true;
        OnWin?.Invoke();  
        Time.timeScale = 0;
    }

    public void Lose()
    {
        canReload = true;
        OnLose?.Invoke();
        Time.timeScale = 0;
    }

    private void ReloadScene()
    {
        if (!canReload) return;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
