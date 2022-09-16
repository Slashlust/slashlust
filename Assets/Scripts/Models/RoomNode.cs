using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#nullable enable

public class RoomNode
{
  public GameObject room = default!;
  public bool visited;
  public HashSet<RoomNode> neighbors = new HashSet<RoomNode>();

  public void AddNeighbor(RoomNode neighbor)
  {
    neighbors.Add(neighbor);
  }

  override public string ToString()
  {
    var n = neighbors.ToList().Select(e => e.room.GetInstanceID());

    return $"instance id: {room.GetInstanceID()}, visited: {visited}, neighbors: {n}";
  }
}
