using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Scores : MonoBehaviour
{
    [SerializeField] private TMP_Text playerScore;
    [SerializeField] private TMP_Text opponentScore;
    
    private void Update()
    {
        playerScore.text = GameManager.instance.PlayerScore.ToString();
        opponentScore.text = GameManager.instance.OpponentScore.ToString();
    }
}
