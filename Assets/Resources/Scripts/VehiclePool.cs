using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class VehiclePool : Singleton<VehiclePool>
{
    public const float finalSpawnYPosition = 0;

	[Tooltip("All of the Jesus vehicles.")]
    public List <VehicleController> vehicles;

    // Use this for initialization
    void Start()
    {
        AddNewVehicle(VehicleTypeEnum.CAR, ResourceLoader.instance.car, 0);
        vehicles[0].GetComponent<VehicleController>().isSelected = true;

        AddNewVehicle(VehicleTypeEnum.BUS, ResourceLoader.instance.bus, -2);
        AddNewVehicle(VehicleTypeEnum.LIMO, ResourceLoader.instance.limo, -4);
        AddNewVehicle(VehicleTypeEnum.SPORTS_CAR, ResourceLoader.instance.sportsCar, -6);
        AddNewVehicle(VehicleTypeEnum.TRUCK, ResourceLoader.instance.truck, 2);
        AddNewVehicle(VehicleTypeEnum.SEMI_TRUCK, ResourceLoader.instance.semiTruck, 4);
        AddNewVehicle(VehicleTypeEnum.MOTORCYLE, ResourceLoader.instance.motorcycle, 6);
    }

    public void AddNewVehicle(VehicleTypeEnum vehicleType, GameObject vehiclePrefab, float xPosition)
    {
        // Get vehicle stats - if possible.
        VehicleStats[] vehicleStats = LevelManager.instance.GetComponents<VehicleStats>();
        List<VehicleStats> matchingStat = (from stat in vehicleStats where stat.vehicleType == vehicleType select stat).ToList();
        if (matchingStat.Count != 1)
        {
            Debug.LogWarning("Couldn't find matching vehicle stat for " + vehicleType + " or found multiple.");
            return;
        }

        GameObject vehicleGO = Instantiate(vehiclePrefab, new Vector3(xPosition, finalSpawnYPosition, 0), Quaternion.identity);
        vehicleGO.transform.SetParent(transform);

        VehicleController vehicleController = vehicleGO.GetComponent<VehicleController>();
        vehicleController.vehicleStats = matchingStat[0];

        Rigidbody2D rb2d = vehicleGO.GetComponent<Rigidbody2D>();
        rb2d.mass = matchingStat[0].weight * 50;
        rb2d.drag = matchingStat[0].weight * 10f;

        vehicles.Add(vehicleController);
    }

    public void SelectVehicle(VehicleController controllerToSelect)
    {
        foreach (var vehicleController in vehicles) {
            vehicleController.isSelected = false;
        }

        controllerToSelect.isSelected = true;
    }
}