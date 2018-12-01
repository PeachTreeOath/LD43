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
            var dir = Vector2.zero;

            //for now let's just detect left and right collisions
            var collisionLocation = GetContactCentroid(collision);
            if(collisionLocation.x < transform.position.x) {
                dir = new Vector2(1, 0);
            } else {
                dir = new Vector2(-1, 0); 
            }

            controller.OnCollideWithWalls(dir); 
        }
    }

    Vector2 GetContactCentroid(Collision2D collision) {
        Vector2 center = Vector2.zero;
        for(var i = 0; i < collision.contactCount; i++) {
            var contact = collision.GetContact(i);
            
            center += contact.point;
        }

        center.Scale(new Vector2(collision.contactCount, collision.contactCount));

        return center;
    }


    // Update is called once per frame
    void Update () {
		
	}
}
