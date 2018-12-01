using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : Singleton<LevelManager>
{
    public float secondsBetweenCheckpoints;
    public float scrollSpeed;
    public float spawnRateInSeconds;
    public float rateOfPrayerDecay;
}