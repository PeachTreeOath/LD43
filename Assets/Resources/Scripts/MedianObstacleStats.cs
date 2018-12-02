using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MedianObstacleStats : ObstacleStats {

    protected override void Start() {

    }

    // :-/  I guess we're ignoring these inputs for now
    public override void Spawn(Vector2 startPosition, Vector2 endPosition) {
        var prefab = GetObstaclePrefab();

        int lanesWide = 1;
        float screensTall = 3;

        int lane = Random.Range(0, 5);

        var position = new Vector3(
            Median.LEFT_EDGE_OF_FIRST_LANE + lane * Median.WIDTH_PER_LANE - ((Median.WIDTH_PER_LANE * (float)lanesWide) / 2f),
            screensTall * Median.HEIGHT_PER_SCREEN, //TODO this isn't quite right
            0f
        );

        var median = Instantiate(prefab, position, Quaternion.identity).GetComponent<Median>();
        median.Initialize();

        median.SetSize(lanesWide, screensTall);
        median.SpawnTelegraphsAlongWidth(spawnTelegraph);
        median.moveTimer = this.moveWaitTime;
    }
}
