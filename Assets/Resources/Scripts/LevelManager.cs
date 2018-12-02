using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : Singleton<LevelManager>
{
    public float distanceBetweenCheckpoints;
    public float scrollSpeed;
    public float spawnRateInSeconds;
    public float rateOfPrayerDecay;
    public float headOnCrashThreshold;

    public float CrashingLinearDrag = 1;
    public float CrashingAngularDrag = 0.1f;
    public float CrashingFriction = 200;

    public float SpeedToWeightCrashingRatio = 2f;
    public float SwerveDecayPerWeight = 0.02f;
    public float WeightForZeroSwerve = 15;
    public float MinSwerve = 10f;
    public float MaxSwerve = 30f;
}