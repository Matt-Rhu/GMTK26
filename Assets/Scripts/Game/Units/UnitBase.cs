using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UnitBase : MonoBehaviour
{
    [OnChange(nameof(OnDataChanged))] public UnitData data;


    private Vector3 trueTarget;
    protected Vector3 targetPos;

    private Vector3 moveDir;
    
    private float timerBeforeCanGrabAgain = -99f;
    [HideInInspector] public bool hasBall;


    protected virtual void Start()
    {
        if (!data)
        {
            Debug.LogError($"Unit data for {name} wasn't set!! Violently destroyed it!!!");
            Destroy(gameObject);
            return;
        }
        
        UpdateSprite();
        
        SetTarget(transform.position);
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

    protected void MoveTo(Vector3 target)
    {
        if (Vector3.Distance(transform.position, target) > data.zoneRadius * 0.5f)
        {
            moveDir = Vector3.Lerp(moveDir, target - transform.position, Time.deltaTime * data.directionInterpolationSpeed);
            transform.Translate(moveDir.normalized * (data.moveSpeed * Time.deltaTime));
        }
        else
            IdleAtTarget();
    }

    protected virtual void IdleAtTarget()
    {
        var rnd = RandomVectors.Range3(-data.idleMoveRadius, data.idleMoveRadius);
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

    public Vector3 GetTarget()
    {
        return targetPos;
    }


    protected void TryGrabBall()
    {
		if (timerBeforeCanGrabAgain > 0)
        {
            timerBeforeCanGrabAgain -= Time.deltaTime;
            return;
        }
		
        if (!BallInGrabZone()) return;
        if (!CanGrabBall()) return;
        Ball.instance.Grab(this);
        timerBeforeCanGrabAgain = data.grabCooldown;
        
        if (data.isOpponent)
            GameManager.instance.Lose();
    }
    
    private bool BallInGrabZone()
    {
        int count = Physics.OverlapSphereNonAlloc(transform.position, data.zoneRadius, new Collider[1], LayerMask.GetMask("Ball"));
        return count > 0;
    }
    
    private bool IdleBallInSeekZone()
    {
        int count = Physics.OverlapSphereNonAlloc(transform.position, data.ballSeekingRadius, new Collider[1], LayerMask.GetMask("Ball"));
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
        if (!data) return;
        
        Gizmos.DrawWireSphere(transform.position, data.zoneRadius);
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, data.ballSeekingRadius);
    }


    protected virtual void OnDataChanged()
    {
        UpdateSprite();
    }

    [Button]
    protected void UpdateSprite()
    {
        if (!data.sprite) return;
        
        var sr = GetComponentInChildren<SpriteRenderer>();
        if (!sr) return;
        
        sr.sprite = data.sprite;
    }
}
