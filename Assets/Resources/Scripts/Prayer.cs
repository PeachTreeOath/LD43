using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Prayer : MonoBehaviour {

    //Serialized Prayer Variables
    [SerializeField] private float SpeedModifer = 1f;
    [SerializeField] private float AngularModifer = .2f;

    //Private Prayer Variables
    private Vector3 _vehicleLocation = Vector3.zero;
    private Vector3 _prayerMeterLocation;
    private Rigidbody2D _prayerRigidBody;

    public void Initialize(Vector3 vehicleLocation)
    {
        //Set the prayer's destination.
        _prayerMeterLocation = GameObject.Find("PrayerMeterProgress").transform.position;
    }

	// Use this for initialization
	void Start () {

        //Initialize the rigid body.
        _prayerRigidBody = gameObject.transform.Find("PrayerSprite").GetComponent<Rigidbody2D>();

	    //Animate toward the progress bar.
	    _prayerRigidBody.velocity = (_prayerMeterLocation * Time.deltaTime) * SpeedModifer;
    }
	
	// Update is called once per frame
    void Update()
    {
        //Animate toward the progress bar.
        _prayerRigidBody.velocity = new Vector2(_prayerRigidBody.velocity.x - AngularModifer, _prayerRigidBody.velocity.y);
    }
}
