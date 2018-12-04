using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MedianSpawner : ProgrammableSpawner {

    private const float Y_POSITION_OF_TELEGRAPHS = 3.7f;

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
            2f
        );

        var median = Instantiate(prefab, position, Quaternion.identity).GetComponent<Median>();
        median.Initialize();

        median.SetSize(lanesWide, screensTall);
        median.moveTimer = moveWaitTime;

        for(var i = 0; i < lanesWide; i++) {
            SpawnTelegraph(median.transform, lane + i);
        }
    }

    protected void SpawnTelegraph(Transform transform, int lane) {
        var telegraphSize = spawnTelegraph.GetComponent<SpriteRenderer>().bounds.size;
        var position = new Vector3(
            lanes[lane] + (Median.WIDTH_PER_LANE /2f),
            Y_POSITION_OF_TELEGRAPHS - telegraphSize.y / 2f,
            transform.position.z
        );

        Instantiate(spawnTelegraph, position, Quaternion.identity, transform);

        AudioManager.instance.PlaySound("obstacle_warning");
    }
}
