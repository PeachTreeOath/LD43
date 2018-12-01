using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public Vector2 upperLeftBound;
    public Vector2 bottomRightBound;

    public float roadSpeed;

    private CheckpointManager checkpointManager;
    private VehiclePool vehiclePool;

    void Start() {
        vehiclePool = FindObjectOfType<VehiclePool>();
        Debug.Log("Loaded VehiclePool: " + vehiclePool.gameObject.name);
        checkpointManager = FindObjectOfType<CheckpointManager>();
        Debug.Log("Loaded CheckpointManager: " + checkpointManager.gameObject.name);

        //start'er up
        hitCheckpoint();
    }

    public void hitCheckpoint() {
        checkpointManager.hitCheckpoint();
    }

    public VehiclePool getVehiclePool() {
        return vehiclePool;
    }
}