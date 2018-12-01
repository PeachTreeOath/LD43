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

	void Start ()
	{
		coneSpawnTimer = coneSpawnInterval;
	}
	
	void Update ()
	{
		if (coneSpawnTimer <= 0)
		{
			// Cone spawning logic.
			// TODO: Logic to get bounds from some manager.
			float randomX = UnityEngine.Random.Range(-8, 8);
			float randomY = UnityEngine.Random.Range(-5, 5);

			Vector3 startPosition = new Vector3(randomX, 8, 0);
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