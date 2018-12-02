﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public Vector2 upperLeftBound;
    public Vector2 bottomRightBound;
	public float defaultDeltaFromTopForWarnings = 5f;
	public float defaultDeltaFromRightForWarnings = 1;

    public GameObject lightShaftsFab;

    [SerializeField]
    private Checkpoint startingCheckpoint; //loads starting vehicles

    private Scroller scroller;
    public float scrollSpeedMultiplier;
    private ObstacleSpawner obstacleSpawner;
    private CheckpointManager checkpointManager;
    private VehiclePool vehiclePool;
    PrayerMeter prayerMeter;

    private float curPos = 0;
    private float nextCheckpointPos = 0; //TODO relocate to Level object
    private bool isPausedForCheckpoint = false;

    //private int dbgCount = 0;

    void Start() {

        //Loading managers, spawners, and UIElements
        vehiclePool = FindObjectOfType<VehiclePool>();
        Debug.Log("Loaded VehiclePool: " + vehiclePool.gameObject.name);
        checkpointManager = FindObjectOfType<CheckpointManager>();
        Debug.Log("Loaded CheckpointManager: " + checkpointManager.gameObject.name);
        prayerMeter = FindObjectOfType<PrayerMeter>();
        scroller = FindObjectOfType<Scroller>();
        Debug.Log("Loaded Scroller: " + scroller.gameObject.name);
        obstacleSpawner = FindObjectOfType<ObstacleSpawner>();
        Debug.Log("Loaded Obstacle Spawner: " + obstacleSpawner);

        scroller.scrollSpeed = LevelManager.instance.scrollSpeed * scrollSpeedMultiplier;

        //start'er up
        hitCheckpoint();
        resumeForCheckpoint();

        if (startingCheckpoint == null) {
            Debug.LogError("No starting checkpoint, you are not going to have any cars");
        } else {
            checkpointManager.dbgLoadUpJesusVanPool(startingCheckpoint);
        }
    }

    void Update() {
        updateAmountMoved();
        //track distance travelled along map
        //see if we hit a checkpoint
        float curPos = getCurrentMapPos();
        float distToNextCheckpoint = getNextCheckpointPos() - curPos;

        checkpointManager.UpdateCheckpointSignDistance((int) distToNextCheckpoint);
        // if (dbgCount++ % 120 == 0) {
        //     Debug.Log("Dist to checkpoint: " + distToNextCheckpoint);
        // }
        if (distToNextCheckpoint <= 0) {
            Debug.Log("CHECKPOINT BABY");
            hitCheckpoint();
            nextCheckpointPos += LevelManager.instance.distanceBetweenCheckpoints;
        }
    }

    public void dbg_setDistToNextCheckpoint(float newDist) {
        Debug.Log("Debug - set dist to next checkpoint " + newDist);
        curPos = nextCheckpointPos - newDist;
    }

    private void updateAmountMoved() {
        if (!isPausedForCheckpoint) {
            curPos += LevelManager.instance.scrollSpeed * Time.deltaTime; 
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
        nextCheckpointPos += LevelManager.instance.distanceBetweenCheckpoints;
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

    public Scroller GetScroller()
    {
        return scroller;
    }

    public CheckpointManager GetCheckPointManager()
    {
        return checkpointManager;
    }

    public ObstacleSpawner GetObstacleSpawner()
    {
        return obstacleSpawner;
    }
}