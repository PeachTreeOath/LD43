using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehiclePool : MonoBehaviour
{
    public const float finalSpawnYPosition = 0;

	[Tooltip("All of the Jesus vehicles.")]
    public List <GameObject> vehicles;

    // Use this for initialization
    void Start()
    {
        AddNewVehicle(ResourceLoader.instance.car, 0);
        vehicles[0].GetComponent<VehicleController>().isSelected = true;

        AddNewVehicle(ResourceLoader.instance.bus, -2);
        AddNewVehicle(ResourceLoader.instance.limo, -4);
        AddNewVehicle(ResourceLoader.instance.sportsCar, -6);
        AddNewVehicle(ResourceLoader.instance.truck, 2);
        AddNewVehicle(ResourceLoader.instance.semiTruck, 4);
        AddNewVehicle(ResourceLoader.instance.motorcycle, 6);
    }

    public void AddNewVehicle(GameObject vehicleType, float xPosition)
    {
        GameObject vehicle = Instantiate(vehicleType, new Vector3(xPosition, finalSpawnYPosition, 0), Quaternion.identity);
        vehicle.transform.SetParent(transform);
        vehicles.Add(vehicle);
        vehicle.GetComponent<VehicleController>().id = vehicles.Count;
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