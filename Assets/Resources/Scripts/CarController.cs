using System.Collections;
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

    public float angledMovementPower;

    public float driftPower;

    #region Vehicle Stats

	[Tooltip("How much it knocks other cars around & how much it gets knocked around.")]
    public float weight;

	[Tooltip("How quickly and easily it handles - i.e. the rotate speed.")]
    public float control;

	[Tooltip("How quicly it moves forward.")]
    public float speed;

	[Tooltip("How much time given at a check point.")]
    public float prayerValue;

	[Tooltip("How likely they are to fall asleep - 0 (low) to 3 (high).")]
    public float sleepChance;

	[Tooltip("How violently the car will swerve.")]
    public float sleepSeverity;

    public float awakeGracePeriod;

    #endregion

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
        Vector2 driftDelta = Random.insideUnitCircle * driftPower * Time.deltaTime;

        if (!isSelected)
            return;

        // Movement from input
        float rotateDelta = control * Input.GetAxisRaw("Horizontal");
        carSprite.transform.Rotate(Vector3.back, rotateDelta + driftDelta.x);

        // Drift car left/right based on how much rotation applied
        float hDelta = GetHorizontalDeltaFromRotation(carSprite.transform.eulerAngles.z);

        float vDelta = speed * Input.GetAxisRaw("Vertical");
        Vector2 newPosition = (Vector2)transform.position + new Vector2(hDelta, vDelta + driftDelta.y);
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