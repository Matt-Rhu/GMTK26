using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WinAndLoseScreens : MonoBehaviour
{
    [SerializeField] private GameObject winScreen, loseScreen;
    [SerializeField] private TMP_Text winText;
    [SerializeField] private TMP_Text winScore;
    [SerializeField] private TMP_Text loseText;
    [SerializeField] private TMP_Text loseScore;
    public void Start()
    {
        GameManager.instance.OnLose += Lose;
        GameManager.instance.OnWin += Win;
    }
    
    private void Win()
    {
        winText.text = "You came up clutch in money time. The glory is yours !";
        winScore.text = GameManager.instance.PlayerScore.ToString() + " - " + GameManager.instance.OpponentScore.ToString();
        winScreen.SetActive(true);
    }

    private void Lose()
    {
        string loseConditionText = "";
        string loseScoreText = "";
        switch (GameManager.instance.lastLoseConditionRegistered)
        {
            case GameManager.LoseCondition.BallCaptured:
                loseConditionText = "Turnover.\n\nThey got the ball... and the win.";
                break;
            case GameManager.LoseCondition.BallOut:
                loseConditionText = "Out of bounds.\n\nThe ball's gone, and so are you.";
                break;
            case GameManager.LoseCondition.ScoreInsuficent:
                loseConditionText = "Buzzer. Time's up, and you came up short.";
                loseScoreText = GameManager.instance.PlayerScore.ToString() + " - " + GameManager.instance.OpponentScore.ToString();
                break;
        }
        loseText.text = loseConditionText;
        loseScore.text = loseScoreText;
        loseScreen.SetActive(true);
    }
}
