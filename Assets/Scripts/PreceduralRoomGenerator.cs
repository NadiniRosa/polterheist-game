using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public enum TileType
{
    Floor,
    Box,
    Trap
}

public class ProceduralRoomGenerator : MonoBehaviour
{
    [Header("Grid Settings")]
    public int width = 10;
    public int height = 13;
    [Space(2)]

    [Header("Number of Boxes and Traps")]
    public int obstacles = 8;
    [Space(2)]

    [Header("Prefabs")]
    public GameObject boxPrefab;
    public GameObject trapPrefab;
    [Space(2)]

    private TileType[,] grid;
    private Transform[] tilePositions;

    private HashSet<Vector2Int> reservedPathTiles = new HashSet<Vector2Int>();
    private List<GameObject> spawnedTiles = new List<GameObject>();
    
    public Transform dynamicTileContainer;
    public Transform floorTileParent;

    public EnemySpawner enemySpawner;

    void Start()
    {
        GenerateRoom();
    }

    public void GenerateRoom()
    {
        ClearPreviousRoom();
        reservedPathTiles.Clear();

        grid = new TileType[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                grid[x, y] = TileType.Floor;
            }
        }

        ReserveDoorZone(3, 0);           // Top
        ReserveDoorZone(3, height - 3);  // Bottom

        GenerateGuaranteedPath();
        PlaceObstacles(obstacles);
        SpawnTiles();

        if (enemySpawner != null)
        {
            enemySpawner.tilePositions = GetCurrentTilePositions();
            enemySpawner.SetGrid(grid);
            enemySpawner.SpawnEnemies();
        }
    }

    void ReserveDoorZone(int startX, int startY)
    {
        for (int x = startX; x < startX + 3; x++)
        {
            for (int y = startY; y < startY + 3; y++)
            {
                if (x >= 0 && x < width && y >= 0 && y < height)
                {
                    grid[x, y] = TileType.Floor;
                }
            }
        }
    }

    void PlaceObstacles(int amount)
    {
        int attempts = 0;
        int placed = 0;

        while (placed < amount && attempts < amount * 10)
        {
            int x = Random.Range(0, width);
            int y = Random.Range(0, height);

            if (IsInReservedDoorZone(x, y) || grid[x, y] != TileType.Floor)
            {
                attempts++;
                continue;
            }

            grid[x, y] = Random.value > 0.5f ? TileType.Box : TileType.Trap;
            placed++;
        }
    }

    bool IsInReservedDoorZone(int x, int y)
    {
        // Top reserved area: x 3–5, y 0–2
        if (x >= 3 && x <= 5 && y >= 0 && y <= 2) return true;

        // Bottom reserved area: x 3–5, y 10–12
        if (x >= 3 && x <= 5 && y >= 10 && y <= 12) return true;

        return reservedPathTiles.Contains(new Vector2Int(x, y));
    }

    void SpawnTiles()
    {
        Transform[] tilePositions = GetCurrentTilePositions();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int index = y * width + x;
                if (index >= tilePositions.Length) continue;

                Vector3 pos = tilePositions[index].position;

                GameObject prefabToSpawn = null;

                switch (grid[x, y])
                {
                    case TileType.Box:
                        prefabToSpawn = boxPrefab;
                        break;
                    case TileType.Trap:
                        prefabToSpawn = trapPrefab;
                        break;
                    default:
                        continue;
                }

                if (prefabToSpawn != null)
                {
                    GameObject instance = Instantiate(prefabToSpawn, pos, Quaternion.identity, dynamicTileContainer);
                    spawnedTiles.Add(instance);
                }
            }
        }
    }


    void ClearPreviousRoom()
    {
        foreach (var obj in spawnedTiles)
        {
            if (obj != null) Destroy(obj);
        }

        spawnedTiles.Clear();
    }


    void GenerateGuaranteedPath()
    {
        Vector2Int start = new Vector2Int(Random.Range(3, 6), Random.Range(10, 13));
        Vector2Int end = new Vector2Int(Random.Range(3, 6), Random.Range(0, 3));

        Vector2Int current = start;
        reservedPathTiles.Add(current);

        while (current.y > end.y)
        {
            List<Vector2Int> possibleMoves = new List<Vector2Int>();

            if (current.y > 0)
                possibleMoves.Add(new Vector2Int(current.x, current.y - 1));
            if (current.x > 0)
                possibleMoves.Add(new Vector2Int(current.x - 1, current.y));
            if (current.x < width - 1)
                possibleMoves.Add(new Vector2Int(current.x + 1, current.y));

            possibleMoves = possibleMoves.OrderBy(x => Random.value).ToList();

            foreach (var move in possibleMoves)
            {
                if (!reservedPathTiles.Contains(move))
                {
                    current = move;
                    reservedPathTiles.Add(current);
                    break;
                }
            }
        }

        foreach (var tile in reservedPathTiles)
        {
            grid[tile.x, tile.y] = TileType.Floor;
        }
    }

    Transform[] GetCurrentTilePositions()
    {
        return floorTileParent.GetComponentsInChildren<Transform>()
            .Where(t => t != floorTileParent)
            .OrderByDescending(t => t.position.y)
            .ThenBy(t => t.position.x)
            .ToArray();
    }
}

