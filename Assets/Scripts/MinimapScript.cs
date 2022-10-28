using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.Extensions;

#nullable enable

public class MinimapScript : MonoBehaviour
{
  [SerializeField]
  [Range(.1f, 10f)]
  float minimapScale = 1f;

  [SerializeField]
  GameObject? roomIconPrefab;
  [SerializeField]
  GameObject? spawnIconPrefab;
  [SerializeField]
  GameObject? bossRoomIconPrefab;

  // Referência.
  RectTransform? roomsTransform;
  RectTransform? contentTransform;
  RectTransform? connectionsTransform;

  List<GameObject> icons = new List<GameObject>();
  List<GameObject> connections = new List<GameObject>();

  void HandleContentTransform(RectTransform contentTransform)
  {
    var player = GameManagerScript.instance.GetPlayer;

    if (player == null)
    {
      return;
    }

    var position = player.transform.position;

    var offset = new Vector2 { x = position.x, y = position.z } / -minimapScale;

    contentTransform.localPosition = offset;
  }

  public void Layout(RoomNetwork network)
  {
    Debug.Log("Minimap redrawn");

    var targetPath = network.targetPath;

    foreach (var icon in icons)
    {
      Destroy(icon);
    }

    foreach (var connection in connections)
    {
      Destroy(connection);
    }

    icons.Clear();
    connections.Clear();

    foreach (var entry in network.roomEdges)
    {
      var roomEdge = entry.Value;

      var a = roomEdge.a;
      var b = roomEdge.b;

      var connection = new GameObject("Connection");

      var positionA = new Vector3
      {
        x = a.room.transform.position.x,
        y = a.room.transform.position.z,
      } * 1f / minimapScale;

      var positionB = new Vector3
      {
        x = b.room.transform.position.x,
        y = b.room.transform.position.z,
      } * 1f / minimapScale;

      var diff = positionB - positionA;

      connection.transform.SetParent(connectionsTransform, false);
      connection.transform.localPosition = positionA;

      var lineRenderer = connection.AddComponent<UILineRenderer>();

      lineRenderer.color = new Color(0.6f, 0.6f, 0.6f);

      lineRenderer.LineThickness = 6f;
      lineRenderer.Points = new Vector2[]{
        Vector2.zero,
        diff
      };

      connections.Add(connection);
    }

    foreach (var entry in network.roomNodes)
    {
      var roomNode = entry.Value;

      var room = roomNode.room;

      GameObject? chosenPrefab = null;

      switch (roomNode.roomScript.roomType)
      {
        case RoomType.boss:
          chosenPrefab = bossRoomIconPrefab;
          break;
        case RoomType.regular:
          chosenPrefab = roomIconPrefab;
          break;
        case RoomType.spawn:
          chosenPrefab = spawnIconPrefab;
          break;
      }

      if (chosenPrefab == null)
      {
        continue;
      }

      var icon = Instantiate(chosenPrefab, roomsTransform);

      var position = new Vector3
      {
        x = room.transform.position.x,
        y = room.transform.position.z,
      } * 1f / minimapScale;

      icon.GetComponent<RectTransform>().localPosition = position;

      if (targetPath?.Contains(roomNode) == true)
      {
        icon.GetComponent<MinimapIconScript>().Paint(new Color(.3f, .8f, .4f));
      }

      icons.Add(icon);
    }

    if (targetPath != null)
    {
      RoomNode? lastItem = null;

      foreach (var item in targetPath)
      {
        if (lastItem != null)
        {
          var connection = new GameObject("Connection");

          var positionA = new Vector3
          {
            x = item.room.transform.position.x,
            y = item.room.transform.position.z,
          } * 1f / minimapScale;

          var positionB = new Vector3
          {
            x = lastItem.room.transform.position.x,
            y = lastItem.room.transform.position.z,
          } * 1f / minimapScale;

          var diff = positionB - positionA;

          connection.transform.SetParent(connectionsTransform, false);
          connection.transform.localPosition = positionA;

          var lineRenderer = connection.AddComponent<UILineRenderer>();

          lineRenderer.color = new Color(.2f, .7f, .3f);

          lineRenderer.LineThickness = 6f;
          lineRenderer.Points = new Vector2[]{
            Vector2.zero,
            diff
          };

          connections.Add(connection);
        }

        lastItem = item;
      }
    }
  }

  void Awake()
  {
    roomsTransform = transform.Find("Background/Content/Rooms")
      .GetComponent<RectTransform>();
    contentTransform = transform.Find("Background/Content")
      .GetComponent<RectTransform>();
    connectionsTransform = transform.Find("Background/Content/Connections")
      .GetComponent<RectTransform>();
  }

  void Update()
  {
    if (contentTransform != null)
    {
      HandleContentTransform(contentTransform);
    }
  }
}
