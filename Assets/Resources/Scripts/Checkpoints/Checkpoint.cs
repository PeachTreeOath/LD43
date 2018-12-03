using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour {
    
    private List<VehicleTypeEnum> availVehicles;
    
    private List<CheckpointCard> cardsForThisCheckpoint; //max of 4 by default

    
    //TODO other things might be available here,
    //stats, prayers, etc

	// Use this for initialization
	void Awake () {

        init();
    }

    void init()
    {
        cardsForThisCheckpoint = new List<CheckpointCard>();
        //cardsForThisCheckpoint.Clear();
        System.Array enumValArray = System.Enum.GetValues(typeof(VehicleTypeEnum));
        availVehicles = new List<VehicleTypeEnum>(enumValArray.Length);
        foreach (int val in enumValArray)
        {
            availVehicles.Add((VehicleTypeEnum)System.Enum.Parse(typeof(VehicleTypeEnum), val.ToString()));
        }


        getRandomCards(cardsForThisCheckpoint, 2);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public List<VehicleTypeEnum> getAvailableVehicles() {
        return availVehicles;
    }

    public List<CheckpointCard> getCards() {

        init();
        return cardsForThisCheckpoint;
    }


    public void getRandomCards(List<CheckpointCard> cardListToFill, int numToGen)
    {
        //hardcode for 2 right now cuz why not

        int lastPicked = -1;
        for (int i = 0; i < numToGen; i++) {
            int randInt = Random.Range(0, 7);
            if (randInt == lastPicked) { //prevent same card twice in a row
                i--;
                continue;
            } else {
                lastPicked = randInt;
            }
            GameObject uiCardPrefab = Resources.Load<GameObject>("Prefabs/UI/UiCard-" + randInt);
            // Debug.Log("Add vehicle to card deck: " + vehicleType);
            GameObject uiCardGo = Instantiate(uiCardPrefab, transform.position, Quaternion.identity);
            CheckpointCard checkpointCard = uiCardGo.GetComponent<CheckpointCard>();
            cardsForThisCheckpoint.Add(checkpointCard);
        }
        
        //AddNewVehicle(vehicleType, prefab, pos);
    }
}
