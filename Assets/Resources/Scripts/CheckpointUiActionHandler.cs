using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointUiActionHandler : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void handleResume() {
        Debug.Log("handleResume clicked");
        GameManager.instance.resumeCheckpoint();
    }
}
