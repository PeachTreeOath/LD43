using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceLoader : Singleton<ResourceLoader>
{
    #region Obstacles
    [HideInInspector]
    public GameObject obstacleConePrefab;

    public GameObject obstaclePedestrianPrefab;

    public GameObject obstacleCyclistPrefab;

    public GameObject vehicleSleepCaption;
    #endregion

    #region Vehicles
    [HideInInspector]
    public GameObject bus;

    [HideInInspector]
    public GameObject car;

    [HideInInspector]
    public GameObject limo;

    [HideInInspector]
    public GameObject sportsCar;

    [HideInInspector]
    public GameObject truck;

    [HideInInspector]
    public GameObject semiTruck;

    [HideInInspector]
    public GameObject motorcycle;

    #endregion

    protected override void Awake()
    {
        base.Awake();
        LoadResources();
    }

    private void LoadResources()
    {
        // Obstacles
        obstacleConePrefab = Resources.Load<GameObject>("Prefabs/ObstacleCone");
        obstaclePedestrianPrefab = Resources.Load<GameObject>("Prefabs/ObstaclePedestrian");
        obstacleCyclistPrefab = Resources.Load<GameObject>("Prefabs/ObstacleCyclist");

        // Ui Elements
        vehicleSleepCaption = Resources.Load<GameObject>("Prefabs/VehicleSleepCaption");

        // Vehicles
        bus = Resources.Load<GameObject>("Prefabs/Vehicles/Bus");
        car = Resources.Load<GameObject>("Prefabs/Vehicles/Car");
        limo = Resources.Load<GameObject>("Prefabs/Vehicles/Limo");
        sportsCar = Resources.Load<GameObject>("Prefabs/Vehicles/SportsCar");
        truck = Resources.Load<GameObject>("Prefabs/Vehicles/Truck");
        semiTruck = Resources.Load<GameObject>("Prefabs/Vehicles/SemiTruck");
        motorcycle = Resources.Load<GameObject>("Prefabs/Vehicles/Motorcycle");
    }
}