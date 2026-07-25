using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class TargetDisplay : MonoBehaviour
{
    private enum DisplayType {Movement, Throw}
    [SerializeField] private DisplayType displayType;
    
    [Space]
    [SerializeField] private UnitBase unit;
    [SerializeField] private LineRenderer line;
    [FormerlySerializedAs("cursor")] [SerializeField] private SpriteRenderer throwCursor;
    [SerializeField] private SpriteRenderer moveCursor;
    private SpriteRenderer actualCursor;

    [Space]
    [SerializeField] private bool displayTimeToDestination = true;
    [HideWithValue(nameof(displayTimeToDestination))] [SerializeField] private TMP_Text text;
    [HideWithValue(nameof(displayTimeToDestination))] [SerializeField] private float textOffset = 1.5f;
    


    private void Start()
    {
        GameManager.instance.OnTacticalPause += ToggleRend;

        if (unit.data.isOpponent)
            displayType = DisplayType.Movement;
        
        line.colorGradient = displayType is DisplayType.Movement ? unit.data.moveDisplayColour : unit.data.throwDisplayColour;
        
        actualCursor = displayType is DisplayType.Movement ? moveCursor : throwCursor;
        actualCursor.color = line.colorGradient.Evaluate(1);
    }

    private void Update()
    {
        var isMoveDisplay = displayType is DisplayType.Movement;
        
        if (!isMoveDisplay)
            ToggleRend(((PlayerUnit)unit).GetThrowCommand() is not PlayerUnit.ThrowCommand.NONE);
        
        // place line points and cursor
        line.SetPosition(0, unit.transform.position);
        Vector3 targetPosition = TargetPosition();
        actualCursor.transform.position = new Vector3(targetPosition.x, 1, targetPosition.z);
        var dir = actualCursor.transform.position - unit.transform.position;
        var offsetFromCursor = isMoveDisplay ? 0.2f : 0.7f;
        line.SetPosition(1, unit.transform.position + dir.normalized * (dir.magnitude - offsetFromCursor));
        
        // if movement display, arrow faces direction
        if (isMoveDisplay)
           actualCursor.transform.parent.LookAt(actualCursor.transform.position + dir);
        
        // if duration display, duration stuff
        if (!displayTimeToDestination)
        {
            text.gameObject.SetActive(false);
            return;
        }
        text.transform.position = line.GetPosition(1) + new Vector3(textOffset, 2, 0);
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
        // float ballSpeed = ((((PlayerUnit)unit).GetThrowTarget() - unit.transform.position) * ).magnitude;
        return Vector3.Distance(((PlayerUnit)unit).GetThrowTarget(), unit.transform.position) / Ball.instance.InitialVelocity;
    }

    private void ToggleRend(bool on)
    {
        line.gameObject.SetActive(on);
    }
}
