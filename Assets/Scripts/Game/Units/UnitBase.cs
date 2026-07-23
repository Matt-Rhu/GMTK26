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
        if (GameManager.instance.TacticalPause) 
            TacticalPauseBehaviour();
        else
            ActiveBehaviour();
    }
    
    
    protected virtual void ActiveBehaviour()
    {
        if (Vector3.Distance(transform.position, targetPos) > 0.1f)
            GoToTarget();
        else
            IdleAtTarget();
    }

    protected virtual void GoToTarget()
    {
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
        int count = Physics.OverlapSphereNonAlloc(transform.position, zoneRadius, new Collider[5], LayerMask.NameToLayer("Ball"));
        return count > 0;
    }

    protected virtual void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, zoneRadius);
    }
}
