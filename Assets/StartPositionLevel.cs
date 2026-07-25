using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartPositionLevel : MonoBehaviour
{

    [SerializeField] private float remainingTime = 30;
    [Space]
    [SerializeField] private int playerStartScore = 75;
    [SerializeField] private int opponentStartScore = 77;
    [Space]
    [SerializeField] private int inZoneScore = 2;
    [SerializeField] private int outZoneScore = 3;

    private void Start()
    {
        GameManager.instance.InitializeFromStartPositionDatas(remainingTime, playerStartScore, opponentStartScore, inZoneScore, outZoneScore);
    }
}
