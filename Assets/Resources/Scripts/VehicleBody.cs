using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleBody : MonoBehaviour {

    private int TERRAIN_LAYER;
    private int CAR_LAYER;

    [SerializeField]
    private Sprite crashedSprite;

    private VehicleController controller;
    private Rigidbody2D rbody;

	void Awake() {
        TERRAIN_LAYER = LayerMask.NameToLayer("Terrain");
        CAR_LAYER = LayerMask.NameToLayer("Cars");

        controller = GetComponentInParent<VehicleController>();
        rbody = GetComponent<Rigidbody2D>();
	}

    void OnCollisionEnter2D(Collision2D collision) {
        if(collision.gameObject.layer == TERRAIN_LAYER) {
            var collisionInfo = CollisionInfo.FromCollision(collision);
            controller.OnCollideWithWalls(collisionInfo); 
        } else if(collision.gameObject.layer == CAR_LAYER) {
            var collisionInfo = CollisionInfo.FromCollision(collision); //TODO pass in the weight from the other car!
            var vehicle = collision.gameObject.GetComponent<VehicleController>();
            controller.OnCollideWithVehicle(collisionInfo, vehicle);
        }
    }

    public void showCrashedSprite() {
        Debug.Log("Show crashed sprite " + gameObject.name);
        if (crashedSprite != null) {
            SpriteRenderer sr = GetComponentInChildren<SpriteRenderer>();
            if (sr != null) {
                sr.sprite = crashedSprite;
            }
        } else {
            Debug.LogWarning("No crashed sprite for " + gameObject.name);
        }
    }

    // Update is called once per frame
    void Update () {
		
	}
}
