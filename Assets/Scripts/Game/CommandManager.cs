using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandManager : MonoBehaviour
{

    private PlayerUnit selectedUnit;
    private bool dragging;
    private float dragDistance;

    // Start is called before the first frame update
    void Start()
    {

    }


    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.TacticalPause)
        {
            if (Input.GetMouseButtonDown(0))
            {
                ProcessPressAction();
            } else if (Input.GetMouseButtonUp(0))
            {
                ProcessReleaseAction();
            }
        } else
        {
            TryDeselectSelectedUnit();
        }
    }

    private void ProcessPressAction()
    {
        RaycastHit rayCastHit = GetMouseRayCastHit();
        GameObject hitGameObject = rayCastHit.transform.gameObject;
        Vector3 targetPosition = new Vector3(rayCastHit.point.x, 0, rayCastHit.point.z);
        if (hitGameObject.TryGetComponent(out PlayerUnit playerUnit))
        { // If mouse touch PlayerUnit select it whatever.
            TryDeselectSelectedUnit();
            playerUnit.Select();
            selectedUnit = playerUnit;
        }
    }

    private void ProcessReleaseAction()
    {
        RaycastHit rayCastHit = GetMouseRayCastHit();
        GameObject hitGameObject = rayCastHit.transform.gameObject;
        Vector3 targetPosition = new Vector3(rayCastHit.point.x, 0, rayCastHit.point.z);

        // If the Unit on which the mouse has been released is the selected one, reset all commands.
        if (hitGameObject.TryGetComponent(out PlayerUnit playerUnit) && playerUnit == selectedUnit)
        {
            CommandSelectedPlayerUnitToStop();
            CancelBallHolderThrow();
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
        Physics.Raycast(raycast, out raycastHit, Mathf.Infinity, LayerMask.GetMask("PlayerUnit", "Terrain"));

        return raycastHit;
    }

    private void TryDeselectSelectedUnit()
    {
        if (selectedUnit != null)
        {
            selectedUnit.Deselect();
            selectedUnit = null;
        }
    }

    

    private void CommandBallHolderToPass(Vector3 passTargetPosition)
    {
        // First find the player holding the ball (if exists)
        PlayerUnit playerUnit = FindPlayerUnitHoldingBall();
        if (playerUnit != null)
        {
            playerUnit.RegisterPassCommand(passTargetPosition);
        }
    }

    private void CommandBallHolderToShoot(Vector3 shootTargetPosition)
    {
        // First find the player holding the ball (if exists)
        PlayerUnit playerUnit = FindPlayerUnitHoldingBall();
        if (playerUnit != null)
        {
            playerUnit.RegisterShootCommand(shootTargetPosition);
        }
    }

    private void CancelBallHolderThrow()
    {
        // First find the player holding the ball (if exists)
        PlayerUnit playerUnit = FindPlayerUnitHoldingBall();
        if (playerUnit != null)
        {
            playerUnit.CancelThrowCommand();
        }
    }


    private PlayerUnit FindPlayerUnitHoldingBall()
    {
        UnitBase unitHoldingBall = Ball.instance.UnitHoldingIt;
        if (unitHoldingBall != null && unitHoldingBall is PlayerUnit)
        {
            return (PlayerUnit) unitHoldingBall;
        }
        return null;
    }

    private void CommandSelectedPlayerUnitToMove(Vector3 targetPosition)
    {
        if (selectedUnit != null)
        {
            selectedUnit.RegisterMoveCommand(targetPosition);
            TryDeselectSelectedUnit();
        }
    }

    private void CommandSelectedPlayerUnitToStop()
    {
        if (selectedUnit != null)
        {
            selectedUnit.RegisterStopCommand();
            TryDeselectSelectedUnit();
        }
    }

}
