﻿using System.Collections;
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
        bus = Resources.Load<GameObject>("Prefabs/Vehicles/Bus");
        car = Resources.Load<GameObject>("Prefabs/Vehicles/Car");
        limo = Resources.Load<GameObject>("Prefabs/Vehicles/Limo");
        sportsCar = Resources.Load<GameObject>("Prefabs/Vehicles/SportsCar");
        truck = Resources.Load<GameObject>("Prefabs/Vehicles/Truck");
        semiTruck = Resources.Load<GameObject>("Prefabs/Vehicles/SemiTruck");
        motorcycle = Resources.Load<GameObject>("Prefabs/Vehicles/Motorcycle");
    }
}