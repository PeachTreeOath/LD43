using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Controls obstacles like cones on the road.
public class ObstacleController : MonoBehaviour
{
	[Tooltip("How fast the obstacle should drop. E.g. if bird fell from the sky it could drop very fast.")]
	public float dropSpeed;

    [Tooltip("How fast the obstacle should walk across the road.")]
    public float horizontalSpeed;

    [Tooltip("How long to wait before dropping.")]
	public float dropTimer;

	[Tooltip("'Final position - logic being if you want the obstacle to stop for a moment somewhere or move faster/slower getting to said position.")]
	public Vector3 endPosition;

	public ObstacleStateEnum obstacleState;

	public ObstacleStats obstacleStats;


    [Tooltip("Warning indicator object, show display while obstacle is waiting to move into the map.")]
	public GameObject telegraph;

	public void SetEndLocation(Vector2 endPosition)
	{
		this.endPosition = endPosition;
	}

	public void Start () 
	{
		obstacleState = ObstacleStateEnum.WAITING;

		// Spawn telegraph (visual & audio indicator) based off of obstacle position.
		ObstacleTelegraph obstacleTelegraph = obstacleStats.spawnTelegraph.GetComponent<ObstacleTelegraph>();
		Vector3 obstacleSize = obstacleStats.spawnTelegraph.GetComponent<SpriteRenderer>().sprite.bounds.size;
		Vector3 obstaclePosition = transform.position;
		Vector3 telegraphPosition = Vector3.zero;
		switch (obstacleTelegraph.directionObstacleComingFrom)
		{
			case DirectionEnum.N:
				telegraphPosition = new Vector3(obstaclePosition.x, GameManager.instance.upperLeftBound.y - (obstacleSize.y / 2) - GameManager.instance.defaultDeltaFromTopForWarnings, 0);
				break;
			case DirectionEnum.E:
				telegraphPosition = new Vector3(GameManager.instance.bottomRightBound.x + (obstacleSize.x / 2) + GameManager.instance.defaultDeltaFromRightForWarnings, obstaclePosition.y, 0);
				break;
			default:
				Debug.LogWarning("Unhandled obstacle dirction - update me!");
				break;
		}
		telegraph = Instantiate(obstacleStats.spawnTelegraph, telegraphPosition, Quaternion.identity);
		telegraph.transform.SetParent(transform);

		if (endPosition.Equals(Vector2.zero))
		{
			Debug.LogWarning("Set end position for obstacle before start!!");
		}
	}

    public void Update() {
        if (!GameManager.instance.isPaused()) {
            UpdatePos();
        }
    }

	public void UpdatePos ()
	{
		// Update state.
		if (dropTimer <= 0)
		{
			if (transform.position.y <= endPosition.y)
			{
				obstacleState = ObstacleStateEnum.PLACED;
			}
			else
			{
				obstacleState = ObstacleStateEnum.DROPPING;
			}
		}

		// Do behavior based on state.
		switch (obstacleState)
		{
			case ObstacleStateEnum.WAITING:
				dropTimer -= Time.deltaTime;
				break;
			case ObstacleStateEnum.DROPPING:
				// Delete telegraph.
				if (telegraph != null)
				{
					Destroy(telegraph);
				}
				
				transform.position = new Vector3(transform.position.x + (horizontalSpeed * Time.deltaTime), transform.position.y - (dropSpeed * Time.deltaTime), transform.position.z);
				break;
			case ObstacleStateEnum.PLACED:
				// Continue moving - potentially in a different way (or add logic to pause depending on obstacle?)
				transform.position = new Vector3(transform.position.x, transform.position.y - (GameManager.instance.roadSpeed * Time.deltaTime), transform.position.z);
				if (transform.position.y <= GameManager.instance.bottomRightBound.y - GetComponent<SpriteRenderer>().sprite.bounds.size.y)
				{
					Destroy(gameObject);
				}
                if (transform.position.x <= GameManager.instance.upperLeftBound.x || transform.position.x >= GameManager.instance.bottomRightBound.x)
                {
                    Destroy(gameObject);
                }
				break;
		}
	}
}