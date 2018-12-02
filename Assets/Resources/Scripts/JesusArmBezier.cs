using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class JesusArmBezier : MonoBehaviour {

    public bool isLeftArm;
    public Transform armEndPosition;
    public float currentArmEndTangent;

    private static int NUM_ARM_POINTS = 30;
    // private static Vector3 RIGHT_ARM_START_POSITION = new Vector3(0.5f, -4.5f, 0);
    private static float NEUTRAL_ARM_SLOPE = 1;
    private static float LOW_ARM_SLOPE = 0.5f;
    private static float HIGH_ARM_SLOPE = 2;
    private static float SLOPE_ITERATION_STEP = 0.1f;

    private Vector3[] mArmPositions = new Vector3[NUM_ARM_POINTS];

    public LineRenderer lineRenderer;

	// Use this for initialization
	void Start () {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = NUM_ARM_POINTS;
    }
	
	// Update is called once per frame
	void Update () {
        UpdateEndTangent();
        DrawCurve(transform.position, armEndPosition.position, currentArmEndTangent);
	}

    // End tangent is different based on the driving direction
    private void UpdateEndTangent() {
        float endTangent;
        if (Input.GetKey("d")) { // turning right
            if (isLeftArm) { // high negative
                endTangent = -1f * HIGH_ARM_SLOPE;
            } else { // low positive
                endTangent = LOW_ARM_SLOPE;
            }
        } else if (Input.GetKey("a")) { // turning left
            if (isLeftArm) { // low negative
                endTangent = -1f * LOW_ARM_SLOPE;
            } else {
                endTangent = HIGH_ARM_SLOPE;
            }
        } else {
            endTangent = isLeftArm ? NEUTRAL_ARM_SLOPE * -1.0f : NEUTRAL_ARM_SLOPE;
        }

        if (endTangent > currentArmEndTangent && endTangent - currentArmEndTangent > SLOPE_ITERATION_STEP) {
            currentArmEndTangent = currentArmEndTangent + SLOPE_ITERATION_STEP;
        } else if (endTangent < currentArmEndTangent && currentArmEndTangent - endTangent > SLOPE_ITERATION_STEP) {
            currentArmEndTangent = currentArmEndTangent - SLOPE_ITERATION_STEP;
        }
    }

    private void DrawCurve(Vector3 startPoint, Vector3 endPoint, float endTangent) {
        // Calculate slope of forearm
        float startTangent = 0f;

        // Calculate intersect
        float cEndPoint = endPoint.y - endTangent * endPoint.x;
        float intersectX = (startPoint.y - cEndPoint) / endTangent;
        float intersectY = startPoint.y;
        Vector3 intersectPoint = new Vector3(intersectX, intersectY, 0);

        // Plot Bezier Curve
        for (int i = 0; i < NUM_ARM_POINTS; i++) {
            float t = (float)i / (float)NUM_ARM_POINTS;
            mArmPositions[i] = CalculateArmPoint(t, startPoint, intersectPoint, endPoint);
        }
        lineRenderer.SetPositions(mArmPositions);

        // Update texture based off of length of arm.
        float armLength = 0;
        for (int i = 1; i < NUM_ARM_POINTS; i++)
        {
            armLength += Vector3.Distance(mArmPositions[i], mArmPositions[i - 1]);
        }
        lineRenderer.material.mainTextureScale = new Vector2(armLength, 1);
    }

    // Calculate arm point using Quadratic Bezier Function
    // https://en.wikipedia.org/wiki/B%C3%A9zier_curve
    private Vector3 CalculateArmPoint(float t, Vector3 startPoint, Vector3 midPoint, Vector3 endPoint) {
        return (1-t)*((1-t)*startPoint + t*midPoint) + t*((1-t)*midPoint + t*endPoint);
    }
}
