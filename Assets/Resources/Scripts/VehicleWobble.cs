using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class VehicleWobble : MonoBehaviour {

    //SAdjust wobble through Inspector Prefabs Vehicle Wobble
    [SerializeField] bool enableWobble = true;
    [SerializeField] float wobbleMin;
    [SerializeField] float wobbleMax;
    [SerializeField] float wobbleFrequency;

    //Starting Position
    private GameObject vehicleSpriteObject;
    private Transform vehicleSpriteTransform;
    private Vector2 startingPos;
    private Stopwatch wobbleTimer;


	// Use this for initialization
	void Start ()
	{
	    vehicleSpriteObject = gameObject.transform.parent.gameObject;
	    vehicleSpriteTransform = vehicleSpriteObject.GetComponent<Transform>();
	    startingPos.x = vehicleSpriteTransform.transform.position.x;
	    startingPos.y = vehicleSpriteTransform.transform.position.y;

        wobbleTimer = new Stopwatch();
        wobbleTimer.Start();
    }
	
	// Update is called once per frame
	void Update ()
	{
	    if (!enableWobble) return;
	    var random = Random.Range(wobbleMin, wobbleMax);
        vehicleSpriteObject.transform.position = new Vector2(startingPos.x + Mathf.Sin(Time.deltaTime) * random,
	        startingPos.y + Mathf.Sin(Time.deltaTime) * random);
	}
}
