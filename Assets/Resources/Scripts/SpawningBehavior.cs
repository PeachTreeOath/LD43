using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class SpawningBehavior {

    [System.Serializable]
    public class Entry {
        public int value;
        public float weight;
    }

    public float startUsingAfter;
    public float stopUsingAfter;
    public float lengthOfBehavior;
    public float weight;

    public bool neverStopUsing = false;

    public bool spawnAnything = true;
    public List<Entry> laneSizeDistribution;
    public List<Entry> lengthDistribution;
    public List<Entry> numberOfLanesDistribution;
    public bool allowNeighboringSpawn = false;

    public float minSpawnInterval;
    public float maxSpawnInterval;
}