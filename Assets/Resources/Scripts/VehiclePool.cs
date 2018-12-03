using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class VehiclePool : MonoBehaviour
{
    public const float finalSpawnYPosition = 0;

    [Tooltip("All of the Jesus vehicles.")]
    public List<VehicleController> vehicles;

    [HideInInspector]
    public List<GameObject> crashedVehicles;

    // Use this for initialization
    void Start()
    {
        crashedVehicles = new List<GameObject>();
        //Load vehicles into the map via checkpoint/ checkpoint mgr
        /*
        AddNewVehicle(VehicleTypeEnum.CAR, ResourceLoader.instance.car, 0);
        vehicles[0].GetComponent<VehicleController>().isSelected = true;

        AddNewVehicle(VehicleTypeEnum.BUS, ResourceLoader.instance.bus, -2);
        AddNewVehicle(VehicleTypeEnum.LIMO, ResourceLoader.instance.limo, -4);
        AddNewVehicle(VehicleTypeEnum.SPORTS_CAR, ResourceLoader.instance.sportsCar, -6);
        AddNewVehicle(VehicleTypeEnum.TRUCK, ResourceLoader.instance.truck, 2);
        AddNewVehicle(VehicleTypeEnum.SEMI_TRUCK, ResourceLoader.instance.semiTruck, 4);
        AddNewVehicle(VehicleTypeEnum.MOTORCYLE, ResourceLoader.instance.motorcycle, 6);
        */
    }

    public void AddNewVehicle(VehicleTypeEnum vehicleType)
    {
        Debug.Log("Add vehicle: " + vehicleType);
        GameObject prefab = null;
        int pos = 0; //TODO pos should be provided by caller
        switch (vehicleType)
        {
            case VehicleTypeEnum.CAR:
                prefab = ResourceLoader.instance.car;
                pos = 0;
                break;

            case VehicleTypeEnum.BUS:
                prefab = ResourceLoader.instance.bus;
                pos = -2;
                break;

            case VehicleTypeEnum.LIMO:
                prefab = ResourceLoader.instance.limo;
                pos = -4;
                break;

            case VehicleTypeEnum.SPORTS_CAR:
                prefab = ResourceLoader.instance.sportsCar;
                pos = -6;
                break;

            case VehicleTypeEnum.TRUCK:
                prefab = ResourceLoader.instance.truck;
                pos = 2;
                break;

            case VehicleTypeEnum.SEMI_TRUCK:
                prefab = ResourceLoader.instance.semiTruck;
                pos = 4;
                break;

            case VehicleTypeEnum.MOTORCYLE:
                prefab = ResourceLoader.instance.motorcycle;
                pos = 6;
                break;

            default:
                Debug.Log("You done fukt up a-a-ron");
                break;
        }
        AddNewVehicle(vehicleType, prefab, pos);
    }

    public void AddNewVehicle(VehicleTypeEnum vehicleType, GameObject vehiclePrefab, float xPosition) {
        float startX = Random.Range(-6, 6);
        float duration = Random.Range(2.75f, 4.25f);

        Vector2 position = new Vector2(xPosition, finalSpawnYPosition);

        SpawnVehicleWithAnimation(vehicleType, vehiclePrefab, position, startX, duration);

        if(vehicleType == VehicleTypeEnum.MOTORCYLE) {
            var offset1 = new Vector2(-0.35f, -0.3f);
            var offset2 = new Vector2(0.35f, -0.3f);
            SpawnVehicleWithAnimation(vehicleType, vehiclePrefab, position + offset1, startX + offset1.x, duration);
            SpawnVehicleWithAnimation(vehicleType, vehiclePrefab, position + offset2, startX + offset2.x, duration);
        }
    }

    private void SpawnVehicleWithAnimation(VehicleTypeEnum vehicleType, GameObject vehiclePrefab, Vector2 position, float startX, float duration) {
        VehicleStats[] vehicleStats = LevelManager.instance.GetComponents<VehicleStats>();
        List<VehicleStats> matchingStat = (from stat in vehicleStats where stat.vehicleType == vehicleType select stat).ToList();
        if (matchingStat.Count != 1) {
            Debug.LogWarning("Couldn't find matching vehicle stat for " + vehicleType + " or found multiple.");
        }

        var vehicleGO = Instantiate(vehiclePrefab, position, Quaternion.identity);
        vehicleGO.transform.SetParent(transform);
        VehicleStats vs = vehicleGO.AddComponent<VehicleStats>();
        vs.awakeGracePeriod = matchingStat[0].awakeGracePeriod;
        vs.vehicleType = matchingStat[0].vehicleType;
        vs.weight = matchingStat[0].weight;
        vs.control = matchingStat[0].control;
        vs.speed = matchingStat[0].speed;
        vs.prayerValue = matchingStat[0].prayerValue;
        vs.sleepChance = matchingStat[0].sleepChance;
        vs.sleepSeverity = matchingStat[0].sleepSeverity;


        var vehicle = vehicleGO.GetComponent<VehicleController>();
        vehicle.vehicleStats = matchingStat[0];

        Rigidbody2D rb2d = vehicleGO.GetComponent<Rigidbody2D>();
        rb2d.mass = matchingStat[0].weight * 50;
        rb2d.drag = matchingStat[0].weight * 10f;

        MiracleAnimator myMiracle = vehicle.gameObject.AddComponent<MiracleAnimator>();
        myMiracle.beginMiracles(new Vector2(startX, -Camera.main.orthographicSize), vehicle.transform.position, duration);

        vehicles.Add(vehicle);
    }

    public void UnselectAllVehicles()
    {
        //Unselect all other controllers
        foreach (var vehicleController in vehicles)
        {
            vehicleController.SetSelected(false);
        }
    }

    public void SelectVehicle(VehicleController controllerToSelect)
    {
        if (controllerToSelect != null)
        {
            if (controllerToSelect.IsCrashed) return;

            controllerToSelect.SetSelected(true);
        }

        //Unselect all other controllers
        foreach (var vehicleController in vehicles)
        {
            if (controllerToSelect != vehicleController)
            {
                vehicleController.SetSelected(false);
            }
        }

        JesusManager.instance.SelectAVehicle(controllerToSelect);
    }

    public void OnVehicleCrash(VehicleController crashedVehicle, bool fatal)
    {
        if (crashedVehicle == null) return;

        if (crashedVehicle.isSelected)
        {
            JesusManager.instance.VehicleCrashedLetGo(crashedVehicle);
            SelectVehicle(null);
        }

        vehicles.Remove(crashedVehicle);

        if (fatal)
        {
            Debug.Log(crashedVehicle.gameObject.name + " crashed");
            crashedVehicles.Add(crashedVehicle.gameObject);
            CheckForGameOver();
        }
    }

    void CheckForGameOver()
    {
        if (vehicles.Count == 0 &&
            gameObject.GetComponent<CarnageViewer>() == null)
        {
            gameObject.AddComponent<CarnageViewer>();
            GameManager.instance.GameOverVehicleDeath();
        }
    }

    public int getNumWorkingCars()
    {
        return vehicles.Count;
    }
}