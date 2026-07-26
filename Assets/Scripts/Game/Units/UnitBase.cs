using System;
using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;

public abstract class UnitBase : MonoBehaviour
{
    [OnChange(nameof(OnDataChanged))] public UnitData data;


    private Vector3 trueTarget;
    protected Vector3 targetPos;

    private Vector3 moveDir;
    
    protected float timerBeforeCanGrabAgain = -99f;
    [HideInInspector] public bool hasBall;

    private float squeakTimer;


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
        squeakTimer += Time.deltaTime;
        
        if (!GameManager.instance.GameStarted) return;

        if (GameManager.instance.TacticalPause) return;
        
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
            var newdir = target - transform.position;
            moveDir = Vector3.Lerp(moveDir, newdir, Time.deltaTime * data.directionInterpolationSpeed);
            transform.Translate(moveDir.normalized * (data.moveSpeed * Time.deltaTime));
            
            if (Vector3.Dot(moveDir, newdir) < data.squeakThreshold)
                TryPlaySqueakSound();
        }
        else
            IdleAtTarget();
    }
    

    protected virtual void IdleAtTarget()
    {
        //iterate until finds a position that is not outside the terrain
        while (true)
        {
            var rnd = RandomVectors.Range3(-data.idleMoveRadius, data.idleMoveRadius);
            rnd.y = 0;

            var pos = trueTarget + rnd;

            var ray = new Ray(pos, Vector3.down);
            if (Physics.Raycast(ray, Mathf.Infinity, LayerMask.GetMask("Terrain")))
                targetPos = pos;
            else
                continue;
            break;
        }
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

        if (GameManager.instance.GameOver) return;
        if (!BallInGrabZone()) return;
        if (!CanGrabBall()) return;
        
        Ball.instance.Grab(this);
        RuntimeManager.PlayOneShot(data.grabSound.Ref);
        
        if (data.isOpponent)
            GameManager.instance.Lose(GameManager.LoseCondition.BallCaptured);
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


    private void TryPlaySqueakSound()
    {
        if (squeakTimer <= data.squeakCooldown) return;
        
        squeakTimer = 0f;
        RuntimeManager.PlayOneShot(data.squeakSound.Ref);
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
