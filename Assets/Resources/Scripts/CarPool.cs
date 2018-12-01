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
        cars[0].GetComponent<CarController>().Select();
        AddNewCar(-2);
    }

    public void AddNewCar(float xPosition)
    {
        GameObject car = Instantiate(ResourceLoader.instance.car, new Vector3(xPosition, 0, 0), Quaternion.identity);
        car.transform.SetParent(transform);
        cars.Add(car);
        car.GetComponent<CarController>().id = cars.Count;
    }

    public void SelectCar(int selectedCarId)
    {
        foreach (GameObject car in cars)
        {
            CarController carController = car.GetComponent<CarController>();
            carController.isSelected = carController.id == selectedCarId;
        }
    }
}