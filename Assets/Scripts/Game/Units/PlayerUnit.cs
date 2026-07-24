using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUnit : UnitBase
{
    [SerializeField] private GameObject selectionFX;

    public enum ThrowCommand
    {
        NONE,
        PASS,
        SHOOT
    }

    public enum MoveCommand
    {
        STOP,
        MOVE_TO
    }

    private ThrowCommand nextThrowCommand = ThrowCommand.NONE;
    private Vector3 nextThrowTargetPosition = Vector3.zero;
    private MoveCommand nextMoveCommand = MoveCommand.STOP;

    
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

    
    protected override void Start()
    {
        base.Start();
        Deselect();
    }
    
    protected override void ActiveBehaviour()
    {
        TryGrabBall();
        // Process next throw command.
        switch (nextThrowCommand)
        {
            case ThrowCommand.PASS:
                ProcessPassCommand();
                break;
            case ThrowCommand.SHOOT:
                ProcessShootCommand();
                break;
            case ThrowCommand.NONE:
                // Do nothing.
                break;
        }
        // Process next move command;
        switch (nextMoveCommand)
        {
            case MoveCommand.MOVE_TO:
                ProcessMoveCommand();
                break;
            case MoveCommand.STOP:
                IdleAtTarget();
                break;
        }
        // Drag the ball if holding it anyway.
        if (hasBall)
        {
            Ball.instance.transform.position = transform.position;
        }
    }

    public void Select()
    {
        selectionFX.SetActive(true);
    }


    public void Deselect()
    {
        selectionFX.SetActive(false);
    }


    public void RegisterStopCommand()
    {
        nextMoveCommand = MoveCommand.STOP;
        targetPos = new Vector3(-9999, -9999, -9999);
    }


    public void RegisterMoveCommand(Vector3 targetPosition)
    {
        nextMoveCommand = MoveCommand.MOVE_TO;
        targetPos = targetPosition;
    }


    public void RegisterPassCommand(Vector3 targetPosition)
    {
        nextThrowCommand = ThrowCommand.PASS;
        nextThrowTargetPosition = targetPosition;
    }


    public void RegisterShootCommand(Vector3 targetPosition)
    {
        nextThrowCommand = ThrowCommand.SHOOT;
        nextThrowTargetPosition = targetPosition;
    }

    public void CancelThrowCommand()
    {
        nextThrowCommand = ThrowCommand.NONE;
        nextThrowTargetPosition = new Vector3(-9999, -9999, -9999);
    }

    private void ProcessMoveCommand()
    {

        /*float distanceToTarget = (targetPos - transform.position).magnitude;
        if (distanceToTarget > 1) // If did not reach distination, continue to move toward it.
        {
            transform.position += (targetPos - transform.position).normalized * moveSpeed * Time.deltaTime;
        } else // If approximatively reached destination, then stop.
        {
            nextCommand = Command.STOP;
        }*/

        if (Vector3.Distance(transform.position, targetPos) > data.zoneRadius * 0.5f)
        {
            MoveTo(targetPos);
        } else
        {
            nextMoveCommand = MoveCommand.STOP;
        }
    }


    private void ProcessPassCommand()
    {
        // Always stop after pass.
        nextThrowCommand = ThrowCommand.NONE;
        if (hasBall)
        {
            hasBall = false;
            Pass(nextThrowTargetPosition);
        }
    }


    private void ProcessShootCommand()
    {
        // Always stop after shoot.
        nextThrowCommand = ThrowCommand.NONE;
        if (hasBall)
        {
            hasBall = false;
            Shoot(nextThrowTargetPosition);
        }
    }

    private void Shoot(Vector3 target)
    {
        var scoreValue = IsInScoreZone() ? GameManager.instance.inZoneScore : GameManager.instance.outZoneScore;
        Ball.instance.Shoot(target, scoreValue);
    }

    private void Pass(Vector3 target)
    {
        Ball.instance.Pass(target);
    }


    protected override void OnDataChanged()
    {
        if (!data) return;
        
        if (data.isOpponent)
        {
            Debug.LogWarning($"Can't have an opponent unit data on a player unit!!");
            data = null;
            return;
        }
        
        base.OnDataChanged();
    }
}
