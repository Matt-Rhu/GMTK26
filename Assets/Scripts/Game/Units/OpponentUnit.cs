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
    
    private float t;


    protected override void ActiveBehaviour()
    {
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
        return new Vector3();
    }
    
    private Vector3 ToutoutTarget()
    {
        return new Vector3();
    }

    private Vector3 IntercepteurTarget()
    {
        return new Vector3();
    }

    private Vector3 SentinelleTarget()
    {
        return new Vector3();
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
