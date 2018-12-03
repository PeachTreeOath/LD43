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

    private static int NUM_ARM_POINTS = 50;
    private static int WRIST_DISTANCE = 2;
    private static float HALF_HAND_LENGTH = 0.5f;

    private Vector3[] mArmPositions = new Vector3[NUM_ARM_POINTS];

    public LineRenderer lineRenderer;

    // Use this for initialization
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = NUM_ARM_POINTS;
        lineRenderer.material = defaultMatieral;

        transform.position = new Vector3(transform.position.x + distanceFromCenter, transform.position.y, transform.position.z);
    }

    // Update is called once per frame
    void Update()
    {
        // Stop arm movement when you run out of prayers
        if(GameManager.instance.isPrayerDeath())
            return;

        if (lineRenderer.startWidth != width)
        {
            lineRenderer.startWidth = width;
            lineRenderer.endWidth = width;
        }
        Vector3 adjustedAngles = AdjustHandAngle(handOfGod.transform.rotation.eulerAngles);
        DrawCurve(
            transform.position,
            TranslatePoint(handOfGod.transform.position, adjustedAngles, -1f * HALF_HAND_LENGTH),
            adjustedAngles);
    }

    private float GetTangentFromAngle(Vector3 eulerAngles)
    {
        float angle = eulerAngles.z; // 0 is north, increasing counter clockwise
        angle = angle + 90f; // adding 90 so that 0 is on positive x axis
        return Mathf.Tan(Mathf.Deg2Rad * angle);
    }

    private Vector3 AdjustHandAngle(Vector3 eulerAngles)
    {
        float modifier = 1.06f;
        if (handOfGod.GetComponent<HandOfGod>().isRightHand)
        {
            modifier = .97f;
        }
        return new Vector3(eulerAngles.x, eulerAngles.y, modifier * eulerAngles.z);
    }

    private Vector3 TranslatePoint(Vector3 originPoint, Vector3 eulerAngles, float distance)
    {
        float angleRadians = eulerAngles.z * Mathf.Deg2Rad; // 0 is north, counterclockwise
        float dx = -1f * distance * Mathf.Sin(angleRadians);
        float dy = distance * Mathf.Cos(angleRadians);
        return new Vector3(originPoint.x + dx, originPoint.y + dy, originPoint.z);
    }

    //     /   __   \
    //    |   (  )   | 
    //     \ |    | /
    //      \|    |/
    //       |    |
    private void DrawCurve(Vector3 pShoulder, Vector3 pHand, Vector3 endAngleVector)
    {
        // Find wrist point
        Vector3 pWrist = TranslatePoint(pHand, endAngleVector, -1f * WRIST_DISTANCE);

        // Calculate intersect
        float endTangent = GetTangentFromAngle(endAngleVector);
        float cEndPoint = pHand.y - endTangent * pHand.x;
        float intersectX = (pShoulder.y - cEndPoint) / endTangent;
        float intersectY = pShoulder.y;
        Vector3 pElbow = new Vector3(pWrist.x, intersectY, pWrist.z);

        // Plot Bezier Curve
        for (int i = 0; i < NUM_ARM_POINTS; i++)
        {
            float t = (float)i / (float)NUM_ARM_POINTS;
            mArmPositions[i] = CalculateArmPoint(t, pShoulder, pElbow, pWrist, pHand);
        }
        // extrapolate the last point to not show gaps
        // Vector3 secondToEndPoint = mArmPositions[NUM_ARM_POINTS - 2];
        // Vector3 finalVector = pHand - secondToEndPoint;
        mArmPositions[NUM_ARM_POINTS - 1] = pHand;
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
    }

    public void SetArmTransparency(bool isTransparent)
    {
        lineRenderer.material = isTransparent ? transparentMaterial : defaultMatieral;
    }
}
