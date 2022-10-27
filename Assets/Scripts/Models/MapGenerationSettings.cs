using System.Collections.Generic;
using UnityEngine;

#nullable enable

[System.Serializable]
public class MapGenerationSettings
{
  public int minRoomCount;
  [Range(0f, 1f)]
  public float roomConnectionChance = .2f;

  public GameObject? deadEndPrefab;
  public GameObject? corridorPrefab;
  public GameObject? bossRoomPrefab;
  public List<GameObject> roomPrefabs = default!;

  public GameObject GetRandomRoomPrefab()
  {
    return roomPrefabs[Random.Range(0, roomPrefabs.Count)];
  }
}
