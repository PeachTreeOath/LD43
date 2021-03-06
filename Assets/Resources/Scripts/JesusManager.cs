﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JesusManager : Singleton<JesusManager>
{

    public HandOfGod leftHand;
    public HandOfGod rightHand;

    public JesusArmBezier[] armBeziers;

    private VehicleController controllingVehicle;
    private HandOfGod guidingHand;

    public void Start()
    {
        leftHand.Hover();
        rightHand.FollowMouse();

        armBeziers = GetComponentsInChildren<JesusArmBezier>();
    }

    public void VehicleCrashedLetGo(VehicleController vehicleController)
    {
        Debug.Log("Vehicle crashed! let go!");
        guidingHand.Hover();
        guidingHand = null;
    }

    public void SelectAVehicle(VehicleController vehicleController)
    {
        if (vehicleController != null)
        {
            controllingVehicle = vehicleController;
            if (rightHand.IsFollowingMouse)
            {
                rightHand.GuideVehicle(vehicleController);
                guidingHand = rightHand;
                leftHand.FollowMouse();
            }
            else if (leftHand.IsFollowingMouse)
            {
                leftHand.GuideVehicle(vehicleController);
                guidingHand = leftHand;
                rightHand.FollowMouse();
            }
            else
            {
                rightHand.FollowMouse();
                guidingHand = null;
                SelectAVehicle(vehicleController);
            }
        }
        else
        {
            guidingHand = null;
            if (rightHand.IsGuidingCar) rightHand.Hover();
            if (leftHand.IsGuidingCar) leftHand.Hover();
        }

    }

    public void SetBodyTransparency(bool isTransparent)
    {
        foreach (JesusArmBezier arm in armBeziers)
        {
            arm.SetArmTransparency(isTransparent);
        }
        leftHand.SetHandTransparency(isTransparent);
        rightHand.SetHandTransparency(isTransparent);
    }
}
