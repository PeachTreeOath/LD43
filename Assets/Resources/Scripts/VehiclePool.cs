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
        AddNewCar(0);
        vehicles[0].GetComponent<VehicleController>().Select();
        AddNewCar(-2);
    }

    public void AddNewCar(float xPosition)
    {
        GameObject car = Instantiate(ResourceLoader.instance.car, new Vector3(xPosition, finalSpawnYPosition, 0), Quaternion.identity);
        car.transform.SetParent(transform);
        vehicles.Add(car);
        car.GetComponent<VehicleController>().id = vehicles.Count;
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