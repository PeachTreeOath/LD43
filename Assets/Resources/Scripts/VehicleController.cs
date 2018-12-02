﻿using System;
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

    public VehicleBody vehicleBody; // Contains both sprite and collider and are separate from the actual GameObject

    private Rigidbody2D rbody;

    public float angledMovementPower;

    public float driftPower;

    public VehicleStats vehicleStats;

    public ParticleSystem sparkEmitter;
    public ParticleSystem fireballEmitter;

    //Cap on how fast the car can move on the x-axis per update
    private bool initialized = false;
    private float maxHSpeedConst = 0.17f;
    private static float WAKE_CONTROL_LOWER_BOUND = 5;
    private static float LANE_CORRECTION_ANGLE_DELTA = 2;
    private static float WAKE_TIME = 1f; // 1 second before waking

    public bool isSleeping;
    public float nextSleepTime;
    public float nextWakeTime;
    private float timeElapsed;
    private Vector2 sleepVector;

    GameObject lightShaft;
    GameObject caption;

    private JesusFace face;

    public bool IsCrashed
    {
        get
        {
            switch (currState)
            {
                case State.CRASHING:
                case State.CRASHED:
                    return true;
                default:
                    return false;
            }
        }
    }

    // Use this for initialization
    void Start()
    {
        initialized = true;
        currState = State.DRIVING;  // this is temporary...
        rbody = GetComponent<Rigidbody2D>();
        nextSleepTime = GetNextSleepOrWakeTime();
        nextWakeTime = float.PositiveInfinity;
        vehiclePool = GameManager.instance.getVehiclePool();
        face = GameObject.Find("JesusBody").GetComponent<JesusFace>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (currState)
        {
            case State.DRIVING:
                UpdateDriving();
                break;
        }
    }

    void UpdateDriving()
    {
        timeElapsed += Time.deltaTime;
        if (timeElapsed > nextSleepTime && !isSleeping)
        {
            onDriverSleep();
            if (isSelected)
            {
                resetWakeTime();
            }
        }
        else if (isSleeping && timeElapsed > nextWakeTime)
        {
            if (isSelected)
            {
                onDriverWake();
            }
        }

        float hInput = 0;
        float vInput = 0;
        if (isSelected) // Only apply input if vehicle is selected, otherwise just continue with drift logic alone
        {
            // Feel free to adjust these magic numbers to make the movement feel better, the current
            // numbers are balanced around the default car model
            hInput = vehicleStats.control * 3f * Input.GetAxisRaw("Horizontal");
            vInput = vehicleStats.speed * 0.0024f * Input.GetAxisRaw("Vertical");
        }

        // Movement from input
        float rotateDelta;
        if (isSelected)
        {
            rotateDelta = (hInput + (sleepVector.x * vehicleStats.sleepSeverity * .82f)) * Time.deltaTime;
            vehicleBody.transform.Rotate(Vector3.back, rotateDelta);
        }
        else if (!isSleeping)
        {
            // North is 0 or 360
            // if less than 10 or more than 350, do nothing
            // if less than 180, reduce, if more than 180, increase
            float angle = vehicleBody.transform.eulerAngles.z;
            if (angle < 0 + WAKE_CONTROL_LOWER_BOUND || angle > 360 - WAKE_CONTROL_LOWER_BOUND)
            { // do nothing
                rotateDelta = 0f;
            }
            else if (angle < 180)
            { // reduce angle
                rotateDelta = -1f * LANE_CORRECTION_ANGLE_DELTA;
            }
            else
            { // angle > 180
                rotateDelta = LANE_CORRECTION_ANGLE_DELTA;
            }
            vehicleBody.transform.Rotate(Vector3.forward, rotateDelta);
        }
        else
        {
            rotateDelta = (hInput + (sleepVector.x * vehicleStats.sleepSeverity * .2f)) * Time.deltaTime;
            vehicleBody.transform.Rotate(Vector3.back, rotateDelta);
        }

        // Drift vehicle left/right based on how much rotation applied
        float hDelta = GetHorizontalDeltaFromRotation(vehicleBody.transform.eulerAngles.z);

        Vector2 newPosition = (Vector2)transform.position
            + new Vector2(hDelta, (vInput + (sleepVector.y * vehicleStats.sleepSeverity * .01f) * Time.deltaTime));
        rbody.MovePosition(newPosition);
    }

    public void StartDrifting()
    {
        if (currState != State.DRIVING) return;

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

    public void OnCollideWithWalls(CollisionInfo info)
    {
        if (!initialized || !enabled) return;

        //TODO 1) Check if the vehicle is "roughly perpendicular" to the wall

        //var angle = vehicleSprite.transform.eulerAngles.z;
        //var carFacingDir = Quaternion.AngleAxis(angle, Vector3.forward) * Vector2.up;
        switch (currState)
        {
            case State.DRIVING:
                if (IsHeadOnCrash(info.normal))
                {
                    StartFatalCrash();
                }
                else if (IsAtCrashSpeed())
                {
                    StartSpinningCrash(info);
                }
                else
                {
                    StartSideSwipeSwerve(info);
                }
                break;

            case State.CRASHING:
                StartFatalCrash();
                break;
        }

    }

    private bool IsHeadOnCrash(Vector2 normal)
    {
        var headOnCrashPercentage = Math.Abs(Vector2.Dot(normal, vehicleBody.transform.up));
        return (1 - headOnCrashPercentage) <= LevelManager.instance.headOnCrashThreshold;
    }

    private bool IsAtCrashSpeed()
    {
        //TODO use vehicle stats to determine if this is a crashing speed!
        return true;
    }

    private void StartFatalCrash()
    {
        vehiclePool.OnVehicleCrash(this);
        currState = State.CRASHED;

        gameObject.layer = LayerMask.NameToLayer("Terrain");

        rbody.bodyType = RigidbodyType2D.Kinematic;
        rbody.angularVelocity = 0f;
        rbody.velocity = new Vector2(0, -LevelManager.instance.scrollSpeed);
        //Debug.Log("Set crash velocity for " + gameObject.name + " enabled: " + enabled);

        face.GotoWinceFace();

        fireballEmitter.gameObject.AddComponent<ObjectFollower>().target = gameObject.transform;
        fireballEmitter.Play();

        //TODO crash effect
        //TODO crash sound
        //TODO screen shake
    }

    private void StartSpinningCrash(CollisionInfo collisionInfo)
    {
        vehiclePool.OnVehicleCrash(this);
        currState = State.CRASHING;
        face.GotoWinceFace();

        rbody.drag = 0.2f;
        rbody.constraints = RigidbodyConstraints2D.None;

        rbody.angularVelocity = collisionInfo.impulse.magnitude * 300f;
        rbody.angularVelocity = Mathf.Clamp(rbody.angularVelocity, -700, 700);
        rbody.velocity = (collisionInfo.normal * 5f) + new Vector2(0, -LevelManager.instance.scrollSpeed * .05f);

        fireballEmitter.gameObject.AddComponent<ObjectFollower>().target = gameObject.transform;
        fireballEmitter.Play();
        //rbody.velocity = new Vector2(0, -LevelManager.instance.scrollSpeed);
        //Debug.Log("Set crash velocity for " + gameObject.name + " enabled: " + enabled);
    }

    private void StartSideSwipeSwerve(CollisionInfo collisionInfo)
    {
        sparkEmitter.Play();
    }

    private void StopDrifting()
    {
        Destroy(caption);
    }

    private void onDriverSleep()
    {
        isSleeping = true;
        RenderSleepCaption();
        StartDrifting();
    }

    private void resetWakeTime()
    {
        // Schedule next wake
        timeElapsed = 0;
        nextWakeTime = WAKE_TIME;
    }

    private void onDriverWake()
    {
        isSleeping = false;
        Destroy(caption);
        resetSleepTime();
    }

    private void resetSleepTime()
    {
        // Schedule next wake
        timeElapsed = 0;
        nextSleepTime = GetNextSleepOrWakeTime();
    }

    private float GetNextSleepOrWakeTime()
    {
        float ret = 8 - vehicleStats.sleepChance * 2 + UnityEngine.Random.Range(-1, 2); // Choose to sleep randomly from 1-7 seconds
        //Debug.Log("GetNextSleepOrWakeTime: " + ret);
        return ret;
    }

    public void SetSelected(bool selected)
    {
        if (selected == isSelected) return;

        isSelected = selected;
        if (isSelected)
        {
            AddLight();
        }
        else
        {
            RemoveLight();
        }
    }

    private void AddLight()
    {
        if (lightShaft != null) return;

        lightShaft = Instantiate(GameManager.instance.lightShaftsFab) as GameObject;
        lightShaft.transform.position = gameObject.transform.position + Vector3.up * 1;
        lightShaft.AddComponent<ObjectFollower>().target = gameObject.transform;
    }

    private void RemoveLight()
    {
        if (lightShaft != null)
        {
            Destroy(lightShaft);
        }
    }

    public void OnMouseDown()
    {
        if (!IsCrashed)
        {
            vehiclePool.SelectVehicle(this);
            resetWakeTime();
            //Debug.Log(Time.time  + " reset timeElapsed nextSleepTime " + nextWakeTime);
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

    private void RenderSleepCaption()
    {
        //Render Sleep Caption
        caption = Instantiate(ResourceLoader.instance.vehicleSleepCaption, vehicleBody.transform.position,
            Quaternion.identity, vehicleBody.transform);
    }
}