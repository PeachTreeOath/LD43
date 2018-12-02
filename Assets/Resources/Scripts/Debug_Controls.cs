using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Debug_Controls : MonoBehaviour {

    [SerializeField]
    private KeyCode jumpToNextCheckpointKey;

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
	}

    private void jumpToNextCheckpoint() {
        GameManager.instance.dbg_setDistToNextCheckpoint(20);
    }
}
