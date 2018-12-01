using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JesusManager : Singleton<JesusManager> {

    public HandOfGod leftHand;
    public HandOfGod rightHand;

    public void Start() {
        leftHand.Hover();
        rightHand.FollowMouse(); 
    }

    public void SelectAVehicle(VehicleController vehicleController) {
        if(rightHand.IsFollowingMouse) {
            rightHand.GuideVehicle(vehicleController);
            leftHand.FollowMouse();
        } else if(leftHand.IsFollowingMouse) {
            leftHand.GuideVehicle(vehicleController);
            rightHand.FollowMouse();
        } else {
            rightHand.FollowMouse();
            SelectAVehicle(vehicleController);
        }
    }
}
