﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager>
{
    public Vector2 upperLeftBound;
    public Vector2 bottomRightBound;
    public float defaultDeltaFromTopForWarnings = 5f;
    public float defaultDeltaFromRightForWarnings = 1;
    public bool isGameOver;
    public GameObject lightShaftsFab;

    [SerializeField]
    private Checkpoint startingCheckpoint; //loads starting vehicles

    [SerializeField]
    public float screenShakeDurationMs = 750;

    private Scroller scroller;
    public float scrollSpeedMultiplier;
    public float displayDistanceMultiplier;
    private PedestrianSpawner pedestrianSpawner;
    private ObstacleSpawner obstacleSpawner;
    private CheckpointManager checkpointManager;
    private VehiclePool vehiclePool;
    PrayerMeter prayerMeter;

    private float curPos = 0;
    private float nextCheckpointPos = 0; //TODO relocate to Level object
    private bool isPausedForCheckpoint = false;

    private float totalDistanceTraveled = 0;

    public bool isPrayerDeath() {
        return GameManager.instance.GetPrayerMeter().GetPrayer() < 0.1f;
    }

    //private int dbgCount = 0;

    //Note this will only start once ever because of the singleton dontdestroyonload
    void Start()
    {
        isGameOver = false;
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
        pedestrianSpawner = FindObjectOfType<PedestrianSpawner>();
        Debug.Log("Loaded Pedestrian Spawner: " + pedestrianSpawner);


        scroller.scrollSpeed = LevelManager.instance.scrollSpeed * scrollSpeedMultiplier;

        StartSceneThings();
    }

    public void StartSceneThings()
    {
        //start'er up
        //hitCheckpoint();
        //resumeForCheckpoint();
        nextCheckpointPos = curPos; //trigger starting checkpoint for realz

        if (startingCheckpoint == null)
        {
            Debug.LogWarning("No starting checkpoint on GameManager (optional) -- No starting vehicles will be created");
        }
        else
        {
            checkpointManager.dbgLoadUpJesusVanPool(startingCheckpoint);
        }
    }

    void Update()
    {
        updateAmountMoved();
        //track distance travelled along map
        //see if we hit a checkpoint
        float curPos = getCurrentMapPos();
        float distToNextCheckpoint = getNextCheckpointPos() - curPos;

        float displayDistance = distToNextCheckpoint * displayDistanceMultiplier;

        checkpointManager.UpdateCheckpointSignDistance(displayDistance);
        // if (dbgCount++ % 120 == 0) {
        //     Debug.Log("Dist to checkpoint: " + distToNextCheckpoint);
        // }
        if (distToNextCheckpoint <= 0)
        {
            Debug.Log("CHECKPOINT BABY");
            hitCheckpoint();
            nextCheckpointPos += LevelManager.instance.distanceBetweenCheckpoints;
        }
    }

    /// <summary>
    /// Use this method exclusively for changing scenes.  It will handle the proper reference
    /// updates needed before/after the scene load
    /// If a full reload of all objects is needed, destroyTheUndestroyables must be set to true,
    /// otherwise objects marked as DontDestroyOnLoad will not be reinitialized.
    /// </summary>
    public void loadScene(string sceneName, bool destroyTheUndestroyables)
    {
        if (destroyTheUndestroyables)
        {
            //this is kind of slow :<
            targetAllSingletonsForDestruction();
        }
        SceneManager.LoadScene(sceneName);
    }

    private void targetAllSingletonsForDestruction()
    {
        GameObject[] ss = GameObject.FindGameObjectsWithTag("singleton");
        //https://answers.unity.com/questions/18217/undoing-dontdestroyonload-without-immediately-dest.html
        //reparent the objects to force them to unload
        foreach (GameObject go in ss)
        {
            Debug.Log("Resetting DontDestroyOnLoad for " + go.name);
            GameObject tmpParent = new GameObject();
            go.transform.parent = tmpParent.transform;
        }
    }

    public void dbg_setDistToNextCheckpoint(float newDist)
    {
        Debug.Log("Debug - set dist to next checkpoint " + newDist);
        curPos = nextCheckpointPos - newDist;
    }

    private void updateAmountMoved()
    {
        if (!isPausedForCheckpoint)
        {
            float distance = LevelManager.instance.scrollSpeed * Time.deltaTime;
            curPos += distance;
            totalDistanceTraveled += distance;
        }
    }

    private float getCurrentMapPos()
    {
        return curPos;
    }

    private float getNextCheckpointPos()
    {
        return nextCheckpointPos;
    }

    /// <summary>
    /// Callback for an action that resumes from the checkpoint screen
    /// </summary>
    public void resumeCheckpoint()
    {
        checkpointManager.resumeCheckpoint();
        resumeForCheckpoint();
    }



    /// <summary>
    /// Triggers the checkpoint display and associated pausing actions
    /// </summary>
    public void hitCheckpoint()
    {
        pauseForCheckpoint();
        if (!checkpointManager.hitCheckpoint())
        {
            //no checkpoint, keep going
            resumeForCheckpoint();
        }
    }

    /// <summary>
    /// Stops all the crap that needs to wait for a checkpoint
    /// </summary>
    private void pauseForCheckpoint()
    {
        isPausedForCheckpoint = true;
        scroller.pause();
        obstacleSpawner.pause();
    }

    /// <summary>
    /// Resumes all the crap stopped for a checkpoint
    /// </summary>
    private void resumeForCheckpoint()
    {
        scroller.resume();
        isPausedForCheckpoint = false;
        obstacleSpawner.resume();
        nextCheckpointPos += LevelManager.instance.distanceBetweenCheckpoints;
        GameManager.instance.PrayerPowerRevive();

        AudioManager.instance.PlaySound("car_honk");
    }

    public bool isPaused()
    {
        return isPausedForCheckpoint;
    }

    public VehiclePool getVehiclePool()
    {
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

    public PedestrianSpawner GetPedestrianSpawner() {
        return pedestrianSpawner;
    }

    public MedianSpawner GetMedianSpawner()
    {
        return GameObject.FindObjectOfType<MedianSpawner>();
    }

    public void GameOverVehicleDeath()
    {
        CanvasGroup cGroup = GameObject.Find("GameOverCanvasGroup").GetComponent<CanvasGroup>();
        cGroup.alpha = 1;
        cGroup.interactable = true;
        cGroup.blocksRaycasts = true;
       // GameObject.Find("GameOverHeader").GetComponent<Text>().text = "Everyone died!";
        GameObject.Find("GameOverTip").GetComponent<Text>().text = "Total Distance Traveled:\n" + (totalDistanceTraveled * displayDistanceMultiplier).ToString("0.00") + " MI";
        GameOver();
    }

    public void GameOverPrayerPowerDeath()
    {
        GameObject.Find("VehiclePool").GetComponent<VehiclePool>().UnselectAllVehicles();
    }

    private void GameOver()
    {
        GameManager.instance.GetCheckPointManager().enabled = false;
        GameManager.instance.GetObstacleSpawner().enabled = false;
        GameManager.instance.GetPedestrianSpawner().enabled = false;
        GameManager.instance.GetMedianSpawner().enabled = false;
        GameManager.instance.GetPrayerMeter().enabled = false;
        GameManager.instance.enabled = false;
        GameObject.Find("EmptyPrayersText").SetActive(false);
        GameObject.Find("LowPrayersText").SetActive(false);
        isGameOver = true;
    }

    public void PrayerPowerRevive()
    {
        GameManager.instance.GetPrayerMeter().AddPrayer(0.2f * GameManager.instance.GetPrayerMeter().maxPrayers);
    }
}