using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MedianSpawner : MonoBehaviour {

    float[] lanes = { -6.75f, -4.86f, -2.99f, -1.02f, 0.87f, 2.86f, 4.73f };

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
        public List<Entry> numberOfLanesDistribution;
        public bool allowNeighboringMedians = false;

        public float minMedianInterval;
        public float maxMedianInterval;
    }

    public List<Epoch> epochs;
    public GameObject spawnTelegraph;

    public bool isSpawningMedians = false;
    public bool allowNeighboringMedians = false;
    public float minMedianInterval;
    public float maxMedianInterval;
    public float moveWaitTime;

    public float elapsedTime = 0f;
    public float timeLeftInEpoch = 0f;

    public float timeUntilNextSpawn;

    private RandomUrn<int> laneCountDistribution = new RandomUrn<int>();
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
        float screensTall = Mathf.Max(medianLengthDistribution.Draw(), 0.5f);

        //mark what lanes will have medians then walk the array and spawn then in chunks
        var mediansAt = GetMedianLocations();
        for(var i = 0; i < mediansAt.Length; i++) {
            if(mediansAt[i]) {
                int j = i;
                while (j < mediansAt.Length && mediansAt[j]) j++;

                int lanesWide = j - i;
                SpawnMedian(i, lanesWide, screensTall);

                i = j;
            }
        }
    }

    protected bool[] GetMedianLocations() {
        int lanesLeft = lanes.Length;
        bool[] lanesUsed = new bool[lanes.Length];
        bool[] mediansAt = new bool[lanes.Length];

        int lanesToSpawn = Mathf.Max(laneCountDistribution.Draw(), 1);
        for (var i = 0; i < lanesToSpawn && lanesLeft > 0; i++) {
            if (lanesLeft == 0) break;

            int lane = Random.Range(0, lanes.Length);
            int lanesWide = Mathf.Max(laneSizeDistribution.Draw(), 1);

            if ((lane + lanesWide) > lanes.Length) {
                lane = lanes.Length - lanesWide - 1;
            }

            //find the next open lane
            while (lanesUsed[lane]) {
                lane += 1;
                if (lane >= lanes.Length) lane = 0;
            }

            // mark used lanes or cut down width to fit
            var j = 0;
            for (j = 0; j < lanesWide; j++) {
                if (!lanesUsed[lane + j]) {
                    lanesUsed[lane + j] = mediansAt[lane + j] = true;
                    lanesLeft--;
                } else {
                    lanesWide = j;
                    break;
                }
            }

            // if we're not allowing neighboring, mark them as used
            if (!allowNeighboringMedians) {
                if (lane - 1 >= 0 && !lanesUsed[lane - 1]) {
                    lanesUsed[lane - 1] = true;
                    lanesLeft--;
                }

                if (lane + lanesWide < lanes.Length && !lanesUsed[lane + lanesWide]) {
                    lanesUsed[lane + lanesWide] = true;
                    lanesLeft--;
                }
            }
        }

        return mediansAt;
    }

    protected void SpawnMedian(int lane, int lanesWide, float screensTall) { 
        var prefab = ResourceLoader.instance.obstacleMedianPrefab;

        var position = new Vector3(
            lanes[lane] + ((Median.WIDTH_PER_LANE * lanesWide) / 2f),
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

        laneCountDistribution.Clear();
        foreach(Entry entry in epoch.numberOfLanesDistribution) {
            laneCountDistribution.Add(entry.value, entry.weight);
        }

        allowNeighboringMedians = epoch.allowNeighboringMedians;
        minMedianInterval = epoch.minMedianInterval;
        maxMedianInterval = epoch.maxMedianInterval;
    }

}
