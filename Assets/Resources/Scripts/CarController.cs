using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    public int id;
    public bool isSelected; // This is how a user selects which car to move
    public GameObject carSprite; // Contains both sprite and collider and are separate from the actual GameObject

    public float moveSpeed;
    public float rotateSpeed;
    public float angledMovementPower;
    public float driftPower;
    public float weight;
    public float prayerValue;
    public float sleepChance;
    public float sleepSeverity;
    public float awakeGracePeriod;

    private Rigidbody2D rbody;
    private bool isSleeping;
    private float nextSleepTime;
    private float sleepTimeElapsed;
    private Vector2 sleepVector;

    // Use this for initialization
    void Start()
    {
        rbody = GetComponent<Rigidbody2D>();
        nextSleepTime = 8 - sleepChance + UnityEngine.Random.Range(-1, 2); // Choose to sleep randomly from 1-8 seconds (sleepSeverity currently can be between 1 and 6)
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
        if (isSelected)
        {
            hInput = rotateSpeed * Input.GetAxisRaw("Horizontal");
            vInput = moveSpeed * Input.GetAxisRaw("Vertical");
        }

        // Movement from input
        float rotateDelta = hInput + (sleepVector.x * sleepSeverity);
        carSprite.transform.Rotate(Vector3.back, rotateDelta);

        // Drift car left/right based on how much rotation applied
        float hDelta = GetHorizontalDeltaFromRotation(carSprite.transform.eulerAngles.z);

        Vector2 newPosition = (Vector2)transform.position + new Vector2(hDelta, vInput + (sleepVector.y * sleepSeverity * .01f));
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

    public void Select()
    {
        isSelected = true;
    }

    public void Deselect()
    {
        isSelected = false;
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
