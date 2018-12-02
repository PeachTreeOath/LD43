using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Controls obstacles like cones on the road.
public class RayCaster : MonoBehaviour
{
    public static RaycastHit2D hitInfo;

    void FixedUpdate()
    {
        hitInfo = Physics2D.GetRayIntersection(Camera.main.ScreenPointToRay(Input.mousePosition));
    }
}