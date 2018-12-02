using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Controls obstacles like cones on the road.
public class HighlightableVehicle : MonoBehaviour
{
    public Renderer rend;
    public VehicleController vc;
    bool wasSelected;

    void Update()
    {
        if(RayCaster.hitInfo.collider != null)
        {
            if(RayCaster.hitInfo.collider.gameObject.GetInstanceID() == gameObject.GetInstanceID())
            {
                wasSelected = true;
                rend.enabled = true;
            }else if(wasSelected)
            {
                rend.enabled = false;
            }
        }else
        {
            rend.enabled = false;
        }

        if(vc.isSelected && 
            rend.enabled)
        {
            rend.enabled = false;
        }
    }
}