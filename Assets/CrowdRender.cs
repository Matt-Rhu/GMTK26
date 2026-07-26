using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrowdRender : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private Animator crowdAnimator;
    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.TacticalPause)
        {
            crowdAnimator.speed = 0;
            return;
        }
        crowdAnimator.speed = 1;
    }
}
