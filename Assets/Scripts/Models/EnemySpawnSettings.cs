using System.Collections.Generic;
using UnityEngine;

#nullable enable

[System.Serializable]
public class EnemySpawnSettings
{
  [Header("General spawn settings")]
  public bool isEnemySpawnEnabled;
  [Min(0f)]
  public float difficultyIndexWeight = 1f;
  public AnimationCurve difficultyCurve = default!;

  [Header("Room spawn settings")]
  [Min(1)]
  public int spawnBatchSize;

  public GameObject bossPrefab = default!;
  public List<GameObject> enemyPrefabs = default!;

  public GameObject GetRandomEnemyPrefab()
  {
    return enemyPrefabs[Random.Range(0, enemyPrefabs.Count)];
  }
}
