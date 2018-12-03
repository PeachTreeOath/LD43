using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Controls obstacles like cones on the road.
public class ObstacleController : MonoBehaviour
{
    public const float northWarningDeltaFromTop = 3f;
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

    private List<string> vehicleSpriteList;

    private string[] vehicleSpriteArr = { "BusSprite", "CarSprite", "LimoSprite", "MotorcycleSprite", "SemiTruckSprite", "SportsCarSprite", "TruckSprite" };

    private SpriteRenderer spr;

    private Rigidbody2D rbody;

    private float rotation;

    [Tooltip("Warning indicator object, show display while obstacle is waiting to move into the map.")]
	public GameObject telegraph;

    private bool penalized = false;

	public void SetEndLocation(Vector2 endPosition)
	{
		this.endPosition = endPosition;
	}

	public void Start () 
	{
        if (transform.position.x < 0) {
            rotation = UnityEngine.Random.Range(0, -90);
        } else {
            rotation = UnityEngine.Random.Range(-90, -180);
        }
        rbody = GetComponent<Rigidbody2D>();
        spr = GetComponent<SpriteRenderer>();
        spr.color = new Color(1f, 1f, 1f, 1f);
        vehicleSpriteList = new List<string>(vehicleSpriteArr);
        obstacleState = ObstacleStateEnum.WAITING;
		moveTimer = obstacleStats.moveWaitTime;

		// Spawn telegraph (visual & audio indicator) based off of obstacle position.
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
        if (obstacleStats.obstacleType == ObstacleTypeEnum.PEDESTRIAN && obstacleState != ObstacleStateEnum.DEAD) {
            var index = (Time.frameCount / 10) % 2;
            switch (index) {
                case 0:
                    spr.flipY = true;
                    break;
                case 1:
                    spr.flipY = false;
                    break;
                default:
                    break;
            }
        }

        if (!GameManager.instance.isPaused()) {
            UpdatePos();
        }
    }

	public void UpdatePos ()
	{
		// Update state.
		if (moveTimer <= 0)
		{
			if (obstacleState == ObstacleStateEnum.WAITING)
            {
				obstacleState = ObstacleStateEnum.MOVING;
			}
		}

		// Do behavior based on state.
		switch (obstacleState)
		{
			case ObstacleStateEnum.WAITING:
                rbody.velocity = new Vector2(0f, 0f);
                moveTimer -= Time.deltaTime;
				break;
			case ObstacleStateEnum.MOVING:
				// Delete telegraph.
				if (telegraph != null)
				{
					Destroy(telegraph);
				}
                rbody.velocity = (Vector2)(Quaternion.Euler(0, 0, rotation) * Vector2.right);
                rbody.rotation = rotation;
                //transform.right = endPosition.normalized - transform.position;
                transform.position = new Vector3(transform.position.x, transform.position.y - ((obstacleStats.verticalSpeed + speedModifier) * Time.deltaTime), transform.position.z);

                if (transform.position.y <= GameManager.instance.bottomRightBound.y - GetComponent<SpriteRenderer>().sprite.bounds.size.y) {
                    Destroy(gameObject);
                }
                if (transform.position.x <= GameManager.instance.upperLeftBound.x || transform.position.x >= GameManager.instance.bottomRightBound.x) {
                    Destroy(gameObject);
                }
                break;

			case ObstacleStateEnum.PLACED:
                // Continue moving - potentially in a different way (or add logic to pause depending on obstacle?)
                rbody.velocity = new Vector2(0f, 0f);
                transform.position = new Vector3(transform.position.x, transform.position.y - ((LevelManager.instance.scrollSpeed + speedModifier) * Time.deltaTime), transform.position.z);
				break;
            case ObstacleStateEnum.DEAD:
                spr.color = new Color(1f, 0f, 0f, 1f);
                rbody.velocity = new Vector2(0f, 0f);
                transform.position = new Vector3(transform.position.x, transform.position.y - ((LevelManager.instance.scrollSpeed + speedModifier) * Time.deltaTime), transform.position.z);
                doDeathPenalty();
                break;
        }
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        OnCollisionWithVehicle(collision);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        OnCollisionWithVehicle(collision);
    }

    private void OnCollisionWithVehicle(Collider2D collision) 
    {
        if (obstacleState == ObstacleStateEnum.MOVING)
        {
            if (vehicleSpriteList.Contains(collision.name))
            {
                obstacleState = ObstacleStateEnum.DEAD;
            }
        }
    }

    private void doDeathPenalty() {
        if (!penalized) {
            penalized = true;
            switch (tag) {
                case "ObPed":
                    GameManager.instance.GetPrayerMeter().RemovePrayer(LevelManager.instance.prayerPenaltyPerPed);
                    AudioManager.instance.PlaySound("ped_death");
                    break;

                case "ObCycle":
                    GameManager.instance.GetPrayerMeter().RemovePrayer(LevelManager.instance.prayerPenaltyPerCycle);
                    break;

                case "ObCone":
                    break;
            }
        }

    }

}