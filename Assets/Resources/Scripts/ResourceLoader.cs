using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceLoader : Singleton<ResourceLoader>
{
    [HideInInspector]
    public GameObject obstacleConePrefab;

    public GameObject obstaclePedestrianPrefab;

    public GameObject obstacleCyclistPrefab;

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
        obstacleConePrefab = Resources.Load<GameObject>("Prefabs/ObstacleCone");
        obstaclePedestrianPrefab = Resources.Load<GameObject>("Prefabs/ObstaclePedestrian");
        obstacleCyclistPrefab = Resources.Load<GameObject>("Prefabs/ObstacleCyclist");

        // Vehicles
        bus = Resources.Load<GameObject>("Prefabs/Bus");
        car = Resources.Load<GameObject>("Prefabs/Car");
        limo = Resources.Load<GameObject>("Prefabs/Limo");
        sportsCar = Resources.Load<GameObject>("Prefabs/SportsCar");
        truck = Resources.Load<GameObject>("Prefabs/Truck");
        semiTruck = Resources.Load<GameObject>("Prefabs/SemiTruck");
        motorcycle = Resources.Load<GameObject>("Prefabs/Motorcycle");
    }
}