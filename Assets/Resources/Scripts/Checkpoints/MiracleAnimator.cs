using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiracleAnimator : MonoBehaviour {


    private Vector2 startPoint;
    private Vector2 endPoint;
    private float duration;

    private Color startColor;

    private float startTime;
    private float endTime;

    private bool inProgress = false;

    private int startingLayer;

    private VehicleController vc;
    private SpriteRenderer sr;
    private Collider2D col;

    void Awake() {

    }

	// Use this for initialization
	void Start () {

        col = gameObject.GetComponentInChildren<Collider2D>();
        if (col != null) {
            startingLayer = col.gameObject.layer;
            //This layer allows click detection without colliding with anything in the game
            col.gameObject.layer = LayerMask.NameToLayer("EnteringCars");
        }
	}
	
	// Update is called once per frame
	void Update () {
        if (inProgress) {
            doLerp();
        }
		
	}

    public void beginMiracles(Vector2 start, Vector2 end, float duration) {
        startTime = Time.time;
        endTime = startTime + duration;
        this.duration = duration;
        this.startPoint = start;
        this.endPoint = end;
        inProgress = true;
        transform.position = start;
        //essentially we assign this script to the object and then hijack it
        //to modify its state for a bit.
        //Once our timer is expired we reset the state and clean ourselves up
        vc = GetComponent<VehicleController>();
        if (vc != null) {
            vc.setEnteringStage(true);
        }
        sr = GetComponentInChildren<SpriteRenderer>();
        if (sr != null) {
            startColor = sr.color;
        }
    }

    /// <summary>
    /// Call this externally to  terminate this miracle before it would normally complete
    /// </summary>
    public void endMiracle() {
        finish();
        Destroy(this, 0.25f); //remove this script, it is done
        //we aren't setting t=1 cuz we want to keep the current position of whatever is incoming
    }

    private void doLerp() {
        float t = (Time.time - startTime) / duration;
        //Debug.Log("t=" + t);
        if (t >= 1) {
            t = 1.0f;
            finish();
            Destroy(this, 0.25f); //remove this script, it is done
        }
        Vector2 newPos = Vector2.Lerp(startPoint, endPoint, t);
        transform.position = newPos;
        if (sr != null) {
            sr.color = getCurrentColor();
        }
    }

    /// <summary>
    /// End the animation sequence and reset the state of vehicle to 
    /// what it was before we f'ed with it
    /// </summary>
    private void finish() {
        inProgress = false;
        vc.setEnteringStage(false);
        //rb.WakeUp();
        if (sr != null) {
            sr.color = startColor;
        }
        if (col != null) {
            col.gameObject.layer = startingLayer;
        }
    }


    private Color getCurrentColor() {
        float t = (Time.time - startTime) / duration;
        float a = Mathf.Cos(Mathf.Deg2Rad * t * 360 * 11);
        a = Mathf.Max(a, 0.25f); //don't fade out completely
        a = Mathf.Min(a, 0.90f); //don't get too bright
        Color newCol = new Color(startColor.r, startColor.g, startColor.b, a);
        return newCol;
    }

}
