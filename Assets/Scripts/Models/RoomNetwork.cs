using System.Collections.Generic;
using UnityEngine;

#nullable enable

public class RoomNetwork
{
  public Dictionary<int, RoomNode> roomNodes = new Dictionary<int, RoomNode>();
  public RoomNode? root;
  public RoomNode? bossRoom;

  public void AddRoom(GameObject room, bool isRoot)
  {
    var roomNode = new RoomNode { room = room };


    roomNodes.Add(room.GetInstanceID(), roomNode);

    if (isRoot)
    {
      root = roomNode;
    }
  }

  public void ConnectRooms(int instanceIDA, int instanceIDB)
  {
    var roomNodeA = roomNodes[instanceIDA];
    var roomNodeB = roomNodes[instanceIDB];

    roomNodeA.AddNeighbor(roomNodeB);
    roomNodeB.AddNeighbor(roomNodeA);
  }

  public void DebugDrawNetwork()
  {
    foreach (var entry in roomNodes)
    {
      var roomNode = entry.Value;

      foreach (var neighbor in roomNode.neighbors)
      {
        Debug.DrawLine(
          roomNode.room.transform.position,
          neighbor.room.transform.position,
          Color.green
        );
      }
    }
  }

  public void DebugDrawPath(List<RoomNode> path)
  {
    RoomNode? roomNodeCache = null;
    foreach (var roomNode in path)
    {
      if (roomNodeCache != null)
      {
        Debug.DrawLine(
          roomNode.room.transform.position,
          roomNodeCache.room.transform.position,
          Color.green
        );
      }
      roomNodeCache = roomNode;
    }
  }

  public List<RoomNode> FindPath(RoomNode start, RoomNode end)
  {
    Queue<RoomNode> queue = new Queue<RoomNode>();
    List<RoomNode> visited = new List<RoomNode>();
    List<RoomNode> path = new List<RoomNode>();

    queue.Enqueue(start);
    visited.Add(start);

    while (queue.Count > 0)
    {
      RoomNode current = queue.Dequeue();
      path.Add(current);

      if (current == end)
      {
        return path;
      }

      foreach (RoomNode neighbor in current.neighbors)
      {
        if (!visited.Contains(neighbor))
        {
          queue.Enqueue(neighbor);
          visited.Add(neighbor);
        }
      }
    }

    return path;
  }
}
