using System.Collections.Generic;
using UnityEngine;

#nullable enable

[System.Serializable]
public class EnemySpawnSettings
{
  public bool isEnemySpawnEnabled;

  public List<GameObject> enemyPrefabs = default!;

  public GameObject GetRandomEnemyPrefab()
  {
    return enemyPrefabs[Random.Range(0, enemyPrefabs.Count)];
  }
}
