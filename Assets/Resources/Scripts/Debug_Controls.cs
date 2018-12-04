using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Debug_Controls : MonoBehaviour {
#if UNITY_EDITOR
    [SerializeField]
    private KeyCode jumpToNextCheckpointKey;

    [SerializeField]
    private KeyCode restartSceneKey;

    [SerializeField]
    private KeyCode removePrayers;

    [SerializeField]
    private KeyCode addPrayers;

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
        if (Input.GetKeyDown(removePrayers)) {
            deltaPrayers(-150);
        }
        if (Input.GetKeyDown(addPrayers)) {
            deltaPrayers(100);
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

    private void deltaPrayers(float amt) {
        PrayerMeter pm = GameManager.instance.GetPrayerMeter();
        if (pm != null) {
            if (amt > 0) {
                Debug.Log("Adding " + amt + " prayers (debug)");
                pm.AddPrayer(amt);
            } else {
                Debug.Log("Removing " + amt + " prayers (debug)");
                pm.RemovePrayer(-amt);
            }
        } else {
            Debug.LogError("No prayer meter to change");
        }

    }
#endif
}
