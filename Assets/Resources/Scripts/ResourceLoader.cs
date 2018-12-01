using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceLoader : Singleton<ResourceLoader>
{
    [HideInInspector]
    public GameObject obstacleConePrefab;

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