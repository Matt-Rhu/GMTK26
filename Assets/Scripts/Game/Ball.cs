using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public static Ball instance;

    public enum BallState { Held, Passed, Shot, Idle};
    [HideInInspector] public BallState currentState;
    
    
    // Shot on clic for debug. SHALL BE FALSE FOR RELEASE.
    public bool debugShotOnClick;
    
    [FoldHeader("Ball Physics")]
    [SerializeField] private float shotForceFactor = 1f;
    [SerializeField] private float speedReductionFactorOnLanding = 2f;
    [SerializeField] private float distanceReductionFactorOnLanding = 1.4f;
    

    private Vector3 velocity = new Vector3(0, 0, 0);
    private Vector3 sourcePosition = new Vector3(0, 0, 0);
    private Vector3 targetPosition = new Vector3(0, 0, 0);


    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        switch(currentState)
        {
            case BallState.Held:
                break;
            case BallState.Passed:
            case BallState.Shot:
                FreeTravel();
                break;
            case BallState.Idle:
                break;
        }
        
        if (debugShotOnClick) // DEBUG ONLY. Freely shoot the ball without selecting any players.
        {
            DebugPassTowardMouseClick();
        }
    }

    private void FreeTravel()
    {
        transform.position += velocity * Time.deltaTime;
        if ((transform.position - targetPosition).magnitude < 0.1)
        {
            Vector3 newSourcePosition = transform.position;
            Vector3 newTargetPosition = newSourcePosition + (targetPosition - sourcePosition) / distanceReductionFactorOnLanding;
            Vector3 newVelocity = (newTargetPosition - newSourcePosition) * shotForceFactor / speedReductionFactorOnLanding;

            sourcePosition = newSourcePosition;
            targetPosition = newTargetPosition;
            velocity = newVelocity;
            if (velocity.magnitude < 1)
            {
                Stop();
            }
        }
    }


    public void Shoot(Vector3 goalPosition)
    {
        //sourcePosition = transform.position;
        //targetPosition = goalPosition;
        //velocity = (targetPosition - sourcePosition) * shotForceFactor;
        //currentState = BallState.Passed;
    }


    public void Pass(Vector3 passTargetPosition)
    {
        sourcePosition = transform.position;
        targetPosition = passTargetPosition;
        velocity = (targetPosition - sourcePosition) * shotForceFactor;
        ChangeState(BallState.Passed);
    }

    public void ChangeState(BallState newState)
    {
        // print(newState);
        currentState = newState;
    }

    public void Stop()
    {
        velocity = Vector3.zero;
        sourcePosition = Vector3.zero;
        targetPosition = Vector3.zero;
        ChangeState(BallState.Idle);
    }

    private void DebugPassTowardMouseClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray raycast = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit raycastHit = new RaycastHit();

            int layerMask = 1 << 8;
            if (Physics.Raycast(raycast, out raycastHit, Mathf.Infinity, layerMask))
            {
                Vector3 passTargetPosition = new Vector3(raycastHit.point.x, 0, raycastHit.point.z);
                Pass(passTargetPosition);
            }
        }
    }
}
