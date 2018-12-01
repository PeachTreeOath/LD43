using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleController : MonoBehaviour
{
    public enum State { ENTERING_STAGE, DRIVING, CRASHING, CRASHED }
    public State currState;

    [Tooltip("The pool the vehicle belongs to.")]
    public VehiclePool vehiclePool;

    public bool isSelected; // This is how a user selects which vehicle to move

    public GameObject vehicleSprite; // Contains both sprite and collider and are separate from the actual GameObject

    private Rigidbody2D rbody;

    public float angledMovementPower;

    public float driftPower;

    public VehicleStats vehicleStats;

    //Cap on how fast the car can move on the x-axis per update
    private float maxHSpeedConst = 0.17f;
    private static float WAKE_CONTROL_LOWER_BOUND = 5;
    private static float LANE_CORRECTION_ANGLE_DELTA = 2;

    public bool isSleeping;
    public float nextSleepTime;
    public float nextWakeTime;
    private float timeElapsed;
    private Vector2 sleepVector;

    GameObject lightShaft;

    // Use this for initialization
    void Start()
    {
        currState = State.DRIVING;  // this is temporary...
        rbody = GetComponent<Rigidbody2D>();
        nextSleepTime = GetNextSleepOrWakeTime();
        vehiclePool = GameManager.instance.getVehiclePool();
    }

    // Update is called once per frame
    void Update() {
        switch(currState) {
            case State.DRIVING:
                UpdateDriving();
                break;
        }
    }

    void UpdateDriving() { 
        timeElapsed += Time.deltaTime;
        if (timeElapsed > nextSleepTime && !isSleeping)
        {
            if (isSelected) {
                timeElapsed = 0;
                nextSleepTime = GetNextSleepOrWakeTime();
            } else {
                StartDrifting();
            }
        } else if (isSleeping && timeElapsed > nextWakeTime) 
        {
            StopDrifting();
        }

        float hInput = 0;
        float vInput = 0;
        if (isSelected) // Only apply input if vehicle is selected, otherwise just continue with drift logic alone
        {
            // Feel free to adjust these magic numbers to make the movement feel better, the current
            // numbers are balanced around the default car model
            hInput = vehicleStats.control * 1.5f * Input.GetAxisRaw("Horizontal");
            vInput = vehicleStats.speed * 0.0012f * Input.GetAxisRaw("Vertical");
        }

        // Movement from input
        float rotateDelta;
        if (isSleeping) {
            rotateDelta = (hInput + (sleepVector.x * vehicleStats.sleepSeverity * .2f)) * Time.deltaTime;
            vehicleSprite.transform.Rotate(Vector3.back, rotateDelta);
        } else {
            // North is 0 or 360
            // if less than 10 or more than 350, do nothing
            // if less than 180, reduce, if more than 180, increase
            float angle = vehicleSprite.transform.eulerAngles.z;
            if (angle < 0+WAKE_CONTROL_LOWER_BOUND || angle > 360-WAKE_CONTROL_LOWER_BOUND) { // do nothing
                rotateDelta = 0f;
            } else if (angle < 180) { // reduce angle
                rotateDelta = -1f * LANE_CORRECTION_ANGLE_DELTA;
            } else { // angle > 180
                rotateDelta = LANE_CORRECTION_ANGLE_DELTA;
            }
            vehicleSprite.transform.Rotate(Vector3.forward, rotateDelta);
        }

        // Drift vehicle left/right based on how much rotation applied
        float hDelta = GetHorizontalDeltaFromRotation(vehicleSprite.transform.eulerAngles.z);

        Vector2 newPosition = (Vector2)transform.position 
            + new Vector2(hDelta, (vInput + (sleepVector.y * vehicleStats.sleepSeverity * .01f) * Time.deltaTime));
        rbody.MovePosition(newPosition);
    }

    public void StartDrifting()
    {
        if (currState != State.DRIVING) return;

        isSleeping = true;
        timeElapsed = 0;
        nextWakeTime = GetNextSleepOrWakeTime();
        // Only pick from top 6 drift directions - don't want to drift north or south.
        DirectionEnum driftDirection = (DirectionEnum)UnityEngine.Random.Range(0, 6);
        switch (driftDirection)
        {
            case DirectionEnum.NE:
                sleepVector = new Vector2(1, 1);
                break;
            case DirectionEnum.E:
                sleepVector = new Vector2(1, 0);
                break;
            case DirectionEnum.SE:
                sleepVector = new Vector2(1, -1);
                break;
            case DirectionEnum.NW:
                sleepVector = new Vector2(-1, 1);
                break;
            case DirectionEnum.W:
                sleepVector = new Vector2(-1, 0);
                break;
            case DirectionEnum.SW:
                sleepVector = new Vector2(-1, -1);
                break;
        }
    }

    public void OnCollideWithWalls(Vector2 normal) {
        //TODO 1) Check if the vehicle is "roughly perpendicular" to the wall

        StartFatalCrash();
    }

    private void StartFatalCrash() {
        vehiclePool.SelectVehicle(null);

        currState = State.CRASHING;

        gameObject.layer = LayerMask.NameToLayer("Terrain");

        rbody.bodyType = RigidbodyType2D.Kinematic;
        rbody.angularVelocity = 0f;
        rbody.velocity = new Vector2(0, -GameManager.instance.roadSpeed);


        //TODO crash effect
        //TODO crash sound
        //TODO screen shake
    }

    private void StopDrifting()
    {
        isSleeping = false;
        timeElapsed = 0;
        nextSleepTime = GetNextSleepOrWakeTime();
    }


    private float GetNextSleepOrWakeTime()
    {
        return 8 - vehicleStats.sleepChance * 2 + UnityEngine.Random.Range(-1, 2); // Choose to sleep randomly from 1-7 seconds
    }

    public void CheckSelected(GameObject selected)
    {
        if(selected.GetInstanceID() != gameObject.GetInstanceID())
        {
            Destroy(lightShaft);
        }
    }

    public void OnMouseDown()
    {
        if (!isSelected)
        {
            lightShaft = Instantiate(GameManager.instance.lightShaftsFab) as GameObject;
            lightShaft.transform.position = gameObject.transform.position + Vector3.up * 1;
            lightShaft.transform.SetParent(gameObject.transform);
            vehiclePool.SelectVehicle(this);
            for (int i = 0; i < vehiclePool.vehicles.Count; i++)
            {
                vehiclePool.vehicles[i].CheckSelected(gameObject);
            }
        }
    }

    private float GetHorizontalDeltaFromRotation(float eulerAngle)
    {
        float result = 0;
        if (eulerAngle < 180)
        {
            float angle = 0 - eulerAngle;
            float angleSeverity = Mathf.Pow(angle, 2);
            result = -angleSeverity * angledMovementPower;
            result = Math.Max(result, -maxHSpeedConst);
        }
        else
        {
            float angle = 360 - eulerAngle;
            float angleSeverity = Mathf.Pow(angle, 2);
            result = angleSeverity * angledMovementPower;
            result = Math.Min(result, maxHSpeedConst);
        }
        //Debug.Log(gameObject.name + " hDelta for angle " + eulerAngle + " = " + result);
        return result;
    }
}