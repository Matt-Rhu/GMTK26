using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "HapticsData", menuName = "FX Data/Haptics", order = 1)]
public class HapticsOptions : ScriptableObject
{
    public float lowFrequency; 
    public float highFrequency;
    public float delay;
    public float duration;
}
