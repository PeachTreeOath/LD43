using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MedianSpawner : MonoBehaviour {

    const int NUM_LANES = 5;

    [System.Serializable]
    public class Entry {
        public int value;
        public float weight;
    }

    [System.Serializable]
    public class Epoch {
        public float startUsingAfter;
        public float stopUsingAfter;
        public float lengthOfEpoch;
        public float weight;

        public bool neverStopUsing = false;

        public bool spawnMedians = true;
        public List<Entry> laneSizeDistribution;
        public List<Entry> medianLengthDistribution;

        public float minMedianInterval;
        public float maxMedianInterval;
    }

    public List<Epoch> epochs;
    public GameObject spawnTelegraph;

    public bool isSpawningMedians = false;
    public float minMedianInterval;
    public float maxMedianInterval;
    public float moveWaitTime;

    public float elapsedTime = 0f;
    public float timeLeftInEpoch = 0f;

    public float timeUntilNextSpawn;

    private RandomUrn<int> laneSizeDistribution = new RandomUrn<int>();
    private RandomUrn<float> medianLengthDistribution = new RandomUrn<float>();
    
	void Start () {
        if (epochs == null) epochs = new List<Epoch>();
	}

    // Update is called once per frame
    void Update() {
        if (GameManager.instance.isPaused()) return;

        elapsedTime += Time.deltaTime;
        UpdateEpoch();
        UpdateSpawn();
    }

    void UpdateSpawn() { 
        if (!isSpawningMedians) return;

        timeUntilNextSpawn -= Time.deltaTime;
        if(timeUntilNextSpawn <= 0) {
            Spawn();
            timeUntilNextSpawn = Random.Range(minMedianInterval, maxMedianInterval);
        }
	}

    void Spawn() {
        var prefab = ResourceLoader.instance.obstacleMedianPrefab;

        int lanesWide = laneSizeDistribution.Draw();
        float screensTall = medianLengthDistribution.Draw();

        int lane = Random.Range(0, NUM_LANES);
        if( (lane + lanesWide) > NUM_LANES) {
            lane = NUM_LANES - lanesWide - 1; 
        }

        var position = new Vector3(
            Median.LEFT_EDGE_OF_FIRST_LANE + (lane * Median.WIDTH_PER_LANE) + ((Median.WIDTH_PER_LANE * lanesWide) / 2f),
            6f + (screensTall * Median.HEIGHT_PER_SCREEN) /2f, //TODO this isn't quite right
            0f
        );

        var median = Instantiate(prefab, position, Quaternion.identity).GetComponent<Median>();
        median.Initialize();

        median.SetSize(lanesWide, screensTall);
        median.SpawnTelegraphsAlongWidth(spawnTelegraph);
        median.moveTimer = moveWaitTime;
    }

    void UpdateEpoch() {
        timeLeftInEpoch -= Time.deltaTime;
        if (timeLeftInEpoch > 0) return;

        RandomUrn<Epoch> epochUrn = new RandomUrn<Epoch>();

        //TODO use LINQ like a real programmer
        foreach(var epoch in epochs) {
            if(epoch.startUsingAfter <= elapsedTime && (epoch.neverStopUsing || epoch.stopUsingAfter > elapsedTime)) {
                epochUrn.Add(epoch, epoch.weight);
            }
        }

        var chosenEpoch = epochUrn.Draw();
        if(chosenEpoch != null) {
            SetupEpoch(chosenEpoch);
        }
    }

    void SetupEpoch(Epoch epoch) {
        timeLeftInEpoch = epoch.lengthOfEpoch;

        isSpawningMedians = epoch.spawnMedians;
        if (!isSpawningMedians) return;

        laneSizeDistribution.Clear();
        foreach(Entry entry in epoch.laneSizeDistribution) {
            laneSizeDistribution.Add(entry.value, entry.weight);
        }

        medianLengthDistribution.Clear();
        foreach(Entry entry in epoch.medianLengthDistribution) {
            medianLengthDistribution.Add(entry.value, entry.weight);
        }

        minMedianInterval = epoch.minMedianInterval;
        maxMedianInterval = epoch.maxMedianInterval;
    }

}
