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

    public GameObject obstacleTerrainPrefab;

    public GameObject obstacleMedianPrefab;

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

    #region Text
    public GameObject obituaryText;
    #endregion

    #region Sprites
    public Sprite captionBubble1;
    public Sprite captionBubble2;
    public Sprite captionBubble3;
    #endregion


    #region Stuff
    public GameObject burningFireFab;
    public GameObject prayerHandsFab;
    #endregion


    #region Rendering
    public GameObject vehicleWobble;
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
        obstacleTerrainPrefab = Resources.Load<GameObject>("Prefabs/HighwayObstruction");
        obstacleMedianPrefab = Resources.Load<GameObject>("Prefabs/Median");

        // Ui Elements
        vehicleSleepCaption = Resources.Load<GameObject>("Prefabs/VehicleSleepCaption");

        //Rendering
        vehicleWobble = Resources.Load<GameObject>("Prefabs/VehicleWobble");

        //Sprites
        captionBubble1 = Resources.Load<Sprite>("Textures/CaptionBubble1");
        captionBubble2 = Resources.Load<Sprite>("Textures/CaptionBubble2");
        captionBubble3 = Resources.Load<Sprite>("Textures/CaptionBubble3");

        // Vehicles
        bus = Resources.Load<GameObject>("Prefabs/Vehicles/Bus");
        car = Resources.Load<GameObject>("Prefabs/Vehicles/Car");
        limo = Resources.Load<GameObject>("Prefabs/Vehicles/Limo");
        sportsCar = Resources.Load<GameObject>("Prefabs/Vehicles/SportsCar");
        truck = Resources.Load<GameObject>("Prefabs/Vehicles/Truck");
        semiTruck = Resources.Load<GameObject>("Prefabs/Vehicles/SemiTruck");
        motorcycle = Resources.Load<GameObject>("Prefabs/Vehicles/Motorcycle");

        //Text
        obituaryText = Resources.Load("Prefabs/ObituaryText2") as GameObject;

        //Stuff
        burningFireFab = Resources.Load("Prefabs/BurningFire") as GameObject;
        prayerHandsFab = Resources.Load("Prefabs/PrayingHands") as GameObject;
    }
}