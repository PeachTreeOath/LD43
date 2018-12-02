using UnityEngine;

public class HandOfGod : MonoBehaviour {
    public enum State {  SNAPPING_TO_MOUSE, FOLLOWING_MOUSE, SNAPPING_TO_CAR, GUIDING_CAR, HOVER_HANDS };

    public float timeToSnap = 0.25f;
    public float maxSpeed = 30f;
    public float epsilon = 0.1f;
    public float eulerAngleZ;

    public State currState;
    public VehicleController vehicleToGuide;

    private static float PLANE_OF_GOD = 0; // on z axis

    private Vector2 velocity;  //TODO might be better as RigidBody2D?

	void Start () {
	}
	
	void Update () {
        eulerAngleZ = transform.eulerAngles.z;
        switch(currState) {
            case State.SNAPPING_TO_MOUSE:
                if( UpdateMovingToPosition(GetMousePosition()) ) {
                    StartFollowingMouse();
                }
                break;

            case State.FOLLOWING_MOUSE:
                LockOnPosition(GetMousePosition());
                break;

            case State.SNAPPING_TO_CAR:
                if(vehicleToGuide != null && UpdateMovingToPosition(vehicleToGuide.transform.position)) {
                    StartGuidingVehicle();
                }
                break;

            case State.GUIDING_CAR:
                if(vehicleToGuide != null) {
                    LockOnPosition(vehicleToGuide.transform.position);
                }
                break;

            case State.HOVER_HANDS:
                //TODO gently drift?
                break;
        }	
	}

    public bool IsGuidingCar {
        get {
            switch(currState) {
                case State.SNAPPING_TO_CAR:
                case State.GUIDING_CAR:
                    return true;
                default:
                    return false;
            }
        }
    }
    
    public bool IsFollowingMouse {
        get {
            switch(currState) {
                case State.SNAPPING_TO_MOUSE:
                case State.FOLLOWING_MOUSE:
                    return true;
                default:
                    return false;
            }
        }
    }

    public void FollowMouse() {
        vehicleToGuide = null;
        currState = State.SNAPPING_TO_MOUSE;
        velocity = Vector2.zero; //TODO inherit from car if already guiding one?
    }

    public void GuideVehicle(VehicleController vehicle) {
        if (currState == State.GUIDING_CAR && vehicleToGuide == vehicle) return;

        currState = State.SNAPPING_TO_CAR;
        velocity = Vector2.zero; //TODO inherit from car if already guiding one?

        vehicleToGuide = vehicle;
    }

    public void Hover() {
        currState = State.HOVER_HANDS;
        velocity = Vector2.zero;

        vehicleToGuide = null;
    }

    protected void StartFollowingMouse() {
        currState = State.FOLLOWING_MOUSE;
    }

    protected void StartGuidingVehicle() {
        currState = State.GUIDING_CAR;
    }

    protected bool UpdateMovingToPosition(Vector3 targetPosition) {
        transform.position = Vector2.SmoothDamp(transform.position, targetPosition, ref velocity, timeToSnap, maxSpeed);

        return Vector2.Distance(transform.position, targetPosition) <= epsilon;
    }

    protected void LockOnPosition(Vector3 position) {
        position.z = PLANE_OF_GOD; // fix bug for jesus hands going into z = -10
        transform.position = position;
    }

    protected Vector3 GetMousePosition() {
        Vector3 mousePos = Input.mousePosition;
        return Camera.main.ScreenToWorldPoint(mousePos);        
    }

}
