using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpponentUnit : UnitBase
{
    [OnChange(nameof(SetSprite))] [SerializeField] private BehaviourType behaviour;
    //TODO: need to change the sprite based on behaviour 
    //private SpriteRenderer spriteRenderer;

    [SerializeField] private float targetRefreshRate = 0.5f;

    public Transform target;
    
    private float t;


    protected override void ActiveBehaviour()
    {
        if (hasBall) 
            GameManager.instance.Lose();
        
        base.ActiveBehaviour();

        t += Time.deltaTime / targetRefreshRate;
        if (t >= 1f)
        {
            t = 0f;
            targetPos = GetTarget();
        }
    }
    

    private Vector3 GetTarget()
    {
        return behaviour switch
        {
            BehaviourType.Presseur => PresseurTarget(),
            BehaviourType.Toutout => ToutoutTarget(),
            BehaviourType.Intercepteur => IntercepteurTarget(),
            BehaviourType.Sentinelle => SentinelleTarget(),
            _ => targetPos
        };
    }

    private Vector3 PresseurTarget()
    {
        return ClosestPlayerUnit();
    }
    
    private Vector3 ToutoutTarget()
    {
        return Ball.instance.transform.position;
    }

    private Vector3 IntercepteurTarget()
    {
        return Vector3.Lerp(Ball.instance.transform.position, ClosestPlayerUnit(), 0.5f);
    }

    private Vector3 SentinelleTarget()
    {
        return target.position;
    }


    private Vector3 ClosestPlayerUnit()
    {
        float shortestDistance = Mathf.Infinity;
        Vector3 closest = transform.position;
        Collider[] results = new Collider[10];
        Physics.OverlapSphereNonAlloc(transform.position, 100, results, LayerMask.NameToLayer("PlayerUnit"));
        foreach (Collider c in results)
        {
            float dist = Vector3.Distance(transform.position, c.transform.position);
            if (!(dist < shortestDistance)) continue;
            shortestDistance = dist;
            closest = c.transform.position;
        }
        return closest;
    }
    
    protected override void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        base.OnDrawGizmosSelected();
    }
    

    private void SetSprite()
    {
        print("update sprite");
    }
    
    private enum BehaviourType
    {
        Presseur = 0,   
        Toutout,        
        Intercepteur,   
        Sentinelle
    }
}
