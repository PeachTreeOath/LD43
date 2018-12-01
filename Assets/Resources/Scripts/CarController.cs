using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{

    public float moveSpeed;
    public float rotateSpeed;
    public float angledMovementPower;
    public GameObject carSprite; // Contains both sprite and collider and are separate from the actual GameObject

    private Rigidbody2D rbody;

    // Use this for initialization
    void Start()
    {
        rbody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        // Movement from input
        float rotateDelta = rotateSpeed * Input.GetAxisRaw("Horizontal");
        carSprite.transform.Rotate(Vector3.back, rotateDelta);

        // Drift car left/right based on how much rotation applied
        float hDelta = GetHorizontalDeltaFromRotation(carSprite.transform.eulerAngles.z);

        float vDelta = moveSpeed * Input.GetAxisRaw("Vertical");
        Vector2 newPosition = (Vector2)transform.position + new Vector2(hDelta, vDelta);
        rbody.MovePosition(newPosition);

        // Apply random drift
        // TODO
    }

    private float GetHorizontalDeltaFromRotation(float eulerAngle)
    {
        if (eulerAngle < 180)
        {
            float angle = 0 - eulerAngle;
			Debug.Log("angle " + angle + " euler " + eulerAngle);
            float angleSeverity = Mathf.Pow(angle, 2);
			Debug.Log("angle sev " + angleSeverity);
            return -angleSeverity * angledMovementPower;
        }
        else
        {
            float angle = 360-eulerAngle;
			Debug.Log("dangle " + angle + " euler " + eulerAngle);
            float angleSeverity = Mathf.Pow(angle, 2);
			Debug.Log("dangle sev " + angleSeverity);
            return angleSeverity * angledMovementPower;
        }
    }
}
