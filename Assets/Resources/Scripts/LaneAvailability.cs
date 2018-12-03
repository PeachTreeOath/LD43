using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaneAvailability : MonoBehaviour {

    private float[] laneTimers = new float[7];

    public bool IsLaneOpen(int laneNumber) {
        return laneTimers[laneNumber] <= 0; 
    }

    public void CloseLane(int laneNumber, float duration) {
        laneTimers[laneNumber] = duration;
    }
	
	void Update () {
        if (GameManager.instance.isPaused()) return;

        for(var i = 0; i < laneTimers.Length; i++) {
            laneTimers[i] -= Time.deltaTime;
        }
	}
}
