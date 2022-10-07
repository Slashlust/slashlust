using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#nullable enable

public class RoomNode
{
  public GameObject room = default!;
  public RoomScript roomScript = default!;
  public HashSet<RoomNode> neighbors = new HashSet<RoomNode>();

  public void AddNeighbor(RoomNode neighbor)
  {
    neighbors.Add(neighbor);
  }

  override public string ToString()
  {
    var n = "";

    foreach (var item in neighbors.ToList())
    {
      n += item.room.GetInstanceID() + ", ";
    }

    return $"instance id: {room.GetInstanceID()}, neighbors: {n}";
  }
}
