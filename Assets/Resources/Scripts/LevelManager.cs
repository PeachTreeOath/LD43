using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : Singleton<LevelManager>
{
    public float secondsBetweenCheckpoints;
    public float spawnRateInSeconds;
    public float rateOfPrayerDecay;
}