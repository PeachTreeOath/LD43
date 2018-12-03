using UnityEngine;
using System.Collections.Generic;

public class RandomUrn<V>  {

    public class Entry {
        public V value;
        public float weight;

        public Entry(V value, float weight) {
            this.value = value;
            this.weight = weight;
        }
    }

    private List<Entry> entries = new List<Entry>();

    public void Add(V value, float weight) {
        entries.Add(new Entry(value, weight));
    }

    public void Clear() {
        this.entries.Clear();
    }

    public V Draw() {
        if (entries.Count == 0) return default(V);

        float total = 0;
        foreach(var entry in entries) {
            total += Mathf.Max(entry.weight, 0f);
        }

        float random = Random.Range(0, total);

        total = 0;
        foreach(var entry in entries) {
            total += entry.weight;
            if (total >= random)
                return entry.value;
        }

        return entries[entries.Count - 1].value;
    }
}
