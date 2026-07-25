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

    public int InZoneScore { get; private set; }

    public int OutZoneScore { get; private set; }


    public delegate void SimpleEvent();
    public event SimpleEvent OnLose;
    public event SimpleEvent OnWin;
    
    public delegate void TacticalPauseEvent(bool pauseOn);
    public event TacticalPauseEvent OnTacticalPause;


    private bool canReload;
    
    
    private void Awake()
    {
        instance = this;
        Inputs.Gameplay.ToggleTacticalPause.performed += _ => ToggleTacticalPause();
        Inputs.Gameplay.Reload.performed += _ => ReloadScene();
        
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


    public void InitializeFromStartPositionDatas(float remainingTime, int playerStartScore, int opponentStartScore, int inZoneScore, int outZoneScore)
    {
        RemainingTime = remainingTime;
        PlayerScore = playerStartScore;
        OpponentScore = opponentStartScore;
        InZoneScore = inZoneScore;
        OutZoneScore = outZoneScore;
    }

    public void AddScore(int amount)
    {
        PlayerScore += amount;
        if (PlayerScore > OpponentScore)
            Win();
    }

    public void ToggleTacticalPause()
    {
        if (canReload) return;
        
        TacticalPause = !TacticalPause;
        OnTacticalPause?.Invoke(TacticalPause);
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
