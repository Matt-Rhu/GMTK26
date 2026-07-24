using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUnit : UnitBase
{
    private void Shoot(Vector3 target)
    {
        var scoreValue = IsInScoreZone() ? GameManager.instance.inZoneScore : GameManager.instance.outZoneScore;
        Ball.instance.Shoot(target, scoreValue);
    }

    private void Pass()
    {
        
    }
    
    public bool CanThrow()
    {
        return !OpponentInZone() && hasBall;
    }
    
    private bool OpponentInZone()
    {
        var count = Physics.OverlapSphereNonAlloc(transform.position, data.zoneRadius, new Collider[1], LayerMask.GetMask("OpponentUnit"));
        return count > 0;
    }

    private bool IsInScoreZone()
    {
        var count = Physics.OverlapSphereNonAlloc(transform.position, 0.1f, new Collider[1], LayerMask.GetMask("ScoreZone"));
        return count > 0;
    }
    
    protected override void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        base.OnDrawGizmosSelected();
    }
}
