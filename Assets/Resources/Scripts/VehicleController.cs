﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleController : MonoBehaviour
{
    public int id;

    [Tooltip("The pool the car belongs to.")]
    public VehiclePool vehiclePool;

    public bool isSelected; // This is how a user selects which car to move

    public GameObject vehicleSprite; // Contains both sprite and collider and are separate from the actual GameObject

    private Rigidbody2D rbody;

    public float angledMovementPower;

    public float driftPower;

    public VehicleStats vehicleStats;

    private bool isSleeping;
    private float nextSleepTime;
    private float sleepTimeElapsed;
    private Vector2 sleepVector;

    // Use this for initialization
    void Start()
    {
        rbody = GetComponent<Rigidbody2D>();
        nextSleepTime = GetNextSleepTime();
        vehiclePool = GetComponentInParent<VehiclePool>();
    }

    // Update is called once per frame
    void Update()
    {
        sleepTimeElapsed += Time.deltaTime;
        if (sleepTimeElapsed > nextSleepTime && !isSleeping)
        {
            StartDrifting();
        }

        float hInput = 0;
        float vInput = 0;
        if (isSelected) // Only apply input if car is selected, otherwise just continue with drift logic alone
        {
            // Feel free to adjust these magic numbers to make the movement feel better, the current
            // numbers are balanced around the default car model
            hInput = vehicleStats.control * 1.5f * Input.GetAxisRaw("Horizontal");
            vInput = vehicleStats.speed * 0.0012f * Input.GetAxisRaw("Vertical");
        }

        // Movement from input
        float rotateDelta = (hInput + (sleepVector.x * vehicleStats.sleepSeverity)) * Time.deltaTime;
        vehicleSprite.transform.Rotate(Vector3.back, rotateDelta);

        // Drift car left/right based on how much rotation applied
        float hDelta = GetHorizontalDeltaFromRotation(vehicleSprite.transform.eulerAngles.z);

        Vector2 newPosition = (Vector2)transform.position + new Vector2(hDelta, (vInput + (sleepVector.y * vehicleStats.sleepSeverity * .01f) * Time.deltaTime));
        rbody.MovePosition(newPosition);
    }

    public void StartDrifting()
    {
        isSleeping = true;
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

    private void StopDrifting()
    {
        isSleeping = false;
        sleepTimeElapsed = 0;
        nextSleepTime = GetNextSleepTime();
    }

    private float GetNextSleepTime()
    {
        return 8 - vehicleStats.sleepChance * 2 + UnityEngine.Random.Range(-1, 2); // Choose to sleep randomly from 1-7 seconds
    }

    public void OnMouseDown()
    {
        vehiclePool.SelectCar(id);
    }

    private float GetHorizontalDeltaFromRotation(float eulerAngle)
    {
        if (eulerAngle < 180)
        {
            float angle = 0 - eulerAngle;
            float angleSeverity = Mathf.Pow(angle, 2);
            return -angleSeverity * angledMovementPower;
        }
        else
        {
            float angle = 360 - eulerAngle;
            float angleSeverity = Mathf.Pow(angle, 2);
            return angleSeverity * angledMovementPower;
        }
    }
}