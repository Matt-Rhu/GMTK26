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
    

    [SerializeField] private GameObject selectionFX;
    public bool holdingBall;

    public enum Command
    {
        STOP,
        PASS,
        SHOOT,
        MOVE_TO
    }

    private Command nextCommand = Command.STOP;
    private Vector3 nextCommandTargetPosition = Vector3.zero;

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

    
    private void Start()
    {
        Deselect();
    }

    private void Update()
    {
        base.Update();
        // Only process PlayerUnit commands when not in tactical pause.
        if (!GameManager.instance.TacticalPause)
        {
            // Process next command.
            switch (nextCommand)
            {
                case Command.MOVE_TO:
                    ProcessMoveCommand();
                    break;
                case Command.PASS:
                    ProcessPassCommand();
                    break;
                case Command.SHOOT:
                    ProcessShootCommand();
                    break;
                case Command.STOP:
                    // Do nothing.
                    break;
            }
            // Drag the ball if holding it anyway.
            if (holdingBall)
            {
                Ball.instance.transform.position = transform.position;
            }
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
        nextCommand = Command.STOP;
        nextCommandTargetPosition = new Vector3(-9999, -9999, -9999);
    }


    public void RegisterMoveCommand(Vector3 targetPosition)
    {
        nextCommand = Command.MOVE_TO;
        nextCommandTargetPosition = targetPosition;
    }


    public void RegisterPassCommand(Vector3 targetPosition)
    {
        nextCommand = Command.PASS;
        nextCommandTargetPosition = targetPosition;
    }


    private void ProcessMoveCommand()
    {
        float distanceToTarget = (nextCommandTargetPosition - transform.position).magnitude;
        if (distanceToTarget > 1) // If did not reach distination, continue to move toward it.
        {
            transform.position += (nextCommandTargetPosition - transform.position).normalized * moveSpeed * Time.deltaTime;
        } else // If approximatively reached destination, then stop.
        {
            nextCommand = Command.STOP;
        }
    }


    private void ProcessPassCommand()
    {
        // Always stop after pass.
        nextCommand = Command.STOP;
        if (holdingBall)
        {
            holdingBall = false;
            Ball.instance.Pass(nextCommandTargetPosition);
        }
    }


    private void ProcessShootCommand()
    {
        // Always stop after shoot.
        nextCommand = Command.STOP;
        if (holdingBall)
        {
            holdingBall = false;
            Ball.instance.Shoot(nextCommandTargetPosition);
        }
    }
}
