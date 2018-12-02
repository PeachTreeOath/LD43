using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleWobble : MonoBehaviour {

    //Starting Position
    private GameObject vehicleSpriteObject;
    private Transform vehicleSpriteTransform;
    private Vector2 startingPos;

	// Use this for initialization
	void Start ()
	{
	    vehicleSpriteObject = gameObject.transform.parent.gameObject;
	    vehicleSpriteTransform = vehicleSpriteObject.GetComponent<Transform>();
	    startingPos.x = vehicleSpriteTransform.transform.position.x;
	    startingPos.y = vehicleSpriteTransform.transform.position.y;
    }
	
	// Update is called once per frame
	void Update ()
	{
	    var random = Random.Range(1, 1.65f);
	    vehicleSpriteObject.transform.position = new Vector2(startingPos.x + Mathf.Sin(Time.deltaTime) * random,
	        startingPos.y + Mathf.Sin(Time.deltaTime) * random);
	}
}
