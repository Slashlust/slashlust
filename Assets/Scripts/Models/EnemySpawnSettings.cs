using System.Collections.Generic;
using UnityEngine;

#nullable enable

[System.Serializable]
public class EnemySpawnSettings
{
  public bool isEnemySpawnEnabled;
  [Min(1)]
  public int spawnBatchSize;
  [Min(1f)]
  public float spawnInterval;
  [Min(1)]
  public int maxEnemiesAlive = 5;

  public List<GameObject> enemyPrefabs = default!;

  public GameObject GetRandomEnemyPrefab()
  {
    return enemyPrefabs[Random.Range(0, enemyPrefabs.Count)];
  }
}
