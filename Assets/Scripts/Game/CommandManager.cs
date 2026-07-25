using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandManager : MonoBehaviour
{

    // private enum ControlsMode
    // {
    //     LEFT_CLICK_ONLY,
    //     BOTH_CLICKS
    // }
    //
    // [SerializeField] private ControlsMode controlsMode = ControlsMode.LEFT_CLICK_ONLY;
    [SerializeField] private float MouseDragStartDistanceThreshold = 32f;

    private PlayerUnit selectedUnit;
    private bool dragging;
    private Vector3 potentialDraggingMouseStart = new Vector3(-9999, -9999, -9999);
    private Vector3 lastHoveredTargetPosition = new Vector3(0, 0, 0);
    private GameObject lastHoveredGameObject = null;

    private void Awake()
    {
        Inputs.Gameplay.Select.started += _ => ProcessPressAction();
        Inputs.Gameplay.Select.canceled += _ => ProcessReleaseAction();

        Inputs.Gameplay.ToggleTacticalPause.performed += _ => TryDeselectSelectedUnit();
    }

    private void Update()
    {
        if (!GameManager.instance.TacticalPause) return;

        RaycastHit rayCastHit = GetMouseRayCastHit();
        if (rayCastHit.transform)
        {
            lastHoveredTargetPosition = new Vector3(rayCastHit.point.x, 0, rayCastHit.point.z);
            lastHoveredGameObject = rayCastHit.transform.gameObject;
        }
        
        // In tactical pause, always highlight when hovering a player. If no action has been performed.
        if (potentialDraggingMouseStart.x <= -9999 && !dragging)
        {
            if (lastHoveredGameObject && lastHoveredGameObject.TryGetComponent(out PlayerUnit playerUnit))
            {
                selectedUnit = playerUnit;
                playerUnit.ShowHighlight();
            } else
            {
                TryDeselectSelectedUnit();
            }
        }
        // If mouse have been pressed once but still not dragging, infer if action intended is dragging based on drag start threshold.
        if (potentialDraggingMouseStart.x > -9999 && !dragging)
        {
            if ((Input.mousePosition - potentialDraggingMouseStart).magnitude >= MouseDragStartDistanceThreshold)
            {
                dragging = true;
            }
        }

        if (!dragging) return;
  

        
        
        if (selectedUnit)
            selectedUnit.SetTarget(lastHoveredTargetPosition);
    }

    private void ProcessPressAction()
    {
        if (!GameManager.instance.TacticalPause) return;

        // On press, keep mouse position in momery to resolve if it is a drag or a simple point click.
        potentialDraggingMouseStart = Input.mousePosition;
    }

    private void ProcessReleaseAction()
    {
        if (!GameManager.instance.TacticalPause) return;

        SelectAndRegisterCommand();
        // Anyway reset dragging & try deselect unit at the end.
        dragging = false;
        potentialDraggingMouseStart = new Vector3(-9999, -9999, -9999);
        TryDeselectSelectedUnit();
    }


    private void SelectAndRegisterCommand()
    {
        //RaycastHit rayCastHit = GetMouseRayCastHit();
        /*GameObject hitGameObject = rayCastHit.transform.gameObject;
        Vector3 targetPosition = new Vector3(rayCastHit.point.x, 0, rayCastHit.point.z);*/

        PlayerUnit playerUnit = null;
        
        // Get release target.
        if (lastHoveredGameObject)
            lastHoveredGameObject.TryGetComponent(out playerUnit);

        // If not dragging, try to perform a throw anyway.
        if (!dragging)
        {
            // 
            if (playerUnit && playerUnit == Ball.instance.UnitHoldingIt)
            {
                CancelBallHolderThrow();
                return;
            }

            // If the target is a goal, try to perform a Shoot.
            if (lastHoveredGameObject)
            {
                if (lastHoveredGameObject.TryGetComponent(out Goal goal)) // If target is the goal then its a Shoot.
                {
                    CommandBallHolderToShoot(lastHoveredTargetPosition);
                    return;
                }
            }

            // Otherwise try to perform a Pass.
            CommandBallHolderToPass(lastHoveredTargetPosition);
            return;
        }


        // Else if dragging and unit is selected, always perform a move command.
        if (dragging && selectedUnit)
        {
            // If playerUnit is selectedUnit, cancel its move command.
            if (playerUnit && playerUnit == selectedUnit)
            {
                CommandSelectedPlayerUnitToStop();
                return;
            }
            CommandSelectedPlayerUnitToMove(lastHoveredTargetPosition);
            return;
        }

        // In anycase, if releasing action on the selected playerUnit, cancel its commands.
        if (playerUnit && playerUnit == selectedUnit)
        {
            
            
        }

        
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
            selectedUnit.HideHightlight();
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
