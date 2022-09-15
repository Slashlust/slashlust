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
    Debug.Log("ID ADDROOM -> " + room.GetInstanceID());

    if (isRoot) {
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


  public List<RoomNode>? AStar(int start, int bossRoom)
  {
    var startNode = roomNodes[start];
    var bossNode = roomNodes[bossRoom];
    // Defina a lista de nós visitados
    HashSet<RoomNode> visited = new HashSet<RoomNode>();
    // Defina a lista de nós para visitar
    List<RoomNode> open = new List<RoomNode>();
    // Defina um dicionário para mapear os nós visitados para seus respectivos nós anteriores
    Dictionary<RoomNode, RoomNode> cameFrom = new Dictionary<RoomNode, RoomNode>();
    // Defina um dicionário para mapear os nós visitados para suas respectivas distâncias
    Dictionary<RoomNode, int> costSoFar = new Dictionary<RoomNode, int>();

    // Adicione o nó inicial à lista de nós para visitar
    open.Add(startNode);

    // Defina a distância do nó inicial como 0
    costSoFar[startNode] = 0;

    // Enquanto a lista de nós para visitar não estiver vazia
    while (open.Count > 0)
    {
      // Defina o nó atual como o nó com menor distância na lista de nós para visitar
      RoomNode current = open[0];
      for (int i = 0; i < open.Count; i++)
      {
        if (costSoFar[open[i]] < costSoFar[current])
        {
          current = open[i];
        }
      }

      // Se o nó atual for o nó final, então retorne o caminho
      if (current.room == bossNode.room)
      {
        return reconstructPath(cameFrom, current);
      }

      // Remova o nó atual da lista de nós para visitar
      open.Remove(current);
      // Marque o nó atual como visitado
      visited.Add(current);

      // Para cada vizinho do nó atual
      foreach (RoomNode neighbor in current.neighbors)
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

  public List<RoomNode> reconstructPath(Dictionary<RoomNode, RoomNode> cameFrom, RoomNode current)
  {
    // Crie uma lista vazia para o caminho
    List<RoomNode> path = new List<RoomNode>();
    path.Add(current);
    // Enquanto o nó atual tiver um nó anterior
    while (current != null)
    {
      // Inseria o nó atual no início da lista
      path.Insert(0, current);
      // Atualize o nó atual para o nó anterior
      current = cameFrom[current];
    }

    // Retorne o caminho
    return path;
  }

}
