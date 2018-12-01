using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Spawns obstacles at a random location in set intervals.
public class ObstacleSpawner : MonoBehaviour 
{
	[Tooltip("All the different types of obstacles to spawn - loaded from LevelManager.")]
	public ObstacleStats[] allObstacleTypes;

	public void Start ()
	{
		allObstacleTypes = LevelManager.instance.GetComponents<ObstacleStats>();

		foreach (ObstacleStats obstacleStats in allObstacleTypes)
		{
			// Get obstacle heights so we can spawn them above bounds.
			GameObject obstacleGO = null;
			switch (obstacleStats.obstacleType)
			{
				case ObstacleTypeEnum.CONE:
					obstacleGO = ResourceLoader.instance.obstacleConePrefab;
					break;
                case ObstacleTypeEnum.PEDESTRIAN:
                    obstacleGO = ResourceLoader.instance.obstaclePedestrianPrefab;
                    break;
                case ObstacleTypeEnum.CYCLIST:
                    obstacleGO = ResourceLoader.instance.obstacleCyclistPrefab;
                    break;
                default:
					Debug.LogWarning("Couldn't find matching GameObject in ResourceLoader for obstacle of type " + obstacleStats.obstacleType);
					break;
			}

			if (obstacleGO != null)
			{
				obstacleStats.obstacleHeight = obstacleGO.GetComponent<SpriteRenderer>().sprite.bounds.size.y;
			}

			obstacleStats.spawnTimer = obstacleStats.firstSpawnTime;
		}
	}
	
	public void Update ()
	{
		foreach (ObstacleStats obstacleStats in allObstacleTypes)
		{
			if (obstacleStats.spawnTimer <= 0)
			{
				// Spawn new obstacle at top of the stage.
				float randomX = UnityEngine.Random.Range(GameManager.instance.upperLeftBound.x, GameManager.instance.bottomRightBound.x);
				float randomY = UnityEngine.Random.Range(GameManager.instance.upperLeftBound.y, GameManager.instance.bottomRightBound.y);
				Vector3 startPosition = new Vector3(randomX, GameManager.instance.upperLeftBound.y + obstacleStats.obstacleHeight, 0);
				Vector3 endPosition = new Vector3(randomX, randomY, 0);

				GameObject cone = Instantiate(ResourceLoader.instance.obstacleConePrefab, startPosition, Quaternion.identity);
				cone.GetComponent<ObstacleController>().endPosition = endPosition;
			
				obstacleStats.spawnTimer = obstacleStats.spawnInterval;
			}
			else
			{
				obstacleStats.spawnTimer -= Time.deltaTime;
			}
		}
	}
}