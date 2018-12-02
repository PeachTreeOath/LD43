using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Random = UnityEngine.Random;

public class ScreenShake : MonoBehaviour {

    [SerializeField] Transform cameraTransform;
    [SerializeField] private float shakeDuration;
    [SerializeField] private float shakeMagnitude;
    private Vector3 initialPosition;
    private Stopwatch shakeTimer;

    // Use this for initialization
    void Start ()
    {
        cameraTransform = gameObject.transform;
        initialPosition = cameraTransform.localPosition;
        shakeTimer = new Stopwatch();
    }
	
	// Update is called once per frame
	void Update () {
	    if (shakeTimer.IsRunning && shakeTimer.ElapsedMilliseconds < shakeDuration)
	    {
	        transform.localPosition = initialPosition + Random.insideUnitSphere * shakeMagnitude;
	    }
	    else
	    {
	        shakeTimer.Stop();
	        transform.localPosition = initialPosition;
	    }
    }

    public void TriggerShake(float duration) 
    {
        shakeDuration = duration;
        shakeTimer.Reset();
        shakeTimer.Start();
    }
}
