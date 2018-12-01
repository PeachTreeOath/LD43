using System;
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

    #region Vehicle Stats

    [Tooltip("How much it knocks other cars around & how much it gets knocked around.")]
    public float weight;

    [Tooltip("How quickly and easily it handles - i.e. the rotate speed.")]
    public float control;

	[Tooltip("How quickly it moves forward.")]
    public float speed;

    [Tooltip("How much time given at a check point.")]
    public float prayerValue;

	[Tooltip("How likely they are to fall asleep - 1 (low) to 3 (high).")]
    public float sleepChance;

    [Tooltip("How violently the car will swerve.")]
    public float sleepSeverity;

    public float awakeGracePeriod;

    #endregion

    private bool isSleeping;
    private float nextSleepTime;
    private float sleepTimeElapsed;
    private Vector2 sleepVector;

    // Use this for initialization
    void Start()
    {
        rbody = GetComponent<Rigidbody2D>();
        nextSleepTime = 8 - sleepChance * 2 + UnityEngine.Random.Range(-1, 2); // Choose to sleep randomly from 1-7 seconds
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
        if (isSelected)
        {
            hInput = control * 0.0428f * Input.GetAxisRaw("Horizontal");
            vInput = speed * 0.0029f * Input.GetAxisRaw("Vertical");
        }

        // Movement from input
        float rotateDelta = hInput + (sleepVector.x * sleepSeverity);
        vehicleSprite.transform.Rotate(Vector3.back, rotateDelta);

        // Drift car left/right based on how much rotation applied
        float hDelta = GetHorizontalDeltaFromRotation(vehicleSprite.transform.eulerAngles.z);

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