﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Prayer meter logic:
/// Bar length is determined by starting rectangle.
/// Added prayers increase the size, up to max.  Removing prayers decrease the size (this happens periodically).
/// The animation is actually 2 bars: one "Surge" bar and the other "Display" bar.
/// As the bar drains the Display bar is shown.
/// When prayers are added, the "Surge" bar snaps to the new prayer amount.  This is followed by 
/// this Display bar animating to catch up.
/// </summary>
public class PrayerMeter : MonoBehaviour
{
    public float lowThreshold;
    public float pulseScale;
    public float pulseSpeed;
    public RectTransform prayerMeter;

    //Prayer Meter Serialized Fields
    [SerializeField] private float DecayTimer = .5f;
    [SerializeField] private int DecayValue = 10;
    [SerializeField] private int MaximumPrayers = 1000;
    [SerializeField] private float PrayerLifeTime = .75F;
    [SerializeField] private GameObject ProgressBar;
    [SerializeField]
    private GameObject SurgeMeter; //shows incoming prayers
    [SerializeField] private GameObject PrayerPrefab;
    [SerializeField]
    private float SurgeTimeMs = 400f;

    //Prayer Meter Private Variables
    private float _prayerMeterDecayTimer;
    private float _maxPrayerMeterProgressSize = 0; //constant, based on orig size of bar
    private RectTransform _prayerMeterProgressRect;
    private RectTransform surgeMeterProgressRect;
    private float _prayerCount = 0;
    private Vector2 _prayerMeterLocation = Vector2.zero;

    private float remainingSurgeTime; //should max at surgeTimeMs

    bool pulsing;
    Vector3 startScale;
    float pulseTimer;
    bool pulseUp;

    public GameObject GetProgressBar()
    {
        return ProgressBar;
    }

    // Use this for initialization
    void Start()
    {
        startScale = prayerMeter.localScale;

        //Initialize Prayer Meter Count
        _prayerCount = MaximumPrayers;

        //Initialize Prayer Meter Progress Size Based on Ui
        _prayerMeterProgressRect = ProgressBar.GetComponent<RectTransform>();
        //Surge meter follows progress meter size until prayers are added
        surgeMeterProgressRect = SurgeMeter.GetComponent<RectTransform>();
        _maxPrayerMeterProgressSize = _prayerMeterProgressRect.sizeDelta.x;

        //Initialize Prayer Meter Location
        _prayerMeterLocation = new Vector2(ProgressBar.transform.position.x, ProgressBar.transform.position.y);

        //Initialize and start the Prayer Meter Decay Timer
        _prayerMeterDecayTimer = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        //Decrement the prayer meter based on decay timer.
        if (Time.time - _prayerMeterDecayTimer >= DecayTimer) {
            //Decrement the prayer meter.
            RemovePrayer(DecayValue);
        }

        UpdateAnimations();
        remainingSurgeTime -= Time.deltaTime;
        remainingSurgeTime = Mathf.Max(remainingSurgeTime, 0);
        
        if(pulsing)
        {
            PulseMeter();
        }else if(!pulsing &&
                _prayerCount / MaximumPrayers <= lowThreshold)
        {
            pulsing = true;
            pulseTimer = Time.time;
            pulseUp = true;
        }else if (_prayerCount / MaximumPrayers > lowThreshold)
        {
            pulsing = false;
            prayerMeter.localScale = startScale;
            prayerMeter.ForceUpdateRectTransforms();
        }
    }

    void PulseMeter()
    {
        float dt = (Time.time - pulseTimer) / pulseSpeed;
        if (pulseUp)
        {
            Vector3 v1 = Vector3.Lerp(startScale, startScale * pulseScale, (Time.time - pulseTimer) / pulseSpeed);
            prayerMeter.localScale = v1;
        }
        else
        {
            Vector3 v1 = Vector3.Lerp(startScale * pulseScale, startScale, (Time.time - pulseTimer) / pulseSpeed);
            prayerMeter.localScale = v1;
        }

        prayerMeter.ForceUpdateRectTransforms();
        if (dt >= 1)
        {
            pulseTimer = Time.time;
            pulseUp = !pulseUp;
        }
    }

    private void UpdateAnimations() {
        //update assuming current prayer calculations are correct
        float target = surgeMeterProgressRect.sizeDelta.x;
        float current = _prayerMeterProgressRect.sizeDelta.x;
        Vector2 newDisplaySize;
        if (target > current) {
            //lerp
            float t = (SurgeTimeMs - remainingSurgeTime) / SurgeTimeMs; //0 on start, 1 on finish
            float newX = Mathf.Lerp(current, target, t);
            newDisplaySize = new Vector2(newX, _prayerMeterProgressRect.sizeDelta.y);
        } else {
            //match surge on the way down
            newDisplaySize = new Vector2(target, _prayerMeterProgressRect.sizeDelta.y);
        }
        _prayerMeterProgressRect.sizeDelta = newDisplaySize;
    }

    public void RemovePrayer(float deltaAmt) {
        _prayerCount -= deltaAmt;
        //Set a bottom threshold for prayer meter count. Game over on running out of prayer power.
        if (_prayerCount <= 0) {
            _prayerCount = 0;
            GameManager.instance.GameOverPrayerPowerDeath();
        }

        //Reset the decay timer.
        _prayerMeterDecayTimer = Time.time;

        TriggerUiUpdate();
    }

    public void AddPrayer(float prayerValue)
    {
        //Increment the prayer count.
        _prayerCount += prayerValue;
        if (_prayerCount > MaximumPrayers)
        {
            _prayerCount = MaximumPrayers;
        }

        float surgeBarSize = getRealBarSize();
        float dispBarSize = getDisplayBarSize();
        float surgeSize = surgeBarSize - dispBarSize;

        if (remainingSurgeTime < 1) { //don't make it take longer if the bar is already increasing
            remainingSurgeTime = SurgeTimeMs;
        }

        TriggerUiUpdate();
    }

    /// <summary>
    /// Get the actual size of the bar as it is right now.  Does not take into 
    /// account animation delay.  This is the real amount of health the player currently has.
    /// </summary>
    /// <returns></returns>
    private float getRealBarSize() {
        float progressUpdate = (_prayerCount / MaximumPrayers) * _maxPrayerMeterProgressSize;
        return progressUpdate;
    }

    /// <summary>
    /// Get the size of the display bar as it is currently being rendered.  This
    /// may or may not be the actual amount of health the player has due to animation delay.
    /// </summary>
    /// <returns></returns>
    private float getDisplayBarSize() {
        return _prayerMeterProgressRect.sizeDelta.x;
    }

   // private void UpdateUi()
   // {
   //     //Update the Ui.
   //     var progressUpdate = (_prayerCount / MaximumPrayers) * _maxPrayerMeterProgressSize;
   //     _prayerMeterProgressRect.sizeDelta = new Vector2(progressUpdate, _prayerMeterProgressRect.sizeDelta.y);
   // }

    private void TriggerUiUpdate()
    {
        float newSurgeSize = getRealBarSize();

        //Queue up an animation for the display bar.
        //If surgeAmount > 0 then health was added -> animate it
        //otherwise we are the same or losing health... just display that normally
        //Display bar is always Lerped toward the surge bar..but this happens in the update loop
        //_prayerMeterProgressRect.sizeDelta = new Vector2(newSize, _prayerMeterProgressRect.sizeDelta.y);
        surgeMeterProgressRect.sizeDelta = new Vector2(newSurgeSize, _prayerMeterProgressRect.sizeDelta.y);
    }
}
