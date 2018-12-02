using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Median : MonoBehaviour {

    public enum State { WAITING_TO_DROP, DROPPING, SCROLLING_BACK_FOR_CARNAGE }

    public const float WIDTH_PER_LANE = 2f;
    public const float HEIGHT_PER_SCREEN = 10f;

    public const float LEFT_EDGE_OF_FIRST_LANE = -2.82f;

    private const float DISTANCE_BETWEEN_TELEGRAPHS = 0.75f;
    private const float Y_POSITION_OF_TELEGRAPHS = 5f - 0.33f;

    private new BoxCollider2D collider;
    private new SpriteRenderer renderer;
    private Rigidbody2D rbody;
    private bool initialized = false;

    public State currState = State.WAITING_TO_DROP;
    public float moveTimer;

    void Start() {
        Initialize();
    }

    public void Initialize() {
        if (initialized) return;

        collider = GetComponent<BoxCollider2D>();
        renderer = GetComponent<SpriteRenderer>();
        rbody = GetComponent<Rigidbody2D>();

        initialized = true;
	}

    public void SetSize(int numLanes, float numberOfScreens) {
        var size = new Vector2(numLanes * WIDTH_PER_LANE, numberOfScreens * HEIGHT_PER_SCREEN);
        collider.size = size;
        renderer.size = size;
    }

    public void SpawnTelegraphsAlongWidth(GameObject telegraphPrefab) {
        var stride = DISTANCE_BETWEEN_TELEGRAPHS;

        var numToSpawn = Mathf.FloorToInt(collider.size.x / stride);
        var totalSpaceUsedToSpawn = numToSpawn * stride;
        var leftPadding = (collider.size.x - totalSpaceUsedToSpawn) / 2f;

        var position = new Vector3(
            collider.bounds.min.x + leftPadding,
            Y_POSITION_OF_TELEGRAPHS,
            transform.position.z
        );

        for (var i = 0; i < numToSpawn; i++) {
            Instantiate(telegraphPrefab, position, Quaternion.identity, transform);
            position.x += stride;
        }
    }

    void DestroyAllTelegraphs() {
        var telegraphs = GetComponentsInChildren<ObstacleTelegraph>();
        foreach(var telegraph in telegraphs) {
            Destroy(telegraph.gameObject);
        }
    }

    void Drop() {
        currState = State.DROPPING;

        DestroyAllTelegraphs();
        rbody.velocity = new Vector2(0, -LevelManager.instance.scrollSpeed);
    }

	// Update is called once per frame
	void Update () {
        if (GameManager.instance.isPaused()) return;	

        switch(currState) {
            case State.WAITING_TO_DROP:
                moveTimer -= Time.deltaTime;
                if(moveTimer <= 0f) {
                    Drop();
                }
                break;
        }
	}
}
