using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour {

    [SerializeField]
    private List<VehicleTypeEnum> availVehicles;

    [SerializeField]
    private List<CheckpointCard> cardsForThisCheckpoint; //max of 4 by default

    //TODO other things might be available here,
    //stats, prayers, etc

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public List<VehicleTypeEnum> getAvailableVehicles() {
        return availVehicles;
    }

    public List<CheckpointCard> getCards() {
        return cardsForThisCheckpoint;
    }
}
