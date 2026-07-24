using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandManager : MonoBehaviour
{

    public List<PlayerUnit> players = new List<PlayerUnit>();
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
        } else
        {
            CommandBallHolderToShoot(targetPosition);
        }
    }

    private void ProcessReleaseAction()
    {
        RaycastHit rayCastHit = GetMouseRayCastHit();
        Vector3 targetPosition = new Vector3(rayCastHit.point.x, 0, rayCastHit.point.z);
        if (selectedUnit != null)
        {
            CommandSelectedPlayerUnitToMove(targetPosition);
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

    private void CommandBallHolderToShoot(Vector3 shotPosition)
    {
        // First find the player holding the ball (if exists)
        foreach (PlayerUnit playerUnit in players)
        {
            if (playerUnit.holdingBall)
            {
                playerUnit.RegisterPassCommand(shotPosition);
            }
        }
    }

    private void CommandSelectedPlayerUnitToMove(Vector3 targetPosition)
    {
        if (selectedUnit != null)
        {
            selectedUnit.RegisterMoveCommand(targetPosition);
            TryDeselectSelectedUnit();
        }
    }

    private void CommandSelectedPlayerUnitToDoNothing()
    {
        if (selectedUnit != null)
        {
            selectedUnit.RegisterStopCommand();
            TryDeselectSelectedUnit();
        }
    }

}
