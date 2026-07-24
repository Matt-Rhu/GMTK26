using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TargetDisplay : MonoBehaviour
{
    [SerializeField] private UnitBase unit;
    [SerializeField] private LineRenderer line;
    [SerializeField] private TMP_Text text;
    [SerializeField] private float textOffset = 1.5f;
    
    private enum DisplayType {Movement, Throw}
    [Space] [SerializeField] private DisplayType displayType;


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

        text.transform.position = line.GetPosition(1) + new Vector3(textOffset, 0, 0);
        text.text = $"{NumberFormatting.Decimals(DurationToTarget(), 1)}s";
    }

    private Vector3 TargetPosition()
    {
        if (displayType is DisplayType.Movement)
            return unit.GetTarget();
        return ((PlayerUnit)unit).GetThrowTarget();
    }

    private float DurationToTarget()
    {
        if (displayType is DisplayType.Movement)
            return Vector3.Distance(unit.GetTarget(), unit.transform.position) / unit.data.moveSpeed;
        float ballSpeed = ((((PlayerUnit)unit).GetThrowTarget() - unit.transform.position) * Ball.instance.shotForceFactor).magnitude;
        return Vector3.Distance(((PlayerUnit)unit).GetThrowTarget(), unit.transform.position) / ballSpeed;
    }

    private void ToggleRend(bool on)
    {
        line.gameObject.SetActive(on);
    }
}
