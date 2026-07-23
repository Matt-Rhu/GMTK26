using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UnitBase : MonoBehaviour
{
    [SerializeField] protected float zoneRadius;
    [SerializeField] protected float moveSpeed;
    
    protected Vector3 targetPos;
    protected bool hasBall;


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
        if (Vector3.Distance(transform.position, targetPos) > zoneRadius * 0.5f)
            GoToTarget();
        else
            IdleAtTarget();
    }

    protected virtual void GoToTarget()
    {
        //TODO: movement is extremely stiff for now, probably would need to accelerate and decelerate, and lerp the target direction so they don't do a hard turn after a target refresh
        transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
    }

    protected virtual void IdleAtTarget()
    {
        //probably just gonna move around at random within a small radius
        //maybe just set a new target nearby
    }
    

    protected virtual void TacticalPauseBehaviour()
    {
        
    }

    
    protected bool BallInZone()
    {
        int count = Physics.OverlapSphereNonAlloc(transform.position, zoneRadius, new Collider[1], LayerMask.NameToLayer("Ball"));
        return count > 0;
    }

    protected virtual void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, zoneRadius);
    }
}
