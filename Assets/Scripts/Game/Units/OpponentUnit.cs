using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpponentUnit : UnitBase
{
    private float t;
    
    protected override void ActiveBehaviour()
    {
        base.ActiveBehaviour();
        
        t += Time.deltaTime / data.targetRefreshRate;
        if (t >= 1f)
        {
            t = 0f;
            SetTarget(GetTarget());
        }
    }
    

    private Vector3 GetTarget()
    {
        return data.behaviour switch
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
        return Vector3.Lerp(Ball.instance.transform.position, ClosestPlayerUnit(true), 0.5f);
    }

    private Vector3 SentinelleTarget()
    {
        return new();
    }


    private Vector3 ClosestPlayerUnit(bool excludeBallHolder = false)
    {
        float shortestDistance = Mathf.Infinity;
        Vector3 closest = transform.position;
        Collider[] results = new Collider[10];
        Physics.OverlapSphereNonAlloc(transform.position, 100, results, LayerMask.GetMask("PlayerUnit"));
        foreach (Collider c in results)
        {
            if (c != null) {
                // Ignore the ball holder if parameter says so.
                if (excludeBallHolder && c.transform.gameObject.TryGetComponent<PlayerUnit>(out PlayerUnit playerUnit) && playerUnit == Ball.instance.UnitHoldingIt)
                {
                    continue;
                }
                
                float dist = Vector3.Distance(transform.position, c.transform.position);
                if (!(dist < shortestDistance)) continue;
                shortestDistance = dist;
                closest = c.transform.position;
            }
            
        }
        return closest;
    }
    
    protected override void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        base.OnDrawGizmosSelected();
    }
    
    protected override void OnDataChanged()
    {
        if (!data.isOpponent)
        {
            Debug.LogWarning($"Can't have a player unit data on an opponent unit!!");
            data = null;
            return;
        }
        
        base.OnDataChanged();
    }
    
    
    public enum BehaviourType
    {
        Presseur = 0,   
        Toutout,        
        Intercepteur,   
        Sentinelle
    }
}
