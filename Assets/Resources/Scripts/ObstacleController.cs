using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Controls obstacles like cones on the road. Assumes obstacles only move downward in the y direction.
public class ObstacleController : MonoBehaviour
{
	[Tooltip("How fast the obstacle should drop. E.g. if bird fell from the sky it could drop very fast.")]
	public float dropSpeed;

	[Tooltip("How long to wait before dropping.")]
	public float dropTimer;

    [HideInInspector]
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
			Debug.Log("Set end position for obstacle before start!!");
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
				transform.position = new Vector3(transform.position.x, transform.position.y - (dropSpeed * Time.deltaTime), transform.position.z);
				break;
			case ObstacleStateEnum.PLACED:
				transform.position = new Vector3(transform.position.x, transform.position.y - (1 * Time.deltaTime), transform.position.z); // TOOD: get distance increment from some manager normal road increase.
				// TODO: check if it is at a boundary from a manager and then delete.
				break;
		}
	}
}