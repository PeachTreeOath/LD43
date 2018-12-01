using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = System.Diagnostics.Debug;

public class PrayerMeter : MonoBehaviour {

    //Prayer Meter Serialized Fields
    [SerializeField] private long DecayTimer = 500;
    [SerializeField] private int DecayValue = 10;
    [SerializeField] private int MaximumPrayers = 1000;
    [SerializeField] private float PrayerLifeTime = .75F;
    [SerializeField] private GameObject ProgressBar;
    [SerializeField] private GameObject PrayerPrefab;

    //Prayer Meter Private Variables
    private Stopwatch _prayerMeterDecayTimer;
    private float _maxPrayerMeterProgressSize = 0;
    private RectTransform _prayerMeterProgressSize;
    private float _prayerCount = 0;
    private Vector2 _prayerMeterLocation = Vector2.zero;

    public GameObject GetProgressBar()
    {
        return ProgressBar;
    }

	// Use this for initialization
	void Start () {

        //Initialize Prayer Meter Count
	    _prayerCount = MaximumPrayers;

	    //Initialize Prayer Meter Progress Size Based on Ui
	    _prayerMeterProgressSize = ProgressBar.GetComponent<RectTransform>();
        _maxPrayerMeterProgressSize = _prayerMeterProgressSize.sizeDelta.x;

        //Initialize Prayer Meter Location
	    _prayerMeterLocation = new Vector2(ProgressBar.transform.position.x, ProgressBar.transform.position.y);

        //Initialize and start the Prayer Meter Decay Timer
        _prayerMeterDecayTimer = new Stopwatch();
        _prayerMeterDecayTimer.Start();
	}
	
	// Update is called once per frame
	void Update () {
		
        //Decrement the prayer meter based on decay timer.
	    if (_prayerMeterDecayTimer.ElapsedMilliseconds >= DecayTimer)
	    {
            //Decrement the prayer meter.
	        _prayerCount -= DecayValue;

            //Set a bottom threshold for prayer meter count.
	        if (_prayerCount <= 0) _prayerCount = 0;

	        //Update the Ui.
	        UpdateUi();

            //Reset the decay timer.
            _prayerMeterDecayTimer.Reset();
	        _prayerMeterDecayTimer.Start();
        }
	}

    public void AddPrayer(float prayerValue)
    {
        //Increment the prayer count.
        _prayerCount += prayerValue;
        if(_prayerCount > MaximumPrayers)
        {
            _prayerCount = MaximumPrayers;
        }

        //Update the Ui.
        UpdateUi();
    }

    private void UpdateUi()
    {
        //Update the Ui.
        var progressUpdate = (_prayerCount / MaximumPrayers) * _maxPrayerMeterProgressSize;
        _prayerMeterProgressSize.sizeDelta = new Vector2(progressUpdate, _prayerMeterProgressSize.sizeDelta.y);
    }
}
