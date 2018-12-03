using UnityEngine;

public class HandOfGod : MonoBehaviour
{
    public enum State { SNAPPING_TO_MOUSE, FOLLOWING_MOUSE, SNAPPING_TO_CAR, GUIDING_CAR, HOVER_HANDS };

    public bool isRightHand;
    public float timeToSnap = 0.25f;
    public float maxSpeed = 30f;
    public float epsilon = 0.1f;

    public State currState;
    public VehicleController vehicleToGuide;

    private static float NEUTRAL_HAND_ANGLE = 45;
    private static float HIGH_HAND_ANGLE = 70;
    private static float LOW_HAND_ANGLE = 20;
    private static float MIN_ANGLE_THRESHOLD = 5; // in degrees
    private static float FINGER_LENGTH = 0.6f;

    private SpriteRenderer spriteRenderer;


    private Vector2 velocity;  //TODO might be better as RigidBody2D?
    public Vector3 targetHandAngle;

    void Start()
    {
        if (isRightHand)
        {
            transform.Rotate(new Vector3(0, 0, -1f * NEUTRAL_HAND_ANGLE));
        }
        else
        {
            transform.Rotate(new Vector3(0, 0, NEUTRAL_HAND_ANGLE));
        }

        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        switch (currState)
        {
            case State.SNAPPING_TO_MOUSE:
                if (UpdateMovingToPosition(GetMousePosition()))
                {
                    StartFollowingMouse();
                }
                break;

            case State.FOLLOWING_MOUSE:
                LockOnPosition(GetMousePosition());
                break;

            case State.SNAPPING_TO_CAR:
                if (vehicleToGuide != null && UpdateMovingToPosition(vehicleToGuide.transform.position))
                {
                    StartGuidingVehicle();
                }
                break;

            case State.GUIDING_CAR:
                if (vehicleToGuide != null)
                {
                    LockOnPosition(vehicleToGuide.transform.position);
                }
                break;

            case State.HOVER_HANDS:
                //TODO gently drift?
                break;
        }
        UpdateHandAngle();
    }

    public bool IsGuidingCar
    {
        get
        {
            switch (currState)
            {
                case State.SNAPPING_TO_CAR:
                case State.GUIDING_CAR:
                    return true;
                default:
                    return false;
            }
        }
    }

    public bool IsFollowingMouse
    {
        get
        {
            switch (currState)
            {
                case State.SNAPPING_TO_MOUSE:
                case State.FOLLOWING_MOUSE:
                    return true;
                default:
                    return false;
            }
        }
    }

    public void FollowMouse()
    {
        vehicleToGuide = null;
        currState = State.SNAPPING_TO_MOUSE;
        velocity = Vector2.zero; //TODO inherit from car if already guiding one?
    }

    public void GuideVehicle(VehicleController vehicle)
    {
        if (currState == State.GUIDING_CAR && vehicleToGuide == vehicle) return;

        currState = State.SNAPPING_TO_CAR;
        velocity = Vector2.zero; //TODO inherit from car if already guiding one?

        vehicleToGuide = vehicle;
    }

    public void Hover()
    {
        currState = State.HOVER_HANDS;
        velocity = Vector2.zero;

        vehicleToGuide = null;
    }

    protected void StartFollowingMouse()
    {
        currState = State.FOLLOWING_MOUSE;
    }

    protected void StartGuidingVehicle()
    {
        currState = State.GUIDING_CAR;
    }

    protected bool UpdateMovingToPosition(Vector3 targetPosition)
    {
        Vector3 translatedTargetPosition = TranslatePoint(targetPosition, transform.rotation.eulerAngles, -1f * FINGER_LENGTH);
        Vector2 position = Vector2.SmoothDamp(transform.position, translatedTargetPosition, ref velocity, timeToSnap, maxSpeed);
        transform.position = new Vector3(position.x, position.y, transform.position.z);

        return Vector2.Distance(transform.position, translatedTargetPosition) <= epsilon;
    }

    protected void LockOnPosition(Vector3 position)
    {
        Vector3 newPosition = new Vector3(position.x, position.y, transform.position.z);
        transform.position = TranslatePoint(newPosition, transform.rotation.eulerAngles, -1f * FINGER_LENGTH);
    }

    protected Vector3 GetMousePosition()
    {
        Vector3 mousePos = Input.mousePosition;
        return Camera.main.ScreenToWorldPoint(mousePos);
    }

    private void UpdateHandAngle()
    {
        if (IsGuidingCar)
        {
            // Set target hand angle
            float hInput = Input.GetAxisRaw("Horizontal");
            if (hInput > 0) // turning right
            {
                targetHandAngle = new Vector3(0, 0, isRightHand ? 360f - HIGH_HAND_ANGLE : LOW_HAND_ANGLE);
            }
            else if (hInput < 0)
            {
                targetHandAngle = new Vector3(0, 0, isRightHand ? 360f - LOW_HAND_ANGLE : HIGH_HAND_ANGLE);
            }
            else
            {
                ResetHandAngle();
            }
        } else
        {
            ResetHandAngle();
        }

        // Rotate to target angle
        float angleDifference = targetHandAngle.z - transform.rotation.eulerAngles.z;
        if (angleDifference == 0)
        {
            return;
        }
        else if (Mathf.Abs(angleDifference) < MIN_ANGLE_THRESHOLD)
        {
            transform.Rotate(new Vector3(0, 0, angleDifference));
        }
        else
        {
            transform.Rotate(new Vector3(0, 0, angleDifference * 0.5f));
        }
    }

    private void ResetHandAngle()
    {
        targetHandAngle = new Vector3(0, 0, isRightHand ? 360f - NEUTRAL_HAND_ANGLE : NEUTRAL_HAND_ANGLE);
    }

    public void SetHandTransparency(bool isTransparent)
    {
        spriteRenderer.color = isTransparent ? new Color(1, 1, 1, 0.5f) : new Color(1, 1, 1, 1);
    }

    private Vector3 TranslatePoint(Vector3 originPoint, Vector3 eulerAngles, float distance)
    {
        float angleRadians = eulerAngles.z * Mathf.Deg2Rad; // 0 is north, counterclockwise
        float dx = -1f * distance * Mathf.Sin(angleRadians);
        float dy = distance * Mathf.Cos(angleRadians);
        return new Vector3(originPoint.x + dx, originPoint.y + dy, originPoint.z);
    }
}