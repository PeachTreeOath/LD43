﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Median : MonoBehaviour {

    public enum State { WAITING_TO_DROP, DROPPING, SCROLLING_BACK_FOR_CARNAGE }

    public const float WIDTH_PER_LANE = 2f;
    public const float HEIGHT_PER_SCREEN = 10f;

    public const float LEFT_EDGE_OF_FIRST_LANE = -4.78f;

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
        var medianHeight = renderer.bounds.size.y;
        if (gameObject.transform.position.y < GameManager.instance.bottomRightBound.y - medianHeight * 2) {
            Destroy(gameObject);
        }
    }
}
