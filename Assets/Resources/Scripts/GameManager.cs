using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public Vector2 upperLeftBound;
    public Vector2 bottomRightBound;
	public float defaultDeltaFromTopForWarnings = 0.3f;
	public float defaultDeltaFromRightForWarnings = 1;

    public float roadSpeed;

    public GameObject lightShaftsFab;

    private Scroller scroller;
    private ObstacleSpawner obstacleSpawner;
    private CheckpointManager checkpointManager;
    private VehiclePool vehiclePool;
    PrayerMeter prayerMeter;

    private float curPos = 0;
    private float nextCheckpointPos = 110; //TODO relocate to Level object
    private bool isPausedForCheckpoint = false;

    private int dbgCount = 0;

    void Start() {
        vehiclePool = FindObjectOfType<VehiclePool>();
        Debug.Log("Loaded VehiclePool: " + vehiclePool.gameObject.name);
        checkpointManager = FindObjectOfType<CheckpointManager>();
        Debug.Log("Loaded CheckpointManager: " + checkpointManager.gameObject.name);
        prayerMeter = FindObjectOfType<PrayerMeter>();
        scroller = FindObjectOfType<Scroller>();
        Debug.Log("Loaded Scroller: " + scroller.gameObject.name);
        obstacleSpawner = FindObjectOfType<ObstacleSpawner>();
        Debug.Log("Loaded Obstacle Spawner: " + obstacleSpawner);

        //start'er up
        hitCheckpoint();
        resumeForCheckpoint(); //For debug only
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
            nextCheckpointPos += 110; //TODO remove this, for debugging only
        }
    }

    private void updateAmountMoved() {
        if (!isPausedForCheckpoint) {
            curPos += 2 * Time.deltaTime; //TODO needs to be based on current speed
        }
    }

    private float getCurrentMapPos() {
        return curPos;
    }

    private float getNextCheckpointPos() {
        return nextCheckpointPos;
    }

    /// <summary>
    /// Callback for an action that resumes from the checkpoint screen
    /// </summary>
    public void resumeCheckpoint() {
        checkpointManager.resumeCheckpoint();
        resumeForCheckpoint();
    }
        


    /// <summary>
    /// Triggers the checkpoint display and associated pausing actions
    /// </summary>
    public void hitCheckpoint() {
        pauseForCheckpoint();
        if (!checkpointManager.hitCheckpoint()) {
            //no checkpoint, keep going
            resumeForCheckpoint();
        }
    }

    /// <summary>
    /// Stops all the crap that needs to wait for a checkpoint
    /// </summary>
    private void pauseForCheckpoint() {
        isPausedForCheckpoint = true;
        scroller.pause();
        obstacleSpawner.pause();
    }

    /// <summary>
    /// Resumes all the crap stopped for a checkpoint
    /// </summary>
    private void resumeForCheckpoint() {
        scroller.resume();
        isPausedForCheckpoint = false;
        obstacleSpawner.resume();
    }

    public bool isPaused() {
        return isPausedForCheckpoint;
    }

    public VehiclePool getVehiclePool() {
        return vehiclePool;
    }

    public PrayerMeter GetPrayerMeter()
    {
        return prayerMeter;
    }
}