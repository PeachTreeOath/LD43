using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleStats : MonoBehaviour 
{
	public ObstacleTypeEnum obstacleType;

    [Tooltip("Whether or not the obstacle is allowed to spawn")]
    public bool allowSpawn = false;

	[Tooltip("How soon from start up should the first obstacle spawn - 0 means right away.")]
	public float firstSpawnTime;

	[Tooltip("How frequently obstacles should spawn after start.")]
	public float spawnInterval;

    [Tooltip("What is the visual obstacle telegraph?")]
    public GameObject spawnTelegraph;

	[Tooltip("How fast the obstacle should drop. E.g. if bird fell from the sky it could drop very fast.")]
	public float verticalSpeed;

    [Tooltip("Speed variation for different obstacles/vehicles (random).")]
    public float verticalRange;

    [Tooltip("How fast the obstacle should walk across the road.")]
    public float horizontalSpeed;

    [Tooltip("How long to wait before obstacle starts moving towards map.")]
	public float moveWaitTime;

	public DirectionEnum directionObstacleComingFrom;

    [HideInInspector]
	public float spawnTimer;
	
    [HideInInspector]
	public float obstacleHeight;
}