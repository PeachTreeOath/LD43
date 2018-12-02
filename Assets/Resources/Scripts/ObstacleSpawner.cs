using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Spawns obstacles at a random location in set intervals.
public class ObstacleSpawner : MonoBehaviour 
{
	[Tooltip("All the different types of obstacles to spawn - loaded from LevelManager.")]
	public ObstacleStats[] allObstacleTypes;

    private bool isPaused = false;

	public void Start ()
	{
		allObstacleTypes = LevelManager.instance.GetComponents<ObstacleStats>();

		foreach (ObstacleStats obstacleStats in allObstacleTypes)
		{
            // Get obstacle heights so we can spawn them above bounds.
            GameObject obstacleGO = null;
            obstacleGO = GetPrefab(obstacleStats);

			if (obstacleGO != null)
			{
				obstacleStats.obstacleHeight = obstacleGO.GetComponent<SpriteRenderer>().sprite.bounds.size.y * obstacleGO.transform.localScale.y;
			}

			obstacleStats.spawnTimer = obstacleStats.firstSpawnTime;
		}
	}

    private GameObject GetPrefab(ObstacleStats stats)
    {
        GameObject obstacleGO = null;
        switch (stats.obstacleType) {
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
            default:
                Debug.LogWarning("Couldn't find matching GameObject in ResourceLoader for obstacle of type " + stats.obstacleType);
                break;
        }
        return obstacleGO;
    }

    public void Update() {
        if (!isPaused) {
            UpdateObstacles();
        }
    }

    public void pause() {
        isPaused = true;
    }

    public void resume() {
        isPaused = false;
    }

	public void UpdateObstacles ()
	{
		foreach (ObstacleStats obstacleStats in allObstacleTypes)
		{
            if(!obstacleStats.allowSpawn) {
                obstacleStats.spawnTimer = obstacleStats.spawnInterval;
            }
            else if (obstacleStats.spawnTimer <= 0)
			{
				// Spawn new obstacle at top of the stage.
				float randomX = UnityEngine.Random.Range(GameManager.instance.upperLeftBound.x, GameManager.instance.bottomRightBound.x);
				float randomY = UnityEngine.Random.Range(GameManager.instance.upperLeftBound.y, GameManager.instance.bottomRightBound.y);
				Vector3 startPosition = new Vector3(randomX, GameManager.instance.upperLeftBound.y + obstacleStats.obstacleHeight, 0);
				Vector3 endPosition = new Vector3(randomX, randomY, 0);

                GameObject obstacleGO = null;
                obstacleGO = GetPrefab(obstacleStats);

                GameObject obstacle = Instantiate(obstacleGO, startPosition, Quaternion.identity);
				obstacle.GetComponent<ObstacleController>().endPosition = endPosition;
				obstacle.GetComponent<ObstacleController>().obstacleStats = obstacleStats;
			
				obstacleStats.spawnTimer = obstacleStats.spawnInterval;
			}
			else
			{
				obstacleStats.spawnTimer -= Time.deltaTime;
			}
		}
	}
}