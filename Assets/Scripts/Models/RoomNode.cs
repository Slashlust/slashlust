using System.Collections.Generic;
using UnityEngine;

#nullable enable

public class RoomNode
{
  public GameObject room = default!;
  public HashSet<RoomNode> neighbors = new HashSet<RoomNode>();

  public void AddNeighbor(RoomNode neighbor)
  {
    Debug.Log("ID ADD NEIGHBOR -> " + neighbor.room.GetInstanceID());
    neighbors.Add(neighbor);
  }
}
