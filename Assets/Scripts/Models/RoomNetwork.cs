using System.Collections.Generic;
using UnityEngine;

#nullable enable

public class RoomNetwork
{
  public Dictionary<int, RoomNode> roomNodes = new Dictionary<int, RoomNode>();
  public Dictionary<string, RoomEdge> roomEdges =
    new Dictionary<string, RoomEdge>();
  public RoomNode? root;
  public RoomNode? bossRoom;
  public List<RoomNode>? targetPath;

  public bool hasBossRoomSpawned = false;

  public void AddRoom(GameObject room, bool isRoot)
  {
    var roomNode = new RoomNode
    {
      room = room,
      roomScript = room.GetComponent<RoomScript>(),
    };

    roomNodes.Add(room.GetInstanceID(), roomNode);

    if (isRoot)
    {
      root = roomNode;
    }
  }

  public List<RoomNode>? AStar(RoomNode start, RoomNode end)
  {
    // Defina a lista de nós visitados
    var visited = new HashSet<RoomNode>();

    // Defina a lista de nós para visitar
    var open = new List<RoomNode>();

    // Defina um dicionário para mapear os nós visitados para seus respectivos nós anteriores
    var cameFrom = new Dictionary<RoomNode, RoomNode>();

    // Defina um dicionário para mapear os nós visitados para suas respectivas distâncias
    var costSoFar = new Dictionary<RoomNode, int>();

    // Adicione o nó inicial à lista de nós para visitar
    open.Add(start);

    // Defina a distância do nó inicial como 0
    costSoFar[start] = 0;

    // Enquanto a lista de nós para visitar não estiver vazia
    while (open.Count > 0)
    {
      // Defina o nó atual como o nó com menor distância na lista de nós para visitar
      var current = open[0];
      for (int i = 0; i < open.Count; i++)
      {
        if (costSoFar[open[i]] < costSoFar[current])
        {
          current = open[i];
        }
      }

      // Se o nó atual for o nó final, então retorne o caminho
      if (current == end)
      {
        return ReconstructPath(cameFrom, current);
      }

      // Remova o nó atual da lista de nós para visitar
      open.Remove(current);
      // Marque o nó atual como visitado
      visited.Add(current);

      // Para cada vizinho do nó atual
      foreach (var neighbor in current.neighbors)
      {
        // Se o vizinho já tiver sido visitado, ignore-o
        if (visited.Contains(neighbor))
        {
          continue;
        }

        // Defina a distância estimada do vizinho como a distância do nó atual + 1
        int estimatedDistance = costSoFar[current] + 1;

        // Se o vizinho ainda não estiver na lista de nós para visitar ou se a distância estimada for menor que a distância registrada para o vizinho
        if (!open.Contains(neighbor) || estimatedDistance < costSoFar[neighbor])
        {
          // Adicione o vizinho à lista de nós para visitar
          open.Add(neighbor);
          // Atualize o nó anterior para o vizinho para o nó atual
          cameFrom[neighbor] = current;
          // Atualize a distância registrada para o vizinho para a distância estimada
          costSoFar[neighbor] = estimatedDistance;
        }
      }
    }

    // Se o algoritmo chegar aqui, isso significa que não existe um caminho até o nó final
    return null;
  }

  public void ConnectRooms(int instanceIDA, int instanceIDB)
  {
    var roomNodeA = roomNodes[instanceIDA];
    var roomNodeB = roomNodes[instanceIDB];

    roomNodeA.AddNeighbor(roomNodeB);
    roomNodeB.AddNeighbor(roomNodeA);

    var id = instanceIDA > instanceIDB
      ? $"{instanceIDB} {instanceIDA}" : $"{instanceIDA} {instanceIDB}";

    if (!roomEdges.ContainsKey(id))
    {
      roomEdges.Add(id, new RoomEdge { a = roomNodeA, b = roomNodeB });
    }
  }

  public void DebugDrawEdges()
  {
    foreach (var entry in roomEdges)
    {
      var roomEdge = entry.Value;

      Debug.DrawLine(
        roomEdge.a.room.transform.position + Vector3.up * 5,
        roomEdge.b.room.transform.position + Vector3.up * 5,
        Color.red
      );
    }
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
          Color.yellow
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
          roomNode.room.transform.position + Vector3.up,
          roomNodeCache.room.transform.position + Vector3.up,
          Color.green
        );
      }

      roomNodeCache = roomNode;
    }
  }

  public List<RoomNode> ReconstructPath(Dictionary<RoomNode, RoomNode> cameFrom, RoomNode current)
  {
    // Crie uma lista vazia para o caminho
    var path = new List<RoomNode>();

    // Enquanto o nó atual tiver um nó anterior
    while (current != null)
    {
      // Inseria o nó atual no início da lista
      path.Insert(0, current);

      try
      {
        // Atualize o nó atual para o nó anterior
        current = cameFrom[current];
      }
      catch (System.Exception)
      {
        break;
      }
    }

    // Retorne o caminho
    return path;
  }
}
