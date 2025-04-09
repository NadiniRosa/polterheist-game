using UnityEngine;
using System.Collections.Generic;

public class EnemyTracker : MonoBehaviour
{
    public static List<GameObject> activeEnemies = new List<GameObject>();

    public static void Register(GameObject enemy)
    {
        if (!activeEnemies.Contains(enemy))
            activeEnemies.Add(enemy);
    }

    public static void Unregister(GameObject enemy)
    {
        if (activeEnemies.Contains(enemy))
            activeEnemies.Remove(enemy);
    }

    public static bool AllEnemiesDefeated()
    {
        return activeEnemies.Count == 0;
    }
}
