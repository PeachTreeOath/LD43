using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{

    public float moveSpeed;

    private Rigidbody2D rbody;

    // Use this for initialization
    void Start()
    {
        rbody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        float hDelta = moveSpeed * Input.GetAxisRaw("Horizontal");
        float vDelta = moveSpeed * Input.GetAxisRaw("Vertical");
        Vector2 newPosition = (Vector2)rbody.transform.position + new Vector2(hDelta, vDelta);
        rbody.MovePosition(newPosition);
    }
}
