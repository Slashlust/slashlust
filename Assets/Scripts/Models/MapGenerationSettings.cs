using System.Collections.Generic;
using UnityEngine;

#nullable enable

[System.Serializable]
public class MapGenerationSettings
{
  public int minRoomCount;

  public GameObject? deadEndPrefab;
  public GameObject? corridorPrefab;
  public List<GameObject> roomPrefabs = default!;

  public GameObject GetRandomRoomPrefab()
  {
    return roomPrefabs[Random.Range(0, roomPrefabs.Count)];
  }
}
