using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MedianObstacleStats : ObstacleStats {

    private const int NUM_LANES = 5;

    protected override void Start() {
        spawnTimer = firstSpawnTime;
    }

    // :-/  I guess we're ignoring these inputs for now
    public override void Spawn(Vector2 startPosition, Vector2 endPosition) {
        /*
        var prefab = GetObstaclePrefab();

        int lanesWide = Random.Range(1, 4);
        float screensTall = Random.Range(1, 12) * 0.5f;

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
        median.moveTimer = this.moveWaitTime;
        */
    }
}
