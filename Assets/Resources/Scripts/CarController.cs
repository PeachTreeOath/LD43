﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    public int id;

	[Tooltip("The pool the car belongs to.")]
    public CarPool carPool;

    public bool isSelected; // This is how a user selects which car to move

    public GameObject carSprite; // Contains both sprite and collider and are separate from the actual GameObject
    
    private Rigidbody2D rbody;

    public float moveSpeed;

    public float rotateSpeed;

    public float angledMovementPower;

    public float weight;

    public float prayerValue;

    public float sleepChance;

    public float sleepSeverity;

    public float awakeGracePeriod;

    // Use this for initialization
    void Start()
    {
        rbody = GetComponent<Rigidbody2D>();
        carPool = GetComponentInParent<CarPool>();
    }

    // Update is called once per frame
    void Update()
    {
        // Apply random drift
        // TODO

        if (!isSelected)
            return;

        // Movement from input
        float rotateDelta = rotateSpeed * Input.GetAxisRaw("Horizontal");
        carSprite.transform.Rotate(Vector3.back, rotateDelta);

        // Drift car left/right based on how much rotation applied
        float hDelta = GetHorizontalDeltaFromRotation(carSprite.transform.eulerAngles.z);

        float vDelta = moveSpeed * Input.GetAxisRaw("Vertical");
        Vector2 newPosition = (Vector2)transform.position + new Vector2(hDelta, vDelta);
        rbody.MovePosition(newPosition);
    }

    public void OnMouseDown()
    {
        carPool.SelectCar(id);
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
