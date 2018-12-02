using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Controls obstacles like cones on the road.
public class ObstacleController : MonoBehaviour
{
	public const float northWarningDeltaFromTop = 3.5f;
	public const float eastWarningDeltaFromRight = 0.3f;

	[Tooltip("'Final position - logic being if you want the obstacle to stop for a moment somewhere or move faster/slower getting to said position.")]
	public Vector3 endPosition;

	public float moveTimer;

    public float speedModifier;

    [HideInInspector]

	public ObstacleStateEnum obstacleState;

    [HideInInspector]
    public float scrollSpeed;

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
		moveTimer = obstacleStats.moveWaitTime;

		// Spawn telegraph (visual & audio indicator) based off of obstacle position.
		ObstacleTelegraph obstacleTelegraph = obstacleStats.spawnTelegraph.GetComponent<ObstacleTelegraph>();
		Vector3 obstacleSize = obstacleStats.spawnTelegraph.GetComponent<SpriteRenderer>().sprite.bounds.size;
		Vector3 obstaclePosition = transform.position;
		Vector3 telegraphPosition = Vector3.zero;
		switch (obstacleStats.directionObstacleComingFrom)
		{
			case DirectionEnum.N:
				telegraphPosition = new Vector3(obstaclePosition.x, northWarningDeltaFromTop, 0);
				break;
			case DirectionEnum.E:
				telegraphPosition = new Vector3(eastWarningDeltaFromRight, obstaclePosition.y, 0);
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

        //Randomize vehicle speed range.
	    speedModifier = Random.Range(-obstacleStats.verticalRange, obstacleStats.verticalRange);
	}

    public void Update() {
        if (!GameManager.instance.isPaused()) {
            UpdatePos();
        }
    }

	public void UpdatePos ()
	{
		// Update state.
		if (moveTimer <= 0)
		{
			if (obstacleState != ObstacleStateEnum.PLACED)
            {
				obstacleState = ObstacleStateEnum.MOVING;
			}
		}

		// Do behavior based on state.
		switch (obstacleState)
		{
			case ObstacleStateEnum.WAITING:
				moveTimer -= Time.deltaTime;
				break;
			case ObstacleStateEnum.MOVING:
				// Delete telegraph.
				if (telegraph != null)
				{
					Destroy(telegraph);
				}
				
				transform.position = new Vector3(transform.position.x + (obstacleStats.horizontalSpeed * Time.deltaTime), transform.position.y - ((obstacleStats.verticalSpeed + speedModifier) * Time.deltaTime), transform.position.z);

                if (transform.position.y <= GameManager.instance.bottomRightBound.y - GetComponent<SpriteRenderer>().sprite.bounds.size.y) {
                    Destroy(gameObject);
                }
                if (transform.position.x <= GameManager.instance.upperLeftBound.x || transform.position.x >= GameManager.instance.bottomRightBound.x) {
                    Destroy(gameObject);
                }
                break;

			case ObstacleStateEnum.PLACED:
				// Continue moving - potentially in a different way (or add logic to pause depending on obstacle?)
				transform.position = new Vector3(0, transform.position.y - ((LevelManager.instance.scrollSpeed + speedModifier) * Time.deltaTime), transform.position.z);

				break;
		}
	}

    private void OnTriggerEnter2D(Collision2D collision)
    {
        obstacleState = ObstacleStateEnum.PLACED;
    }

    private void OnTriggerStay2D(Collision2D collision)
    {
        obstacleState = ObstacleStateEnum.PLACED;
    }
}