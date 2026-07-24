using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandManager : MonoBehaviour
{
    private PlayerUnit selectedUnit;
    private bool dragging;
    private float dragDistance;


    private void Awake()
    {
        Inputs.Gameplay.Select.started += _ => ProcessPressAction();
        Inputs.Gameplay.Select.canceled += _ => ProcessReleaseAction();

        Inputs.Gameplay.ToggleTacticalPause.performed += _ => TryDeselectSelectedUnit();
    }

    private void Update()
    {
        if (!dragging) return;
        
        RaycastHit rayCastHit = GetMouseRayCastHit();
        Vector3 targetPosition = new Vector3(rayCastHit.point.x, 0, rayCastHit.point.z);
        
        if (selectedUnit)
            selectedUnit.SetTarget(targetPosition);
    }

    private void ProcessPressAction()
    {
        if (!GameManager.instance.TacticalPause) return;
        
        dragging = true;
        RaycastHit rayCastHit = GetMouseRayCastHit();
        GameObject hitGameObject = rayCastHit.transform.gameObject;
        Vector3 targetPosition = new Vector3(rayCastHit.point.x, 0, rayCastHit.point.z);
        if (hitGameObject.TryGetComponent(out PlayerUnit playerUnit))
        { // If mouse touch PlayerUnit select it whatever.
            playerUnit.Select();
            selectedUnit = playerUnit;
        }
    }

    private void ProcessReleaseAction()
    {
        if (!GameManager.instance.TacticalPause) return;
        
        dragging = false;
        RaycastHit rayCastHit = GetMouseRayCastHit();
        GameObject hitGameObject = rayCastHit.transform.gameObject;
        Vector3 targetPosition = new Vector3(rayCastHit.point.x, 0, rayCastHit.point.z);

        // If the Unit on which the mouse has been released is the selected one, reset all commands.
        if (hitGameObject.TryGetComponent(out PlayerUnit playerUnit) && playerUnit == selectedUnit)
        {
            CommandSelectedPlayerUnitToStop();
            CancelBallHolderThrow();
        } else if (hitGameObject.TryGetComponent(out Goal goal))
        {
            if (selectedUnit == null)
            {
                CommandBallHolderToShoot(targetPosition);
            }
        } else // Otherwise process the commands as normal.
        {
            if (selectedUnit != null)
            {
                CommandSelectedPlayerUnitToMove(targetPosition);
            }
            else
            {
                CommandBallHolderToPass(targetPosition);
            }
        }
        TryDeselectSelectedUnit();
    }

    private RaycastHit GetMouseRayCastHit()
    {
        Ray raycast = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit raycastHit = new RaycastHit();
        Physics.Raycast(raycast, out raycastHit, Mathf.Infinity, LayerMask.GetMask("PlayerUnit", "Terrain", "Goal"));

        return raycastHit;
    }

    private void TryDeselectSelectedUnit()
    {
        if (selectedUnit)
        {
            selectedUnit.Deselect();
            selectedUnit = null;
        }
    }

    

    private void CommandBallHolderToPass(Vector3 passTargetPosition)
    {
        // First find the player holding the ball (if exists)
        PlayerUnit playerUnit = FindPlayerUnitHoldingBall();
        if (playerUnit)
        {
            playerUnit.RegisterPassCommand(passTargetPosition);
        }
    }

    private void CommandBallHolderToShoot(Vector3 shootTargetPosition)
    {
        // First find the player holding the ball (if exists)
        PlayerUnit playerUnit = FindPlayerUnitHoldingBall();
        if (playerUnit)
        {
            playerUnit.RegisterShootCommand(shootTargetPosition);
        }
    }

    private void CancelBallHolderThrow()
    {
        // First find the player holding the ball (if exists)
        PlayerUnit playerUnit = FindPlayerUnitHoldingBall();
        if (playerUnit)
        {
            playerUnit.CancelThrowCommand();
        }
    }


    private PlayerUnit FindPlayerUnitHoldingBall()
    {
        UnitBase unitHoldingBall = Ball.instance.UnitHoldingIt;
        if (unitHoldingBall && unitHoldingBall is PlayerUnit unit)
        {
            return unit;
        }
        return null;
    }

    private void CommandSelectedPlayerUnitToMove(Vector3 targetPosition)
    {
        if (selectedUnit)
        {
            selectedUnit.SetTarget(targetPosition);
            TryDeselectSelectedUnit();
        }
    }

    private void CommandSelectedPlayerUnitToStop()
    {
        if (selectedUnit)
        {
            selectedUnit.SetTarget(selectedUnit.transform.position);
            TryDeselectSelectedUnit();
        }
    }

}
