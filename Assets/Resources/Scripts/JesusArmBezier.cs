using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class JesusArmBezier : MonoBehaviour
{
    public GameObject handOfGod;
    
    public float width;

	[Tooltip("The amount of distance from Jesus center arm should be.")]
    public float distanceFromCenter;

    private static int NUM_ARM_POINTS = 30;

    private Vector3[] mArmPositions = new Vector3[NUM_ARM_POINTS];

    public LineRenderer lineRenderer;

    // Use this for initialization
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = NUM_ARM_POINTS;

        transform.position = new Vector3(transform.position.x + distanceFromCenter, transform.position.y, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if(lineRenderer.startWidth != width)
        {
            lineRenderer.startWidth = width;
            lineRenderer.endWidth = width;
        }
        float currentArmEndTangent = getTangentFromAngle(handOfGod.transform.rotation.eulerAngles);
        DrawCurve(transform.position, handOfGod.transform.position + Vector3.back, currentArmEndTangent);
    }

    private float getTangentFromAngle(Vector3 eulerAngles)
    {
        float angle = eulerAngles.z; // 0 is north, increasing counter clockwise
        angle = angle + 90f; // adding 90 so that 0 is on positive x axis
        float modifier = 1.06f;
        if(handOfGod.GetComponent<HandOfGod>().isRightHand)
        {
            modifier = .97f;
        }
        return Mathf.Tan(Mathf.Deg2Rad*angle* modifier);
    }

    private void DrawCurve(Vector3 startPoint, Vector3 endPoint, float endTangent)
    {
        // Calculate intersect
        float cEndPoint = endPoint.y - endTangent * endPoint.x;
        float intersectX = (startPoint.y - cEndPoint) / endTangent;
        float intersectY = startPoint.y;
        Vector3 intersectPoint = new Vector3(intersectX, intersectY, 1);

        // Plot Bezier Curve
        for (int i = 0; i < NUM_ARM_POINTS; i++)
        {
            float t = (float)i / (float)NUM_ARM_POINTS;
            mArmPositions[i] = CalculateArmPoint(t, startPoint, intersectPoint, endPoint);
        }
        // extrapolate the last point to not show gaps
        Vector3 secondToEndPoint = mArmPositions[NUM_ARM_POINTS - 2];
        Vector3 finalVector = endPoint - secondToEndPoint;
        mArmPositions[NUM_ARM_POINTS-1] = endPoint - 0.5f * Vector3.Normalize(finalVector);
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
    private Vector3 CalculateArmPoint(float t, Vector3 startPoint, Vector3 midPoint, Vector3 endPoint)
    {
        return (1 - t) * ((1 - t) * startPoint + t * midPoint) + t * ((1 - t) * midPoint + t * endPoint);
    }
}
