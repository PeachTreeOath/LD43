using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleStats : MonoBehaviour 
{
	public ObstacleTypeEnum obstacleType;

	[Tooltip("How soon from start up should the first obstacle spawn - 0 means right away.")]
	public float firstSpawnTime;

	[Tooltip("How frequently cones should spawn after start.")]
	public float spawnInterval;

    [Tooltip("What is the visual obstacle telegraph?")]
    public GameObject spawnTelegraph;

    [HideInInspector]
	public float spawnTimer;
	
    [HideInInspector]
	public float obstacleHeight;
}