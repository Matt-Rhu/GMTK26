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

    [FoldHeader("Configuration")]
    [SerializeField] private SpriteRenderer ballSprite;
    [SerializeField] private SpriteRenderer ballOutline;
    [FoldHeader("States Colors")]
    [SerializeField] private Color heldColor;
    [SerializeField] private Color passedColor;
    [SerializeField] private Color shotColor;
    [SerializeField] private Color idleColor;
    // Shot on clic for debug. SHALL BE FALSE FOR RELEASE.
    public bool debugShotOnClick;

    [FoldHeader("Ball Physics")]
    public float InitialVelocity = 10f;
    [SerializeField] private float speedReductionFactorOnLanding = 2f;
    [SerializeField] private float distanceReductionFactorOnLanding = 1.4f;


    private float defaultScale = 0f;
    private float realInitialVelocity = 10f;
    private Vector3 velocity = new Vector3(0, 0, 0);
    private Vector3 sourcePosition = new Vector3(0, 0, 0);
    private Vector3 targetPosition = new Vector3(0, 0, 0);


    private void Awake()
    {
        defaultScale = ballSprite.transform.localScale.x;
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
        UpdateYDepthBasedOnScale();
        ReColorOutline();
        switch (CurrentState)
        {
            case BallState.Held:
                DriveTravel();
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

    private void UpdateYDepthBasedOnScale()
    {
        if (ballSprite.transform.localScale.x > 5)
        {
            ballSprite.transform.position = new Vector3(ballSprite.transform.position.x, 1.0f, ballSprite.transform.position.z);
            return;
        }
        ballSprite.transform.position = new Vector3(ballSprite.transform.position.x, 0f, ballSprite.transform.position.z);
    }
    private void ReColorOutline()
    {
        Color newColor;
        switch (CurrentState)
        {
            case BallState.Held:
                newColor = heldColor;
                break;
            case BallState.Passed:
                newColor = passedColor;
                break;
            case BallState.Shot:
                newColor = shotColor;
                break;
            case BallState.Idle:
            default:
                newColor = idleColor;
                break;
        }
        ballOutline.color = newColor;
    }
    private void DriveTravel()
    {
        float parabolProgress = 1.0f - ParabolicInterpolation(Mathf.Repeat(Time.realtimeSinceStartup * 4.0f, 1.0f));
        ballSprite.transform.localScale = Vector3.one * (defaultScale - parabolProgress * defaultScale / 3.0f);
    }

    private void FreeTravel()
    {
        // Update ball position
        transform.position += velocity * Time.deltaTime;
        // Update sprite size based on progress before next bounce.
        float throwFactor = 1.0f;
        if (CurrentState == BallState.Shot)
        {
            throwFactor = 1.25f;
        }
        float parabolProgress = ParabolicInterpolation(Vector3.Distance(sourcePosition, transform.position) / Vector3.Distance(sourcePosition, targetPosition));

        ballSprite.transform.localScale = Vector3.one * (defaultScale + parabolProgress * defaultScale * (velocity.magnitude/realInitialVelocity) * throwFactor);

        
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

    private float ParabolicInterpolation(float x)
    {
        if (x < 0) { return 0; }
        if (x > 1) { return 1; }

        return 4 * x * (1 - x);
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
        ballSprite.transform.localScale = Vector3.one * defaultScale;
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
