using UnityEngine;

#nullable enable

[System.Serializable]
public class EnemyDrop
{
  public GameObject prefab = default!;
  [Range(0f, 1f)]
  public float chance = .1f;
}
