using System.Collections.Generic;
using UnityEngine;

#nullable enable

public class RoomNode
{
  public GameObject room = default!;
  public HashSet<RoomNode> neighbors = new HashSet<RoomNode>();

  public void AddNeighbor(RoomNode neighbor)
  {
    neighbors.Add(neighbor);
  }
}
