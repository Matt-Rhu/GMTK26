using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    [SerializeField] private TMP_Text timerText;

    [Space] 
    [SerializeField] private float flickerSpeed = 1;

    private float t;
    
    private void Update()
    {
        timerText.text = NumberFormatting.Decimals(GameManager.instance.RemainingTime, 2);
        
        
        if (!GameManager.instance.TacticalPause)
        {
            timerText.gameObject.SetActive(true);
            return;
        }
        
        t += Time.deltaTime * flickerSpeed;

        if (!(t > 1)) return;
        t = 0;
        timerText.gameObject.SetActive(!timerText.gameObject.activeSelf);
    }
}
