using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarPool : MonoBehaviour
{
	[Tooltip("All of the Jesus cars.")]
    public List <GameObject> cars;

    // Use this for initialization
    void Start()
    {
        AddNewCar(0);
        cars[0].GetComponent<CarController>().isSelected = true;
        AddNewCar(-2);
    }

    public void AddNewCar(float xPosition)
    {
        GameObject car = Instantiate(ResourceLoader.instance.car, new Vector3(xPosition, 0, 0), Quaternion.identity);
        cars.Add(car);
    }

    public void SelectCar(int selectedCarId)
    {
        foreach (GameObject car in cars)
        {
            CarController carController = GetComponent<CarController>();
            carController.isSelected = carController.id == selectedCarId;
        }
    }
}