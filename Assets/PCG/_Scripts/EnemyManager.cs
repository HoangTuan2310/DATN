using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance;

    private readonly List<GameObject> aliveEnemies = new List<GameObject>();

    public int TotalSpawned { get; private set; } = 0;
    public int TotalDestroyed { get; private set; } = 0;

    public int CurrentEnemyCount => aliveEnemies.Count;

    public bool IsAllEnemiesDead => aliveEnemies.Count == 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void RegisterEnemy(GameObject enemy)
    {
        if (enemy == null) return;

        if (!aliveEnemies.Contains(enemy))
        {
            aliveEnemies.Add(enemy);
            TotalSpawned++;
            Debug.Log("[EnemyManager] Spawned: " + enemy.name +
                      " | Total Spawned: " + TotalSpawned +
                      " | Alive: " + aliveEnemies.Count);
        }
    }

    public void UnregisterEnemy(GameObject enemy)
    {
        if (enemy == null) return;

        if (aliveEnemies.Contains(enemy))
        {
            aliveEnemies.Remove(enemy);
            TotalDestroyed++;
            Debug.Log("[EnemyManager] Destroyed: " + enemy.name +
                      " | Total Destroyed: " + TotalDestroyed +
                      " | Alive: " + aliveEnemies.Count);
        }
    }

    public void ClearAll()
    {
        aliveEnemies.Clear();
        TotalSpawned = 0;
        TotalDestroyed = 0;
    }

    public List<GameObject> GetAliveEnemies()
    {
        return new List<GameObject>(aliveEnemies);
    }
}
