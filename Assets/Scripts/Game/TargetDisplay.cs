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
    [SerializeField] private LineRenderer bounceLine;
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
        
        line.colorGradient = bounceLine.colorGradient = IsMoveDisplay() ? unit.data.moveDisplayColour : unit.data.throwDisplayColour;
        
        actualCursor = IsMoveDisplay() ? moveCursor : throwCursor;
        actualCursor.color = line.colorGradient.Evaluate(1);
    }

    private void Update()
    {
        if (!IsMoveDisplay())
            ToggleRend(((PlayerUnit)unit).GetThrowCommand() is not PlayerUnit.ThrowCommand.NONE);

        // place line points and cursor
        float lineYDepth = 30f;
        if (IsMoveDisplay())
        {
            lineYDepth = 1f;
        }
        
        Vector3 lineStartPosition = unit.transform.position;
        line.SetPosition(0, new Vector3(lineStartPosition.x, lineYDepth-0.1f, lineStartPosition.z));
        Vector3 targetPosition = TargetPosition();
        actualCursor.transform.position = new Vector3(targetPosition.x, lineYDepth, targetPosition.z);
        Vector3 dir = actualCursor.transform.position - unit.transform.position;
        var offsetFromCursor = IsMoveDisplay() ? 0.2f : 0.7f;
        Vector3 lineEndPosition = unit.transform.position + dir.normalized * (dir.magnitude - OffsetFromCursor());
        line.SetPosition(1, new Vector3(lineEndPosition.x, lineYDepth-0.1f, lineEndPosition.z));
        
        // if movement display, arrow faces direction
        if (IsMoveDisplay())
           actualCursor.transform.parent.LookAt(actualCursor.transform.position + dir);
        
        BounceDisplay();
        
        SetDurationDisplay();
        SetScoreDisplay();
    }

    private Vector3 TargetPosition()
    {
        if (displayType is DisplayType.Movement)
            return unit.GetTarget();
        return ((PlayerUnit)unit).GetThrowTarget();
    }

    private void BounceDisplay()
    {
        bounceLine.gameObject.SetActive(false);
        
        if (displayType is DisplayType.Movement) return;
        if (((PlayerUnit)unit).GetThrowCommand() is not PlayerUnit.ThrowCommand.SHOOT) return;

        bounceLine.gameObject.SetActive(true);

        var shotDir = ((PlayerUnit)unit).GetThrowTarget() - unit.transform.position;
        var dir = Vector3.Reflect(shotDir.normalized, -Vector3.forward) * 5;
        var point1 = actualCursor.transform.position + dir.normalized * 0.2f;
        var point2 = actualCursor.transform.position + dir;
        bounceLine.SetPosition(0, new Vector3(point1.x, point1.y - 0.1f, point1.z));
        bounceLine.SetPosition(1, new Vector3(point2.x, point2.y - 0.1f, point2.z));
    }

    private void SetScoreDisplay()
    {
        shootText.gameObject.SetActive(false);
        
        if (displayType is DisplayType.Movement) return;
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
        return Vector3.Distance(((PlayerUnit)unit).GetThrowTarget(), unit.transform.position) / Ball.instance.InitialVelocity;
    }

    private void ToggleRend(bool on)
    {
        line.gameObject.SetActive(on);
    }
    
    private bool IsMoveDisplay()
    {
        return displayType is DisplayType.Movement;
    }

    private float OffsetFromCursor()
    {
        return IsMoveDisplay() ? 0.2f : 0.7f;
    }
}
