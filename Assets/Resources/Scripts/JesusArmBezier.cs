using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class JesusArmBezier : MonoBehaviour
{
    public GameObject handOfGod;

    public Material defaultMatieral;

    public Material transparentMaterial;

    public float width;

    [Tooltip("The amount of distance from Jesus center arm should be.")]
    public float distanceFromCenter;

    private static int NUM_ARM_POINTS = 30;
    private static int WRIST_DISTANCE = 2;
    private static int PLANE_OF_GOD = 0; // z plane

    private Vector3[] mArmPositions = new Vector3[NUM_ARM_POINTS];

    public LineRenderer lineRenderer;

    // Use this for initialization
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = NUM_ARM_POINTS;
        lineRenderer.material = defaultMatieral;

        transform.position = new Vector3(transform.position.x + distanceFromCenter, transform.position.y, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (lineRenderer.startWidth != width)
        {
            lineRenderer.startWidth = width;
            lineRenderer.endWidth = width;
        }
        DrawCurve(
            transform.position,
            handOfGod.transform.position + Vector3.back,
            handOfGod.transform.rotation.eulerAngles);
    }

    private float getTangentFromAngle(Vector3 eulerAngles)
    {
        float angle = eulerAngles.z; // 0 is north, increasing counter clockwise
        angle = angle + 90f; // adding 90 so that 0 is on positive x axis
        float modifier = 1.06f;
        if (handOfGod.GetComponent<HandOfGod>().isRightHand)
        {
            modifier = .97f;
        }
        return Mathf.Tan(Mathf.Deg2Rad * angle * modifier);
    }

    //     /   __   \
    //    |   (  )   | 
    //     \ |    | /
    //      \|    |/
    //       |    |
    private void DrawCurve(Vector3 pShoulder, Vector3 pHand, Vector3 endAngleVector)
    {
        // Find wrist point
        float endTangent = getTangentFromAngle(endAngleVector);
        float wristX = endTangent < 0 ? pHand.x + WRIST_DISTANCE : pHand.x - WRIST_DISTANCE;
        float wristC = pHand.y - endTangent * pHand.x;
        float wristY = endTangent * wristX + wristC;
        Vector3 pWrist = new Vector3(wristX, wristY, PLANE_OF_GOD);

        // Calculate intersect
        float cEndPoint = pHand.y - endTangent * pHand.x;
        float intersectX = (pShoulder.y - cEndPoint) / endTangent;
        float intersectY = pShoulder.y;
        Vector3 pElbow = new Vector3(pWrist.x, intersectY, PLANE_OF_GOD);

        // Plot Bezier Curve
        for (int i = 0; i < NUM_ARM_POINTS; i++)
        {
            float t = (float)i / (float)NUM_ARM_POINTS;
            mArmPositions[i] = CalculateArmPoint(t, pShoulder, pElbow, pWrist, pHand);
        }
        // extrapolate the last point to not show gaps
        Vector3 secondToEndPoint = mArmPositions[NUM_ARM_POINTS - 2];
        Vector3 finalVector = pHand - secondToEndPoint;
        mArmPositions[NUM_ARM_POINTS - 1] = pHand - 0.5f * Vector3.Normalize(finalVector);
        lineRenderer.SetPositions(mArmPositions);

        // Update texture based off of length of arm.
        float armLength = 0;
        for (int i = 1; i < NUM_ARM_POINTS; i++)
        {
            armLength += Vector3.Distance(mArmPositions[i], mArmPositions[i - 1]);
        }
        lineRenderer.material.mainTextureScale = new Vector2(armLength, 1);
    }

    // Calculate arm point using Cubic Bezier Function
    // https://en.wikipedia.org/wiki/B%C3%A9zier_curve
    private Vector3 CalculateArmPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        float u = (1f - t);
        float u2 = Mathf.Pow(u, 2);
        float u3 = Mathf.Pow(u, 3);
        float t2 = Mathf.Pow(t, 2);
        float t3 = Mathf.Pow(t, 3);
        return u3 * p0 + 3f * u2 * t * p1 + 3f * u * t2 * p2 + t3 * p3;
        // return (1 - t) * ((1 - t) * p0 + t * p1) + t * ((1 - t) * p1 + t * p3);
    }

    public void SetArmTransparency(bool isTransparent)
    {
        lineRenderer.material = isTransparent ? transparentMaterial : defaultMatieral;
    }
}
