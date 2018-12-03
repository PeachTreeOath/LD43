using UnityEngine;
using System.Collections;

public class PedestrianSpawner : ProgrammableSpawner {

    public ObstacleStats obstacleStats;
    public float closeLaneDuration = 0.25f;

    protected override void Spawn() {
        bool[] spawnAt = GetSpawnLocations(closeLaneDuration);
        for(var i = 0; i < spawnAt.Length; i++) {
            if (spawnAt[i]) SpawnPedestrian(i);
        }
    }

    private void SpawnPedestrian(int lane) {
        float randomX = UnityEngine.Random.Range(GameManager.instance.upperLeftBound.x, GameManager.instance.bottomRightBound.x);
        float randomY = UnityEngine.Random.Range(GameManager.instance.upperLeftBound.y, GameManager.instance.bottomRightBound.y);

        Vector2 startPosition = new Vector2(lanes[lane] + Median.WIDTH_PER_LANE / 2f, 6f);
		Vector3 endPosition = new Vector3(randomX, randomY, 0);

        var obstacleGO = ResourceLoader.instance.obstaclePedestrianPrefab;
        var obstacle = Instantiate(obstacleGO, startPosition, Quaternion.identity).GetComponent<ObstacleController>();
        obstacle.endPosition = endPosition;
        obstacle.obstacleStats = obstacleStats;
    }
}
