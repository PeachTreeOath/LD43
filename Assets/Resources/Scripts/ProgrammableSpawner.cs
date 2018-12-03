using UnityEngine;
using System.Collections.Generic;

public abstract class ProgrammableSpawner : MonoBehaviour {
    protected float[] lanes = { -6.75f, -4.86f, -2.99f, -1.02f, 0.87f, 2.86f, 4.73f };

    public SpawningBehaviors spawningBehaviors;
    public GameObject spawnTelegraph;

    public bool isSpawningMedians = false;
    public bool allowNeighboringMedians = false;
    public float minMedianInterval;
    public float maxMedianInterval;
    public float moveWaitTime;

    public float elapsedTime = 0f;
    public float timeLeftInEpoch = 0f;

    public float timeUntilNextSpawn;

    protected RandomUrn<int> laneCountDistribution = new RandomUrn<int>();
    protected RandomUrn<int> laneSizeDistribution = new RandomUrn<int>();
    protected RandomUrn<float> medianLengthDistribution = new RandomUrn<float>();
    
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

    protected abstract void Spawn();

    protected bool[] GetSpawnLocations() {
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

    void UpdateEpoch() {
        timeLeftInEpoch -= Time.deltaTime;
        if (timeLeftInEpoch > 0) return;

        RandomUrn<SpawningBehavior> ruleUrn = new RandomUrn<SpawningBehavior>();

        //TODO use LINQ like a real programmer
        foreach(var rule in spawningBehaviors.rules) {
            if(rule.startUsingAfter <= elapsedTime && (rule.neverStopUsing || rule.stopUsingAfter > elapsedTime)) {
                ruleUrn.Add(rule, rule.weight);
            }
        }

        var chosenRule = ruleUrn.Draw();
        if(chosenRule != null) {
            SetupRule(chosenRule);
        }
    }

    void SetupRule(SpawningBehavior rule) {
        timeLeftInEpoch = rule.lengthOfBehavior;

        isSpawningMedians = rule.spawnAnything;
        if (!isSpawningMedians) return;

        laneSizeDistribution.Clear();
        foreach(var entry in rule.laneSizeDistribution) {
            laneSizeDistribution.Add(entry.value, entry.weight);
        }

        medianLengthDistribution.Clear();
        foreach(var entry in rule.lengthDistribution) {
            medianLengthDistribution.Add(entry.value, entry.weight);
        }

        laneCountDistribution.Clear();
        foreach(var entry in rule.numberOfLanesDistribution) {
            laneCountDistribution.Add(entry.value, entry.weight);
        }

        allowNeighboringMedians = rule.allowNeighboringSpawn;
        minMedianInterval = rule.minSpawnInterval;
        maxMedianInterval = rule.maxSpawnInterval;
    }

}