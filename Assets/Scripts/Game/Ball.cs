using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public static Ball instance;

    public enum BallState { Held, Passed, Shot, Idle };
    public BallState CurrentState { get; private set; } = BallState.Idle;

    [HideInInspector] public int BallScore;
    [HideInInspector] public UnitBase UnitHoldingIt;

    // Shot on clic for debug. SHALL BE FALSE FOR RELEASE.
    public bool debugShotOnClick;

    [FoldHeader("Ball Physics")]
    public float InitialVelocity = 10f;
    [SerializeField] private float speedReductionFactorOnLanding = 2f;
    [SerializeField] private float distanceReductionFactorOnLanding = 1.4f;


    private float realInitialVelocity = 10f;
    private Vector3 velocity = new Vector3(0, 0, 0);
    private Vector3 sourcePosition = new Vector3(0, 0, 0);
    private Vector3 targetPosition = new Vector3(0, 0, 0);


    private void Awake()
    {
        realInitialVelocity = 3 * InitialVelocity;
        instance = this;
    }

    private void Update()
    {
        if (!GameManager.instance.GameStarted) return;

        if (GameManager.instance.OutOfTime) return;

        if (GameManager.instance.TacticalPause) return;

        ActiveBehaviour();
    }

    private void ActiveBehaviour()
    {
        switch (CurrentState)
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
            Vector3 newVelocity = (newTargetPosition - newSourcePosition).normalized * velocity.magnitude / speedReductionFactorOnLanding;

            sourcePosition = newSourcePosition;
            targetPosition = newTargetPosition;
            velocity = newVelocity;
            if (velocity.magnitude < 1)
            {
                Stop();
            }
        }
    }


    public void Shoot(Vector3 goalPosition, int score)
    {
        Release();
        sourcePosition = transform.position;
        targetPosition = goalPosition;
        velocity = (targetPosition - sourcePosition).normalized * realInitialVelocity;
        ChangeState(BallState.Shot);

        BallScore = score;
    }


    public void Pass(Vector3 passTargetPosition)
    {
        Release();
        sourcePosition = transform.position;
        targetPosition = passTargetPosition;
        velocity = (targetPosition - sourcePosition).normalized * realInitialVelocity;
        ChangeState(BallState.Passed);
    }

    public void ChangeState(BallState newState)
    {
        // print(newState);
        CurrentState = newState;
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

            int layerMask = 1 << 8;
            if (Physics.Raycast(raycast, out var raycastHit, Mathf.Infinity, layerMask))
            {
                Vector3 passTargetPosition = new Vector3(raycastHit.point.x, 0, raycastHit.point.z);
                Pass(passTargetPosition);
            }

            BallScore = 3;
        }
    }

    public void Grab(UnitBase unit)
    {
        UnitHoldingIt = unit;
        UnitHoldingIt.hasBall = true;
        Ball.instance.ChangeState(Ball.BallState.Held);
    }

    private void Release()
    {
        if (!UnitHoldingIt) return;
        UnitHoldingIt.hasBall = false;
        UnitHoldingIt = null;
    }

    public Vector3 Velocity()
    {
        return velocity;
    }
}
