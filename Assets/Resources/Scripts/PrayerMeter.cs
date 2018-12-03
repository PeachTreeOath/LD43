using System.Collections;
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
    public GameObject lowPrayersText;
    public GameObject emptyPrayersText;

    public int periodicPrayerFactor;

    //Prayer constants set in level manager

    //Prayer Meter Serialized Fields
    //[SerializeField] private int DecayValue = 10;
    //[SerializeField] private int MaximumPrayers = 1000;
    [SerializeField] private float PrayerLifeTime = .75F;
    [SerializeField] private GameObject ProgressBar;
    [SerializeField]
    private GameObject SurgeMeter; //shows incoming prayers
    [SerializeField] private GameObject PrayerPrefab;
    [SerializeField]
    private float SurgeTimeMs = 400f;

    //Prayer Meter Private Variables
    public float maxPrayers;
    private float decayPerTick;
    private float decayTick;
    private float incomePerTick;
    private float incomeTick;
    private float _prayerMeterDecayTimer;
    private float _prayerMeterIncomeTimer;
    private float _maxPrayerMeterProgressSize = 0; //constant, based on orig size of bar
    private RectTransform _prayerMeterProgressRect;
    private RectTransform surgeMeterProgressRect;
    private float _prayerCount = 0;
    private Vector2 _prayerMeterLocation = Vector2.zero;

    private float remainingSurgeTime; //should max at surgeTimeMs

    private LevelManager levelManager;
    bool pulsing;
    Vector3 startScale;
    float pulseTimer;
    bool pulseUp;

    public GameObject GetProgressBar()
    {
        return ProgressBar;
    }

    void Awake()
    {
        levelManager = FindObjectOfType<LevelManager>();
        if (levelManager == null)
        {
            Debug.LogError("Couldn't find object with level manager");
        }
    }

    // Use this for initialization
    void Start()
    {
        startScale = prayerMeter.localScale;

        //Initialize Prayer Meter Count
        maxPrayers = levelManager.maxPrayers;
        //_prayerCount = levelManager.maxPrayers;
        _prayerCount = levelManager.startingAmtOfPrayers;

        decayTick = levelManager.prayerDecayTickRate;
        decayPerTick = levelManager.prayerDecayPerSec * decayTick;

        incomeTick = levelManager.prayerIncomeTickRate;
        incomePerTick = levelManager.prayersPerSecondPerCar * incomeTick;

        //Initialize Prayer Meter Progress Size Based on Ui
        _prayerMeterProgressRect = ProgressBar.GetComponent<RectTransform>();
        //Surge meter follows progress meter size until prayers are added
        surgeMeterProgressRect = SurgeMeter.GetComponent<RectTransform>();
        _maxPrayerMeterProgressSize = _prayerMeterProgressRect.sizeDelta.x;

        //Initialize Prayer Meter Location
        _prayerMeterLocation = new Vector2(ProgressBar.transform.position.x, ProgressBar.transform.position.y);

        //Initialize and start the Prayer Meter Decay Timer
        _prayerMeterDecayTimer = Time.time;

        lowPrayersText.GetComponent<Renderer>().enabled = false;
        foreach (Renderer rend in emptyPrayersText.GetComponentsInChildren<Renderer>())
        {
            rend.enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Decrement the prayer meter based on decay timer.
        float decayDiff = Time.time - _prayerMeterDecayTimer;
        if (decayDiff >= decayTick)
        {
            //Decrement the prayer meter.
            RemovePrayer(decayPerTick * (decayDiff / decayTick)); //scale with how far expired the tick was
        }

        float incomeDiff = Time.time - _prayerMeterIncomeTimer;
        if (incomeDiff >= incomeTick)
        {
            int numCars = GameManager.instance.getVehiclePool().getNumWorkingCars();
            int amt = (int)(numCars * incomePerTick * (incomeDiff / incomeTick));
            //Round amt up to nearest periodicPrayerFactor
            while (amt % periodicPrayerFactor != 0)
            {
                amt++;
            }
            Debug.Log("Adding timed prayer income for numCars:" + numCars + ", amt=" + amt);
            AddTimedPrayer(amt);
        }

        UpdateAnimations();
        remainingSurgeTime -= Time.deltaTime;
        remainingSurgeTime = Mathf.Max(remainingSurgeTime, 0);

        if (pulsing)
        {
            PulseMeter();
        }
        if (_prayerCount == 0)
        {
            lowPrayersText.GetComponent<Renderer>().enabled = false;
            foreach (Renderer rend in emptyPrayersText.GetComponentsInChildren<Renderer>())
            {
                rend.enabled = true;
            }
            pulsing = true;
            pulseTimer = Time.time;
            pulseUp = true;
        }
        else if (!pulsing &&
                _prayerCount / maxPrayers <= lowThreshold)
        {
            lowPrayersText.GetComponent<Renderer>().enabled = true;
            foreach (Renderer rend in emptyPrayersText.GetComponentsInChildren<Renderer>())
            {
                rend.enabled = false;
            }
            pulsing = true;
            pulseTimer = Time.time;
            pulseUp = true;
        }
        else if (_prayerCount / maxPrayers > lowThreshold)
        {
            if (pulsing)
            {
                lowPrayersText.GetComponent<Renderer>().enabled = false;
                foreach (Renderer rend in emptyPrayersText.GetComponentsInChildren<Renderer>())
                {
                    rend.enabled = false;
                }
            }
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

    private void UpdateAnimations()
    {
        //update assuming current prayer calculations are correct
        float target = surgeMeterProgressRect.sizeDelta.x;
        float current = _prayerMeterProgressRect.sizeDelta.x;
        Vector2 newDisplaySize;
        if (target > current)
        {
            //lerp
            float t = (SurgeTimeMs - remainingSurgeTime) / SurgeTimeMs; //0 on start, 1 on finish
            float newX = Mathf.Lerp(current, target, t);
            newDisplaySize = new Vector2(newX, _prayerMeterProgressRect.sizeDelta.y);
        }
        else
        {
            //match surge on the way down
            newDisplaySize = new Vector2(target, _prayerMeterProgressRect.sizeDelta.y);
        }
        _prayerMeterProgressRect.sizeDelta = newDisplaySize;
    }

    public void RemovePrayer(float deltaAmt)
    {
        _prayerCount -= deltaAmt;
        //Set a bottom threshold for prayer meter count. Game over on running out of prayer power.
        if (_prayerCount <= 0)
        {
            _prayerCount = 0;
            GameManager.instance.GameOverPrayerPowerDeath();
        }

        //Reset the decay timer.
        _prayerMeterDecayTimer = Time.time;

        TriggerUiUpdate();
    }

    public void AddTimedPrayer(int prayerValue) {
        if(GameManager.instance.isPrayerDeath())
            return;

        prayerValue /= periodicPrayerFactor;

        VehiclePool vp = GameManager.instance.getVehiclePool();
        int prayerCount = vp.vehicles.Count == 0 ? 0 : prayerValue / vp.vehicles.Count;
        int prayerExtra = prayerValue % vp.vehicles.Count;

        for (int i = 0; i < vp.vehicles.Count - 1; i++)
        {
            for (int p = 0; p < prayerCount; p++)
            {
                GameObject ph = Instantiate(ResourceLoader.instance.prayerHandsFab) as GameObject;
                ph.GetComponent<PrayerHands>().value = periodicPrayerFactor;
                ph.transform.position = vp.vehicles[i].gameObject.transform.position + new Vector3(Random.Range(0f, 1f),
                                                                                                    Random.Range(0f, 1f),
                                                                                                    -1);
                ph.transform.SetParent(vp.vehicles[i].gameObject.transform);
            }
        }

        for (int p = 0; p < prayerCount + prayerExtra; p++)
        {
            GameObject ph = Instantiate(ResourceLoader.instance.prayerHandsFab) as GameObject;
            ph.GetComponent<PrayerHands>().value = periodicPrayerFactor;
            ph.transform.position = vp.vehicles[vp.vehicles.Count - 1].gameObject.transform.position + new Vector3(Random.Range(0f, 1f),
                                                                                                Random.Range(0f, 1f),
                                                                                                -1);
            ph.transform.SetParent(vp.vehicles[vp.vehicles.Count - 1].gameObject.transform);
        }

        _prayerMeterIncomeTimer = Time.time;
    }

    public void AddPrayer(float prayerValue)
    {

        //Increment the prayer count.
        _prayerCount += prayerValue;
        if (_prayerCount > maxPrayers)
        {
            _prayerCount = maxPrayers;
        }

        float surgeBarSize = getRealBarSize();
        float dispBarSize = getDisplayBarSize();
        float surgeSize = surgeBarSize - dispBarSize;

        if (remainingSurgeTime < 1)
        { //don't make it take longer if the bar is already increasing
            remainingSurgeTime = SurgeTimeMs;
        }

        TriggerUiUpdate();
    }

    public float GetPrayer() {
        return _prayerCount;
    }

    /// <summary>
    /// Get the actual size of the bar as it is right now.  Does not take into 
    /// account animation delay.  This is the real amount of health the player currently has.
    /// </summary>
    /// <returns></returns>
    private float getRealBarSize()
    {
        float progressUpdate = (_prayerCount / maxPrayers) * _maxPrayerMeterProgressSize;
        return progressUpdate;
    }

    /// <summary>
    /// Get the size of the display bar as it is currently being rendered.  This
    /// may or may not be the actual amount of health the player has due to animation delay.
    /// </summary>
    /// <returns></returns>
    private float getDisplayBarSize()
    {
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
