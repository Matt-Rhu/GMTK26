using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetDisplay : MonoBehaviour
{
    [SerializeField] private LineRenderer line;
    [SerializeField] private UnitBase unit;
    
    private enum DisplayType {Movement, Throw}
    [SerializeField] private DisplayType displayType;


    private void Start()
    {
        GameManager.instance.OnTacticalPause += ToggleRend;

        if (unit.data.isOpponent)
            displayType = DisplayType.Movement;
        
        line.colorGradient = displayType is DisplayType.Movement ? unit.data.moveDisplayColour : unit.data.throwDisplayColour;
    }

    private void Update()
    {
        if (displayType is DisplayType.Throw)
            ToggleRend(((PlayerUnit)unit).GetThrowCommand() is not PlayerUnit.ThrowCommand.NONE);
        
        line.SetPosition(0, unit.transform.position);
        line.SetPosition(1, TargetPosition());
    }

    private Vector3 TargetPosition()
    {
        if (displayType is DisplayType.Movement)
            return unit.GetTarget();
        return ((PlayerUnit)unit).GetThrowTarget();
    }

    private void ToggleRend(bool on)
    {
        line.gameObject.SetActive(on);
    }
}
