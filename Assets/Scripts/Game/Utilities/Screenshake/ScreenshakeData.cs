using UnityEngine;


[CreateAssetMenu(fileName = "ScreenshakeData", menuName = "FX Data/Screenshake", order = 0)]
public class ScreenshakeData : ScriptableObject
{
    public float duration = 1;
    public float speed = 10;
    public float magnitude = 5;
    public AnimationCurve falloff;
}
