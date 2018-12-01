using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceLoader : Singleton<ResourceLoader>
{
    [HideInInspector]
    public GameObject obstacleConePrefab;

    [HideInInspector]
    public GameObject car;

    protected override void Awake()
    {
        base.Awake();
        LoadResources();
    }

    private void LoadResources()
    {
        obstacleConePrefab = Resources.Load<GameObject>("Prefabs/ObstacleCone");

        car = Resources.Load<GameObject>("Prefabs/Car");
    }
}