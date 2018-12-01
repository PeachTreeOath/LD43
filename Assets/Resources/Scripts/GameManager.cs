using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public Vector2 upperLeftBound;
    public Vector2 bottomRightBound;

    public float roadSpeed;

    public GameObject lightShaftsFab;

    private CheckpointManager checkpointManager;
    private VehiclePool vehiclePool;

    private float curPos = 0;
    private float nextCheckpointPos = 11; //TODO relocate to Level object

    private int dbgCount = 0;

    void Start() {
        vehiclePool = FindObjectOfType<VehiclePool>();
        Debug.Log("Loaded VehiclePool: " + vehiclePool.gameObject.name);
        checkpointManager = FindObjectOfType<CheckpointManager>();
        Debug.Log("Loaded CheckpointManager: " + checkpointManager.gameObject.name);

        //start'er up
        hitCheckpoint();
    }

    void Update() {
        updateAmountMoved();
        //track distance travelled along map
        //see if we hit a checkpoint
        //currently assuming a linear progression along Y axis just to keep it simple
        //TODO offload this to the specific Level that is loaded
        float curPos = getCurrentMapPos();
        float distToNextCheckpoint = getNextCheckpointPos() - curPos;
        if (dbgCount++ % 120 == 0) {
            //Debug.Log("Dist to checkpoint: " + distToNextCheckpoint);
        }
        if (distToNextCheckpoint <= 0) {
            Debug.Log("CHECKPOINT BABY");
            hitCheckpoint();
            nextCheckpointPos += 11; //TODO remove this, for debugging only
        }
    }

    private void updateAmountMoved() {
        curPos += 2 * Time.deltaTime; //TODO needs to be based on current speed
    }

    private float getCurrentMapPos() {
        return curPos;
    }

    private float getNextCheckpointPos() {
        return nextCheckpointPos;
    }

    public void hitCheckpoint() {
        checkpointManager.hitCheckpoint();
    }

    public VehiclePool getVehiclePool() {
        return vehiclePool;
    }
}