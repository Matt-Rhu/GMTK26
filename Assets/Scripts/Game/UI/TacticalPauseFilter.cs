using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TacticalPauseFilter : MonoBehaviour
{
    [SerializeField] private RawImage filter;
    [Space]
    [SerializeField] private float alpha = 0.25f;
    [SerializeField] private float transitionSpeed = 5;
    
    private void Update()
    {
        var targetAlpha = GameManager.instance.TacticalPause ? alpha : 0;
        var col =  filter.color;
        col.a = targetAlpha;
        
        filter.color = Color.Lerp(filter.color, col, transitionSpeed * Time.deltaTime);
    }
}
