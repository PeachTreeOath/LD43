using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarPool : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        GameObject car = Instantiate(ResourceLoader.instance.car);
        car.transform.SetParent(transform);
    }
}
