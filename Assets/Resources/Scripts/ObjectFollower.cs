using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class ObjectFollower : MonoBehaviour
{
    public Transform target;

    Vector3 followOffset;

    void Start()
    {
        followOffset = gameObject.transform.position - target.position;
    }

    void LateUpdate()
    {
        gameObject.transform.position = target.position + followOffset;
    }
}
