using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleBody : MonoBehaviour {

    private int TERRAIN_LAYER;

    private VehicleController controller;
    private Rigidbody2D rbody;

	void Awake() {
        TERRAIN_LAYER = LayerMask.NameToLayer("Terrain");

        controller = GetComponentInParent<VehicleController>();
        rbody = GetComponent<Rigidbody2D>();
	}

    void OnCollisionEnter2D(Collision2D collision) {
        if(collision.gameObject.layer == TERRAIN_LAYER) {
            var collisionInfo = CollisionInfo.FromCollision(collision);
            controller.OnCollideWithWalls(collisionInfo); 
        } //else if()
    }

    // Update is called once per frame
    void Update () {
		
	}
}
