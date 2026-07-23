using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUnit : UnitBase
{
    private bool OpponentInZone()
    {
        int count = Physics.OverlapSphereNonAlloc(transform.position, zoneRadius, new Collider[1], LayerMask.NameToLayer("OpponentUnit"));
        return count > 0;
    }
    
    protected override void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        base.OnDrawGizmosSelected();
    }
}
