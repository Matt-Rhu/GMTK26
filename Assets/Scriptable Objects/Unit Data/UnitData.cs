using UnityEngine;
using UnityEngine.Timeline;

[CreateAssetMenu(fileName = "New Unit", menuName = "Unit Data", order = 0)]
public class UnitData : ScriptableObject
{
    public Sprite sprite;
    [Space] 
    public Ball.BallState[] canGrabBallInStates;
    public bool isOpponent = true;
    [HideWithValue(nameof(isOpponent))] public OpponentUnit.BehaviourType behaviour;
}
