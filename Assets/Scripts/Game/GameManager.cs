using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public bool GameStarted { get; private set; }
    public bool TacticalPause { get; private set; }
    public bool OutOfTime { get; private set; }
    public float RemainingTime { get; private set; }
    public int PlayerScore { get; private set; }
    public int OpponentScore { get; private set; }

    public int InZoneScore { get; private set; }

    public int OutZoneScore { get; private set; }

    // References
    public MainCamera mainCamera;

    [Header("Settings")]
    [SerializeField] private float zoomInDuration = 0f;
    [SerializeField] private float timeBeforeGameStartAfterZoom = 1f;
    [SerializeField] private bool startWithTutorial = false;

    public delegate void SimpleEvent();
    public event SimpleEvent OnLose;
    public event SimpleEvent OnWin;
    
    public delegate void TacticalPauseEvent(bool pauseOn);
    public event TacticalPauseEvent OnTacticalPause;


    private bool canReload;
    private float startGameTimer = 0f;
    
    private void Awake()
    {
        GameStarted = false;
        startGameTimer = zoomInDuration + timeBeforeGameStartAfterZoom;
        instance = this;
        Inputs.Gameplay.ToggleTacticalPause.performed += _ => ToggleTacticalPause();
        Inputs.Gameplay.Reload.performed += _ => ReloadScene();
        
        Time.timeScale = 1;
    }

    private void Start()
    {
        mainCamera.EnterLevelZoom(zoomInDuration);
    }

    private void Update()
    {
        // To not do anything before the game start.
        if (processIdleBeforeGameStarted()) return;

        if (TacticalPause) return;
        RemainingTime -= Time.deltaTime;
        RemainingTime = Mathf.Clamp(RemainingTime, 0, Mathf.Infinity);
        if (RemainingTime <= 0)
        {
            OutOfTime = true;
            // If Time is over, you do not as long as the ball is in shot (can still win).
            if (Ball.instance.CurrentState != Ball.BallState.Shot)
            {
                Lose();
            }
        }
    }

    private bool processIdleBeforeGameStarted()
    {
        if (startGameTimer > 0f)
        {
            GameStarted = false;
            startGameTimer -= Time.deltaTime;
        } else
        {
            GameStarted = true;
        }
        return !GameStarted;
        
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
        if (!GameStarted) return;

        if (canReload) return;
        
        TacticalPause = !TacticalPause;
        OnTacticalPause?.Invoke(TacticalPause);
    }
    
    public void Win()
    { 
        canReload = true;
        OnWin?.Invoke();  
    }

    public void Lose()
    {
        canReload = true;
        OnLose?.Invoke();
    }

    private void ReloadScene()
    {
        if (!canReload) return;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
