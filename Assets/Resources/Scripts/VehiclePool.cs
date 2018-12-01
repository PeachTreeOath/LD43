using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class VehiclePool : MonoBehaviour
{
    public const float finalSpawnYPosition = 0;

	[Tooltip("All of the Jesus vehicles.")]
    public List <GameObject> vehicles;

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

    public void AddNewVehicle(VehicleTypeEnum vehicleType, GameObject vehicleGO, float xPosition)
    {
        // Get vehicle states - if possible.
        VehicleStats[] vehicleStats = LevelManager.instance.GetComponents<VehicleStats>();
        List<VehicleStats> matchingStat = (from stat in vehicleStats where stat.vehicleType == vehicleType select stat).ToList();
        if (matchingStat.Count != 1)
        {
            Debug.LogWarning("Couldn't find matching vehicle stat for " + vehicleType + " or found multiple.");
            return;
        }

        GameObject vehicle = Instantiate(vehicleGO, new Vector3(xPosition, finalSpawnYPosition, 0), Quaternion.identity);
        vehicle.transform.SetParent(transform);
        vehicles.Add(vehicle);

        VehicleController vehicleController = vehicle.GetComponent<VehicleController>();
        vehicleController.id = vehicles.Count;
        vehicleController.vehicleStats = matchingStat[0];
    }

    public void SelectCar(int selectedCarId)
    {
        foreach (GameObject car in vehicles)
        {
            VehicleController vehicleController = car.GetComponent<VehicleController>();
            vehicleController.isSelected = vehicleController.id == selectedCarId;
        }
    }
}