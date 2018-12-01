using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointManager : MonoBehaviour {

    [SerializeField]
    private List<Checkpoint> checkpoints;

    [SerializeField]
    private int curCheckpoint = -1;

    public GameObject prayerHandsFab;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void hitCheckpoint() {
        int cp = ++curCheckpoint;
        Debug.Log("Hit checkpoint " + cp);

        if (cp < 0 || cp >= checkpoints.Count) {
            Debug.LogError("Checkpoint ID not valid: " + cp);
            return;  //do nothing
        }

        //TODO make this great
        dbgLoadUpJesusVanPool(cp);
        GainPrayers();
    }

    void GainPrayers()
    {
        VehiclePool vp = GameManager.instance.getVehiclePool();
        int prayerCount = 0;
        Debug.Log("vp.vehicles.Count: " + vp.vehicles.Count);
        for (int i = 0; i < vp.vehicles.Count; i++)
        {
            prayerCount += vp.vehicles[i].gameObject.GetComponent<VehicleStats>().prayerValue;
            for(int p = 0; p < vp.vehicles[i].gameObject.GetComponent<VehicleStats>().prayerValue; p++)
            {
                GameObject ph = Instantiate(prayerHandsFab) as GameObject;
                ph.transform.position = vp.vehicles[i].gameObject.transform.position + new Vector3(Random.Range(0f, 1f),
                                                                                                    Random.Range(0f, 1f), 
                                                                                                    -1);
                ph.transform.SetParent(vp.vehicles[i].gameObject.transform);
            }
        }
    }


    private void dbgLoadUpJesusVanPool(int cpIdx) {
        Debug.Log("Debug Spawn Stuff");
        Checkpoint cp = checkpoints[cpIdx];
        //for now we will just dump out
        //everything that is avaialable
        List<VehicleTypeEnum> vs = cp.getAvailableVehicles();

        VehiclePool vp = GameManager.instance.getVehiclePool();

        foreach (VehicleTypeEnum vte in vs) {
            vp.AddNewVehicle(vte);
        }
        
    }


}
