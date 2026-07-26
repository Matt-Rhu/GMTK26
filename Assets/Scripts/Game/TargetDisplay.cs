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
    
    [Space]
    [SerializeField] private TMP_Text shootText;
    [HideWithValue(nameof(displayType), 1, false)] [SerializeField] private float shootTextOffset = 2.5f;

    [Space]
    [SerializeField] private bool displayTimeToDestination = true;
    [FormerlySerializedAs("text")] [HideWithValue(nameof(displayTimeToDestination))] [SerializeField] private TMP_Text durationText;
    [HideWithValue(nameof(displayTimeToDestination))] [SerializeField] private float textOffset = 1.5f;
    
    private SpriteRenderer actualCursor;


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
        float lineYDepth = 30f;
        if (isMoveDisplay)
        {
            lineYDepth = 1f;
        }
        
        Vector3 lineStartPosition = unit.transform.position;
        line.SetPosition(0, new Vector3(lineStartPosition.x, lineYDepth-0.1f, lineStartPosition.z));
        Vector3 targetPosition = TargetPosition();
        actualCursor.transform.position = new Vector3(targetPosition.x, lineYDepth, targetPosition.z);
        Vector3 dir = actualCursor.transform.position - unit.transform.position;
        var offsetFromCursor = isMoveDisplay ? 0.2f : 0.7f;
        Vector3 lineEndPosition = unit.transform.position + dir.normalized * (dir.magnitude - offsetFromCursor);
        line.SetPosition(1, new Vector3(lineEndPosition.x, lineYDepth-0.1f, lineEndPosition.z));
        
        // if movement display, arrow faces direction
        if (isMoveDisplay)
           actualCursor.transform.parent.LookAt(actualCursor.transform.position + dir);
        
        SetDurationDisplay();
        SetScoreDisplay();
    }

    private Vector3 TargetPosition()
    {
        if (displayType is DisplayType.Movement)
            return unit.GetTarget();
        return ((PlayerUnit)unit).GetThrowTarget();
    }

    private void SetScoreDisplay()
    {
        shootText.gameObject.SetActive(false);
        
        if (displayType is DisplayType.Movement) return;
        if (unit.data.isOpponent) return;
        if (((PlayerUnit)unit).GetThrowCommand() is not PlayerUnit.ThrowCommand.SHOOT) return;

        shootText.gameObject.SetActive(true);
        
        var points = ((PlayerUnit)unit).CalculateScore();
        shootText.text = $"+{points}pts";
        if (points == 0)
            shootText.text = "Miss";
        
        shootText.transform.position = Goal.instance.transform.position + new Vector3(shootTextOffset, 0, 0);
    }

    private void SetDurationDisplay()
    {
        if (!displayTimeToDestination)
        {
            durationText.gameObject.SetActive(false);
            return;
        }
        durationText.transform.position = TargetPosition() + new Vector3(textOffset, 2, 0);
        durationText.text = $"{NumberFormatting.Decimals(DurationToTarget(), 1)}s";
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
