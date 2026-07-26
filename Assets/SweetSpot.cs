using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SweetSpot : MonoBehaviour
{
    public int Score = 3;
    [Space] [SerializeField] private TMP_Text text;

    private void OnValidate()
    {
        if (!text) return;
        
        text.text = Score.ToString();
    }
}
