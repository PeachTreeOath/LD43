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

    protected virtual void Start() {
        var obstacleGO = GetObstaclePrefab();

        if (obstacleGO != null)
        {
            obstacleHeight = obstacleGO.GetComponent<SpriteRenderer>().sprite.bounds.size.y * obstacleGO.transform.localScale.y;
        }

		spawnTimer = firstSpawnTime;
    }

    //TODO passing this in is clumsy
    public virtual void Spawn(Vector2 startPosition, Vector2 endPosition) {
        var obstacleGO = GetObstaclePrefab();
        var obstacle = Instantiate(obstacleGO, startPosition, Quaternion.identity).GetComponent<ObstacleController>();
        obstacle.endPosition = endPosition;
        obstacle.obstacleStats = this;
    }

    protected GameObject GetObstaclePrefab()
    {
        GameObject obstacleGO = null;
        switch (obstacleType) {
            case ObstacleTypeEnum.CONE:
                obstacleGO = ResourceLoader.instance.obstacleConePrefab;
                break;
            case ObstacleTypeEnum.PEDESTRIAN:
                obstacleGO = ResourceLoader.instance.obstaclePedestrianPrefab;
                break;
            case ObstacleTypeEnum.CYCLIST:
                obstacleGO = ResourceLoader.instance.obstacleCyclistPrefab;
                break;
            case ObstacleTypeEnum.TERRAIN:
                obstacleGO = ResourceLoader.instance.obstacleTerrainPrefab;
                break;
            case ObstacleTypeEnum.MEDIAN:
                obstacleGO = ResourceLoader.instance.obstacleMedianPrefab;
                break;
            default:
                Debug.LogWarning("Couldn't find matching GameObject in ResourceLoader for obstacle of type " + obstacleType);
                break;
        }
        return obstacleGO;
    }
}