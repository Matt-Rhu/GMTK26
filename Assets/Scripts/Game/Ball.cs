using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public static Ball instance;
    

    public enum BallState { Held, Passed, Shot, Idle};

    public BallState currentState;
    // Ball physics parameters
    public BallPhysics ballPhysicsParameters;
    [System.Serializable]
    public class BallPhysics
    {
        public float shotForceFactor = 1f;
        public float speedReductionFactorOnLanding = 2f;
        public float distanceReductionFactorOnLanding = 1.4f;
    }


    private Vector3 velocity = new Vector3(0, 0, 0);
    private Vector3 sourcePosition = new Vector3(0, 0, 0);
    private Vector3 targetPosition = new Vector3(0, 0, 0);

    // Shot on clic for debug. SHALL BE FALSE FOR RELEASE.
    public bool debugShotOnClick = false;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {

    }

    void Update()
    {
        switch(currentState)
        {
            case BallState.Held:
                break;
            case BallState.Passed:
            case BallState.Shot:
                freeTravel();
                break;
            case BallState.Idle:
                break;
        }

        
        if (debugShotOnClick) // DEBUG ONLY. Freely shoot the ball without selecting any players.
        {
            debugPassTowardMouseClick();
        }
        
    }

    private void freeTravel()
    {
        transform.position += velocity * Time.deltaTime;
        if ((transform.position - targetPosition).magnitude < 0.1)
        {
            Vector3 newSourcePosition = transform.position;
            Vector3 newTargetPosition = newSourcePosition + (targetPosition - sourcePosition) / ballPhysicsParameters.distanceReductionFactorOnLanding;
            Vector3 newVelocity = (newTargetPosition - newSourcePosition) * ballPhysicsParameters.shotForceFactor / ballPhysicsParameters.speedReductionFactorOnLanding;

            sourcePosition = newSourcePosition;
            targetPosition = newTargetPosition;
            velocity = newVelocity;
            if (velocity.magnitude < 1)
            {
                stop();
            }
        }
    }


    public void shoot(Vector3 goalPosition)
    {
        //sourcePosition = transform.position;
        //targetPosition = goalPosition;
        //velocity = (targetPosition - sourcePosition) * shotForceFactor;
        //currentState = BallState.Passed;
    }


    public void pass(Vector3 passTargetPosition)
    {
        sourcePosition = transform.position;
        targetPosition = passTargetPosition;
        velocity = (targetPosition - sourcePosition) * ballPhysicsParameters.shotForceFactor;
        changeState(BallState.Passed);
    }

    public void changeState(BallState newState)
    {
        // print(newState);
        currentState = newState;
    }

    public void stop()
    {
        velocity = Vector3.zero;
        sourcePosition = Vector3.zero;
        targetPosition = Vector3.zero;
        changeState(BallState.Idle);
    }

    private void debugPassTowardMouseClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray raycast = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit raycastHit = new RaycastHit();

            int layerMask = 1 << 8;
            if (Physics.Raycast(raycast, out raycastHit, Mathf.Infinity, layerMask))
            {
                Vector3 passTargetPosition = new Vector3(raycastHit.point.x, 0, raycastHit.point.z);
                pass(passTargetPosition);
            }
        }
    }
}
