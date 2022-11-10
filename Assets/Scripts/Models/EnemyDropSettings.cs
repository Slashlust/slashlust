using System.Collections.Generic;
using UnityEngine;

#nullable enable

[System.Serializable]
public class EnemyDropSettings
{
  public bool enableDrops = true;
  public List<EnemyDrop> enemyDrops = default!;

  public List<GameObject> RandomizeDropPrefabs()
  {
    var drops = new List<GameObject>();

    foreach (var item in enemyDrops)
    {
      if (Random.value <= item.chance)
      {
        drops.Add(item.prefab);
      }
    }

    return drops;
  }
}
