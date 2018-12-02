using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiracleAnimator : MonoBehaviour {


    private Vector2 startPoint;
    private Vector2 endPoint;
    private float duration;

    private float startTime;
    private float endTime;

    private bool inProgress = false;

    private VehicleController vc;
    private Rigidbody2D rb;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (inProgress) {
            doLerp();
        }
		
	}

    public void beginMiracles(Vector2 start, Vector2 end, float duration) {
        Debug.Log("Beginning a miraculous entry for " + gameObject.name + " starting:" + start
            + ", ending:" + end + ", duration:" + duration);
        startTime = Time.time;
        endTime = startTime + duration;
        this.duration = duration;
        this.startPoint = start;
        this.endPoint = end;
        inProgress = true;
        rb = GetComponent<Rigidbody2D>();
        if (rb == null) {
            Debug.LogError("Can't perform a miracle without a rigidbody");
        }
        rb.Sleep();
        transform.position = start;
        vc = GetComponent<VehicleController>();
        if (vc != null) {
            vc.setEnteringStage(true);
        }
    }

    private void doLerp() {
        float t = (Time.time - startTime) / duration;
        Debug.Log("t=" + t);
        if (t >= 1) {
            t = 1.0f;
            inProgress = false;
            vc.setEnteringStage(false);
            rb.WakeUp();
            Destroy(this, 0.25f); //remove this script, it is done
        }
        Vector2 newPos = Vector2.Lerp(startPoint, endPoint, t);
        //rb.MovePosition(newPos);
        transform.position = newPos;
    }


}
