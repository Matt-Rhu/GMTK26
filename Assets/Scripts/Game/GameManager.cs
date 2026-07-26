using System;
using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public bool GameStarted { get; private set; }
    public bool TacticalPause { get; private set; }
    private bool outOfTime;
    public bool OutOfTime
    {
        get => outOfTime;
        private set
        {
            if (outOfTime != value && value)
                OnTimerEnd();
            outOfTime = value;
        }
    }
    
    public bool GameOver { get; private set; }
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

    [FoldHeader("Sounds")] 
    [SerializeField] private SoundReference whistle;
    [SerializeField] private SoundReference buzzer;
    [SerializeField] private SoundReference crowd;
    [SerializeField] private SoundReference music;

    public delegate void SimpleEvent();
    public event SimpleEvent OnLose;
    public event SimpleEvent OnWin;
    
    public delegate void TacticalPauseEvent(bool pauseOn);
    public event TacticalPauseEvent OnTacticalPause;
    
    private bool canReload;
    private float startGameTimer = 0f;
    
    private void Awake()
    {
        startGameTimer = zoomInDuration + timeBeforeGameStartAfterZoom;
        instance = this;
        Inputs.Gameplay.ToggleTacticalPause.performed += _ => ToggleTacticalPause();
        Inputs.Gameplay.Reload.performed += _ => ReloadScene();
        
        Time.timeScale = 1;
    }

    private void Start()
    {
        StartLevel();
    }

    private void StartLevel()
    {
        GameStarted = false;
        GameOver = false;
        mainCamera.EnterLevelZoom(zoomInDuration);
        StartCoroutine(StartDelay());
        SoundManager.instance.StartInstance(crowd);
    }

    private void Update()
    {
        // To not do anything before the game start.
        if (!GameStarted) return;

        if (TacticalPause) return;
        RemainingTime -= Time.deltaTime;
        RemainingTime = Mathf.Clamp(RemainingTime, 0, Mathf.Infinity);
        
        
        if (!Ball.instance) return;
        if (RemainingTime <= 0)
        {
            OutOfTime = true;

            if (Ball.instance.CurrentState == Ball.BallState.Shot) return;
            CheckScore();
        }
    }

    private IEnumerator StartDelay()
    {
        yield return new WaitForSeconds(startGameTimer);
        
        GameStarted = true;
        RuntimeManager.PlayOneShot(whistle.Ref);
        SoundManager.instance.StartInstance(music);
        SoundManager.instance.SetInstanceParameter(music, 0);
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
            SoundManager.instance.SetInstanceParameter(music, 1);
    }

    public void ToggleTacticalPause()
    {
        if (!GameStarted) return;

        if (canReload) return;
        
        TacticalPause = !TacticalPause;
        OnTacticalPause?.Invoke(TacticalPause);
    }

    private void OnTimerEnd()
    {
        RuntimeManager.PlayOneShot(buzzer.Ref);
        SoundManager.instance.StopInstance(music);
    }

    private void CheckScore()
    {
        if (PlayerScore > OpponentScore)
            Win();
        else
            Lose();
    }
    
    public void Win()
    {
        GameOver = true;
        canReload = true;
        OnWin?.Invoke(); 
        
        SoundManager.instance.SetInstanceParameter(crowd, 1);
    }

    public void Lose()
    {
        GameOver = true;
        canReload = true;
        OnLose?.Invoke();
        
        SoundManager.instance.StopInstance(crowd);
    }

    private void ReloadScene()
    {
        if (!canReload) return;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
