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

	[Tooltip("'Final position. Afterwards its drop speed matches the road speed.")]
	public Vector3 endPosition;

    [HideInInspector]

	public ObstacleStateEnum obstacleState;

	public void SetEndLocation(Vector2 endPosition)
	{
		this.endPosition = endPosition;
	}

	public void Start () 
	{
		obstacleState = ObstacleStateEnum.WAITING;

		if (endPosition.Equals(Vector2.zero))
		{
			Debug.LogWarning("Set end position for obstacle before start!!");
		}
	}
	
	public void Update ()
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
				transform.position = new Vector3(transform.position.x + (horizontalSpeed * Time.deltaTime), transform.position.y - (dropSpeed * Time.deltaTime), transform.position.z);
				break;
			case ObstacleStateEnum.PLACED:
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