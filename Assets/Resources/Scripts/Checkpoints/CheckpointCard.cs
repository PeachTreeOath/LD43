﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointCard : MonoBehaviour {

    [SerializeField]
    private string itemText;

    [SerializeField]
    private string controlText;

    [SerializeField]
    private string prayerText;

    [SerializeField]
    private string sleepText;

    [SerializeField]
    private Sprite itemSprite;

    [SerializeField]
    private Sprite cardBackground;

    [SerializeField]
    private VehicleTypeEnum vehicleType;

    public string getText() {
        return itemText;
    }

    public string getControlText() {
        return controlText;
    }

    public string getPrayerText() {
        return prayerText;
    }

    public string getSleepText() {
        return sleepText;
    }

    public Sprite getItemSprite() {
        return itemSprite;
    }

    public Sprite getBackgroundSprite() {
        return cardBackground;
    }

    public VehicleTypeEnum getVehicleType() {
        return vehicleType;
    }
}
