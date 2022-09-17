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
  RectTransform? contentTransform;

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
    // TODO: Check pra evitar re render do layout do mapa

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

    foreach (var entry in network.roomNodes)
    {
      var roomNode = entry.Value;

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

      var icon = Instantiate(chosenPrefab, contentTransform);

      var position = new Vector3
      {
        x = roomNode.room.transform.position.x,
        y = roomNode.room.transform.position.z,
      } * 1f / minimapScale;

      icon.GetComponent<RectTransform>().localPosition = position;

      icons.Add(icon);
    }

    foreach (var entry in network.roomEdges)
    {
      var roomEdge = entry.Value;

      var a = roomEdge.a;
      var b = roomEdge.b;

      var connection = new GameObject("Connection");

      var position = new Vector3
      {
        x = a.room.transform.position.x,
        y = a.room.transform.position.z,
      } * 1f / minimapScale;

      var position2 = new Vector3
      {
        x = b.room.transform.position.x,
        y = b.room.transform.position.z,
      } * 1f / minimapScale;

      var diff = position2 - position;

      connection.transform.SetParent(contentTransform, false);
      connection.transform.localPosition = position;

      var lineRenderer = connection.AddComponent<UILineRenderer>();

      lineRenderer.LineThickness = 6f;
      lineRenderer.color = new Color(0.4f, 0.4f, 0.4f, .6f);
      // lineRenderer.;
      // TODO: fix, tirar opacidade talvez
      // TODO: fixcolocar mask em volta dos ícones
      // TODO: fix fazer mecanica de highlight do mapa
      lineRenderer.Points = new Vector2[]{
        Vector2.zero,
        diff
      };

      connections.Add(connection);
    }

    // TODO: Fazer o layout do minimapa usando a room network

    // foreach (var neighbor in roomNode.neighbors)
    // {
    //   var connection = new GameObject("Connection");

    //   connection.transform.SetParent(icon.transform);
    //   connection.transform.SetSiblingIndex(0);
    //   connection.transform.localPosition = Vector3.zero;

    //   var lineRenderer = connection.AddComponent<UILineRenderer>();

    //   lineRenderer.LineThickness = 6f;
    //   lineRenderer.color = new Color(.5f, .5f, .5f, .6f);

    //   var nPosition = new Vector3
    //   {
    //     x = neighbor.room.transform.position.x,
    //     y = neighbor.room.transform.position.z,
    //   } * 1f / minimapScale;

    //   var diff = nPosition - roomPosition;

    //   lineRenderer.Points = new Vector2[] {
    //     Vector2.zero,
    //     new Vector2 { x = diff.x, y = diff.y }
    //   };
    // }
  }

  void Awake()
  {
    contentTransform = transform.Find("Background/Content")
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
