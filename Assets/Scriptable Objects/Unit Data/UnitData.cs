using UnityEngine;
using UnityEngine.Timeline;

[CreateAssetMenu(fileName = "New Unit", menuName = "Unit Data", order = 0)]
public class UnitData : ScriptableObject
{
    [Header("Movement Settings")]
    public float moveSpeed = 5;
    public float directionInterpolationSpeed = 2.5f;
    public float idleMoveRadius = 2f;
    
    [Header("Ball Interaction Settings")]
    public float zoneRadius = 1;
    public float ballSeekingRadius = 2.5f;
    public float grabCooldown = 3f;
    public Ball.BallState[] canGrabBallInStates;

    [Header("Opponent Settings")]
    public bool isOpponent = true;
    [HideWithValue(nameof(isOpponent))] public OpponentUnit.BehaviourType behaviour;
    [HideWithValue(nameof(isOpponent))] public float targetRefreshRate = 0.5f;
    
    [Header("Visuals")]
    public Sprite sprite;
}
