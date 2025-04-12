using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject objectToSpawn;
    [HideInInspector] public int spawnCount;

    public Vector2Int gridSize = new Vector2Int(10, 13);
    public Transform[] tilePositions;
    private TileType[,] grid;

    public void SpawnEnemies()
    {
        if (objectToSpawn == null || tilePositions == null || tilePositions.Length == 0 || grid == null)
            return;

        spawnCount = Random.Range(2, 6);

        int width = grid.GetLength(0);
        int height = grid.GetLength(1);

        int spawned = 0;
        int attempts = 0;

        while (spawned < spawnCount && attempts < 100)
        {
            int index = Random.Range(0, tilePositions.Length);
            Transform tile = tilePositions[index];

            if (tile == null) { attempts++; continue; }

            int x = index % width;
            int y = index / width;

            if (y >= 9 || x < 0 || x >= width || y < 0 || y >= height)
            {
                attempts++;
                continue;
            }

            if (grid[x, y] != TileType.Floor)
            {
                attempts++;
                continue;
            }

            GameObject obj = Instantiate(objectToSpawn, tile.position, Quaternion.identity);
            EnemyTracker.Register(obj);

            spawned++;
        }
    }

    public void SetGrid(TileType[,] sourceGrid)
    {
        grid = sourceGrid;
    }
}
