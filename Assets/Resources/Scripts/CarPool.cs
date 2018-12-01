using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarPool : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        GameObject car = Instantiate(ResourceLoader.instance.car);
        GameObject car2 = Instantiate(ResourceLoader.instance.car);
        car2.transform.position = new Vector2(-2, 0);

        car.transform.SetParent(transform);
    }
}
