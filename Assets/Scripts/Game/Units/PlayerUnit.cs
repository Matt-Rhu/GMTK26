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
    
    private ThrowCommand nextThrowCommand = ThrowCommand.NONE;
    private Vector3 nextThrowTargetPosition = Vector3.zero;


    
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

    private bool IsOnSweetSpot()
    {
        var ray = new Ray(transform.position, Vector3.down);
        return Physics.Raycast(ray, Mathf.Infinity, LayerMask.GetMask("ScoreZone"));
    }

    private int GetSweetSpotMaxScore()
    {
        int result = 0;
        Ray raycast = new Ray(transform.position, Vector3.down);
        RaycastHit[] raycastHits = Physics.RaycastAll(raycast, Mathf.Infinity, LayerMask.GetMask("ScoreZone"));
        print(raycastHits);
        foreach (RaycastHit raycastHit in raycastHits)
        {
            if (raycastHit.transform != null)
            {
                GameObject hitGameObject = raycastHit.transform.gameObject;
                if (hitGameObject.TryGetComponent(out SweetSpot sweetSpot))
                {
                    result = Math.Max(result, sweetSpot.Score);
                }
            }
        }
        return result;
    }
    
    protected override void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        base.OnDrawGizmosSelected();
    }

    
    protected override void Start()
    {
        base.Start();
        HideHightlight();
    }
    
    protected override void ActiveBehaviour()
    {
        base.ActiveBehaviour();
        
        // Always hide Highlight when active.
        if (selectionFX.activeInHierarchy)
        {
            HideHightlight();
        }
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
    }

    public void ShowHighlight()
    {
        selectionFX.SetActive(true);
    }


    public void HideHightlight()
    {
        selectionFX.SetActive(false);
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
        timerBeforeCanGrabAgain = data.grabCooldown;
        Ball.instance.Shoot(target, CalculateScore());
    }

    public int CalculateScore()
    {
        return GetSweetSpotMaxScore();
    }

    private void Pass(Vector3 target)
    {
        timerBeforeCanGrabAgain = data.grabCooldown;
        Ball.instance.Pass(target);
    }

    public Vector3 GetThrowTarget()
    {
        return nextThrowTargetPosition;
    }

    public ThrowCommand GetThrowCommand()
    {
        return nextThrowCommand;
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
