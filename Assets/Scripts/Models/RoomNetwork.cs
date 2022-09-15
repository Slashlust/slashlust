using System.Collections.Generic;
using UnityEngine;

#nullable enable

public class RoomNetwork
{
  public Dictionary<int, RoomNode> roomNodes = new Dictionary<int, RoomNode>();

  public void AddRoom(GameObject room)
  {
    var roomNode = new RoomNode { room = room };

    roomNodes.Add(room.GetInstanceID(), roomNode);
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
}
