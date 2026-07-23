using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUnit : UnitBase
{
    protected override void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        base.OnDrawGizmosSelected();
    }
}
