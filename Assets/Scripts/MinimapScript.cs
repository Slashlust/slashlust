using System.Collections.Generic;
using UnityEngine;

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

    icons.Clear();

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
        Debug.Log("chosenPrefab null");

        continue;
      }

      var icon = Instantiate(chosenPrefab, contentTransform);

      icon.GetComponent<RectTransform>().localPosition = new Vector3
      {
        x = roomNode.room.transform.position.x,
        y = roomNode.room.transform.position.z,
      } * 1f / minimapScale;

      icons.Add(icon);

      // TODO: Fazer o layout do minimapa usando a room network

      // foreach (var neighbor in roomNode.neighbors)
      // {
      //   Debug.DrawLine(
      //     roomNode.room.transform.position,
      //     neighbor.room.transform.position,
      //     Color.yellow
      //   );
      // }
    }
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
