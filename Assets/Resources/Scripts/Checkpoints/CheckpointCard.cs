﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointCard : MonoBehaviour {

    [SerializeField]
    private string itemText;

    [SerializeField]
    private Sprite itemSprite;

    [SerializeField]
    private Sprite cardBackground;

    [SerializeField]
    private VehicleTypeEnum vehicleType;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public string getText() {
        return itemText;
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
