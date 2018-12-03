using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MedianSpawner : ProgrammableSpawner {

    protected override void Spawn() {
        float screensTall = Mathf.Max(medianLengthDistribution.Draw(), 0.5f);

        //mark what lanes will have medians then walk the array and spawn then in chunks
        var mediansAt = GetSpawnLocations(screensTall * 1f);
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
}
