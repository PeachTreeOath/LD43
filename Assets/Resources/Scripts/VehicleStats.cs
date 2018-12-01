using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleStats : MonoBehaviour
{
    [Tooltip("Type of vehicle - you should only have one stat component per vehicle type!.")]
	public VehicleTypeEnum vehicleType;

    [Tooltip("How much it knocks other vehicles around & how much it gets knocked around.")]
    public float weight;

    [Tooltip("How quickly and easily it handles - i.e. the rotate speed.")]
    public float control;

	[Tooltip("How quickly it moves forward.")]
    public float speed;

    [Tooltip("How much time given at a check point.")]
    public int prayerValue;

	[Tooltip("How likely they are to fall asleep - 1 (low) to 3 (high).")]
    public float sleepChance;

    [Tooltip("How violently the vehicle will swerve.")]
    public float sleepSeverity;

    public float awakeGracePeriod;
}
