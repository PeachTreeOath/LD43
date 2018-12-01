using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointManager : MonoBehaviour {

    [SerializeField]
    private List<Checkpoint> checkpoints;

    [SerializeField]
    private int curCheckpoint = -1;

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
