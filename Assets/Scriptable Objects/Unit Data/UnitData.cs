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
    [HideWithValue(nameof(isOpponent), true)] public float grabCooldown = 0.5f;
    public Ball.BallState[] canGrabBallInStates;

    [Header("Opponent Settings")]
    public bool isOpponent = true;
    [HideWithValue(nameof(isOpponent))] public OpponentUnit.BehaviourType behaviour;
    [HideWithValue(nameof(isOpponent))] public float targetRefreshRate = 0.5f;
    
    [Header("Visuals")]
    public Sprite sprite;
    public Gradient moveDisplayColour;
    [HideWithValue(nameof(isOpponent), true)] public Gradient throwDisplayColour;

    [Header("Sounds")] 
    public SoundReference grabSound;
    public SoundReference squeakSound;
    public float squeakCooldown = 2;
    public float squeakThreshold = 0.5f;
}
