using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Scores : MonoBehaviour
{
    [SerializeField] private TMP_Text playerScore;
    [SerializeField] private TMP_Text opponentScore;
    [Space] 
    [SerializeField] private string playerName = "P1:";
    [SerializeField] private string opponentName = "CPU:";
    
    private void Update()
    {
        playerScore.text = $"{playerName} {GameManager.instance.PlayerScore.ToString()}";
        opponentScore.text = $"{opponentName} {GameManager.instance.OpponentScore.ToString()}";
    }
}
