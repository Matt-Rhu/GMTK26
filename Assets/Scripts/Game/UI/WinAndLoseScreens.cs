using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinAndLoseScreens : MonoBehaviour
{
    [SerializeField] private GameObject winScreen, loseScreen;
    
    public void Start()
    {
        GameManager.OnLose += Lose;
        GameManager.OnWin += Win;
    }
    
    private void Win()
    {
        winScreen.SetActive(true);
    }

    private void Lose()
    {
        loseScreen.SetActive(true);
    }
}
