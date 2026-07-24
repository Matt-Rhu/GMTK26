using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UnitBase : MonoBehaviour
{
    [SerializeField] protected UnitData data;
    [Space] 
    [SerializeField] protected float moveSpeed;
    [SerializeField] protected float directionInterpolationSpeed = 3;
    [SerializeField] protected float idleMoveRadius = 1.5f;
    [Space]
    [SerializeField] protected float zoneRadius;
    [SerializeField] protected float ballSeekingRadius = 2.5f;

    protected Vector3 trueTarget;
    protected Vector3 targetPos;

    protected Vector3 moveDir;
    
    protected bool hasBall;


    protected virtual void Start()
    {
        UpdateSprite();
    }
    
    protected virtual void Update()
    {
        if (GameManager.instance.OutOfTime) return;
        
        if (GameManager.instance.TacticalPause) 
            TacticalPauseBehaviour();
        else
            ActiveBehaviour();
    }
    
    
    protected virtual void ActiveBehaviour()
    {
        TryGrabBall();
        
        var target = IdleBallInSeekZone() ? Ball.instance.transform.position : targetPos;
        MoveTo(target);
    }

    private void MoveTo(Vector3 target)
    {
        if (Vector3.Distance(transform.position, target) > zoneRadius * 0.5f)
        {
            moveDir = Vector3.Lerp(moveDir, target - transform.position, Time.deltaTime * directionInterpolationSpeed);
            transform.Translate(moveDir.normalized * (moveSpeed * Time.deltaTime));
        }
        else
            IdleAtTarget();
    }

    protected virtual void IdleAtTarget()
    {
        var rnd = RandomVectors.Range3(-idleMoveRadius, idleMoveRadius);
        rnd.y = 0;
        targetPos = trueTarget + rnd;
    }
    

    protected virtual void TacticalPauseBehaviour()
    {
        
    }

    public void SetTarget(Vector3 target)
    {
        targetPos = trueTarget = target;
    }


    private void TryGrabBall()
    {
        if (!BallInGrabZone()) return;
        if (!CanGrabBall()) return;
        hasBall = true;
        Ball.instance.ChangeState(Ball.BallState.Held);
        
        if (data.isOpponent)
            GameManager.instance.Lose();
    }
    
    private bool BallInGrabZone()
    {
        int count = Physics.OverlapSphereNonAlloc(transform.position, zoneRadius, new Collider[1], LayerMask.GetMask("Ball"));
        return count > 0;
    }
    
    private bool IdleBallInSeekZone()
    {
        int count = Physics.OverlapSphereNonAlloc(transform.position, ballSeekingRadius, new Collider[1], LayerMask.GetMask("Ball"));
        return count > 0 && Ball.instance.CurrentState is Ball.BallState.Idle;
    }

    private bool CanGrabBall()
    {
        foreach (var state in data.canGrabBallInStates)
            if (Ball.instance.CurrentState == state)
                return true;
        return false;
    }

    protected virtual void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, zoneRadius);
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, ballSeekingRadius);
    }
    

    [Button]
    protected void UpdateSprite()
    {
        if (data.sprite == null) return;
        
        var sr = GetComponentInChildren<SpriteRenderer>();
        if (sr == null) return;
        
        sr.sprite = data.sprite;
    }
}
