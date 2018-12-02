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

        cardsForThisCheckpoint = new List<CheckpointCard>();
        //cardsForThisCheckpoint.Clear();
        System.Array enumValArray = System.Enum.GetValues(typeof(VehicleTypeEnum));
        availVehicles = new List<VehicleTypeEnum>(enumValArray.Length);
        foreach (int val in enumValArray)
        {
            availVehicles.Add((VehicleTypeEnum)System.Enum.Parse(typeof(VehicleTypeEnum), val.ToString()));
        }


        cardsForThisCheckpoint.Add(getRandomCard());
        cardsForThisCheckpoint.Add(getRandomCard());
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



    public CheckpointCard getRandomCard()
    {
        int randInt = Random.Range(0, 6);
        GameObject uiCardPrefab = Resources.Load<GameObject>("Prefabs/UI/UiCard-" + randInt);

        // Debug.Log("Add vehicle to card deck: " + vehicleType);

        GameObject uiCardGo = Instantiate(uiCardPrefab, transform.position, Quaternion.identity);
        CheckpointCard checkpointCard = uiCardGo.GetComponent<CheckpointCard>();

        return checkpointCard;

        
        
        //AddNewVehicle(vehicleType, prefab, pos);
    }
}
