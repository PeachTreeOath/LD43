using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Spawns obstacles at a random location in set intervals.
public class ObstacleSpawner : MonoBehaviour 
{
	[Tooltip("How frequently cones should spawn.")]
	public float coneSpawnInterval;

    [HideInInspector]
	public float coneSpawnTimer;
	
    [HideInInspector]
	public float coneHeight;

	public void Start ()
	{
		coneSpawnTimer = coneSpawnInterval;

		// Get obstacle heights so we can spawn them above bounds.
		coneHeight = ResourceLoader.instance.obstacleConePrefab.GetComponent<SpriteRenderer>().sprite.bounds.size.y;
	}
	
	public void Update ()
	{
		if (coneSpawnTimer <= 0)
		{
			// Cone spawning logic.
			float randomX = UnityEngine.Random.Range(GameManager.instance.upperLeftBound.x, GameManager.instance.bottomRightBound.x);
			float randomY = UnityEngine.Random.Range(GameManager.instance.upperLeftBound.y, GameManager.instance.bottomRightBound.y);
			Vector3 startPosition = new Vector3(randomX, GameManager.instance.upperLeftBound.y + coneHeight, 0);
			Vector3 endPosition = new Vector3(randomX, randomY, 0);

			GameObject cone = Instantiate(ResourceLoader.instance.obstacleConePrefab, startPosition, Quaternion.identity);
			cone.GetComponent<ObstacleController>().endPosition = endPosition;
			
			coneSpawnTimer = coneSpawnInterval;
		}
		else
		{
			coneSpawnTimer -= Time.deltaTime;
		}
	}
}