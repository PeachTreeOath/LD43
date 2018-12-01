using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointManager : MonoBehaviour {

    [SerializeField]
    private Canvas checkpointUi;

    [SerializeField]
    private List<Checkpoint> checkpoints;

    [SerializeField]
    private int curCheckpoint = -1;

    private bool showingCheckpointUi = false;

	// Use this for initialization
	void Start () {
        if (checkpointUi == null) {
            Debug.LogError("Missing checkpoint UI");
        }
        if (checkpoints == null || checkpoints.Count == 0) {
            Debug.LogError("Missing # of checkpoints");
        }

        hideCheckpointUi();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    //returns true if the checkpoint was hit OK
    //returns false if there wasn't a checkpoint to hit
    public bool hitCheckpoint() {
        bool cpHit = true;
        int cp = ++curCheckpoint;
        Debug.Log("Hit checkpoint " + cp);

        if (cp < 0 || cp >= checkpoints.Count) {
            Debug.LogError("Checkpoint ID not valid: " + cp);
            cpHit = false;
        } else {

            //TODO pause gameplay
            if (cp > 0) {
                showCheckpointUi();
            } else {
                resumeCheckpoint();
            }
        }
        return cpHit;

    }

    /// <summary>
    /// Shows the checkpoint ui for the current checkpoint
    /// </summary>
    private void showCheckpointUi() {
        Debug.Log("Show checkpoint UI");
        //TODO tie in with curCheckpoint index
        //to load proper card choices
        CanvasGroup cpCanvasGroup = checkpointUi.GetComponent<CanvasGroup>();
        cpCanvasGroup.alpha = 1;
        cpCanvasGroup.interactable = true;
        cpCanvasGroup.blocksRaycasts = true;
    }

    /// <summary>
    /// Hide the canvas for the checkpoint
    /// </summary>
    private void hideCheckpointUi() {
        CanvasGroup cpCanvasGroup = checkpointUi.GetComponent<CanvasGroup>();
        cpCanvasGroup.alpha = 0;
        cpCanvasGroup.interactable = false;
        cpCanvasGroup.blocksRaycasts = false;
    }

    /// <summary>
    /// Should be called when the current checkpoint resume
    /// has been requested. 
    /// </summary>
    public void resumeCheckpoint() {
        Debug.Log("Resume checkpoint");

        hideCheckpointUi();

        //TODO make this great
        dbgLoadUpJesusVanPool(curCheckpoint);
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
