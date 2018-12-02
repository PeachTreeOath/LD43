using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Debug_Controls : MonoBehaviour {

    [SerializeField]
    private KeyCode jumpToNextCheckpointKey;

    [SerializeField]
    private KeyCode restartSceneKey;

	// Use this for initialization
	void Start () {
        //remove this script in prod
        Debug.LogWarning("Starting with debug controls enabled");
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(jumpToNextCheckpointKey)) {
            jumpToNextCheckpoint();
        }
        if (Input.GetKeyDown(restartSceneKey)) {
            restartScene();
        }
	}

    private void jumpToNextCheckpoint() {
        GameManager.instance.dbg_setDistToNextCheckpoint(5);
    }

    private void restartScene() {
        GameOver go = FindObjectOfType<GameOver>();
        if (go != null) {
            go.RestartGame();
        } else {
            Debug.LogError("Couldn't find the GameOver script to run");
        }
    }
}
