using System.Collections.Generic;
using UnityEngine;

#nullable enable

public class RoomScript : MonoBehaviour
{
  [SerializeField]
  bool isRoot = false;
  [SerializeField]
  Vector3 dimensions;

  public RoomType roomType;

  public Vector3 GetDimensions => dimensions;

  System.Collections.IEnumerator BakeNavMesh()
  {
    yield return new WaitForSecondsRealtime(2f);

    GameManagerScript.instance.BakeNavMesh();
  }

  public void GenerateRooms()
  {
    var manager = GameManagerScript.instance;

    var network = manager.GetRoomNetwork;

    var attachments = GetAttachments();

    var roomPrefabs = manager.GetMapGenerationSettings.roomPrefabs;

    var corridorPrefab = manager.GetMapGenerationSettings.corridorPrefab!;

    var deadEndPrefab = manager.GetMapGenerationSettings.deadEndPrefab!;

    void GenerateDeadEnd(GameObject attachment)
    {
      var deadEnd = Instantiate(
        deadEndPrefab,
        attachment.transform.position,
        attachment.transform.rotation
      );

      deadEnd.GetComponent<DeadEndScript>().room = gameObject;
    }

    foreach (var attachment in attachments)
    {
      if (
        network.roomNodes.Count >=
        manager.GetMapGenerationSettings.minRoomCount
      )
      {
        RaycastHit hit;
        if (
          Physics.Raycast(
            attachment.transform.position + Vector3.up,
            attachment.transform.forward,
            out hit,
            9f,
            Layers.geometryMask
          )
        )
        {
          if (hit.collider.name == "DeadEndModel")
          {
            // TODO: Trabalhar um atributo de chance de conexão de salas

            var deadEnd = hit.collider.transform.parent.gameObject;

            var deadEndRoom = deadEnd.GetComponent<DeadEndScript>().room;

            // TODO: Talvez não seja necessário, mas removendo a layer de
            // geometry do dead end para evitar que ele impacte nos raycasts ou
            // geração do navmesh
            deadEnd.layer = Layers.defaultLayer;

            Destroy(deadEnd);

            // TODO: Melhorar fluxo de conexão de salas

            var attachmentCorridorScript2 =
              corridorPrefab.GetComponent<CorridorScript>();

            var corridorLength2 = attachmentCorridorScript2.GetDimensions.z;

            Instantiate(
              corridorPrefab,
              attachment.transform.position + attachment.transform.forward *
                corridorLength2 / 2f,
              attachment.transform.rotation
            );

            network.ConnectRooms(
              gameObject.GetInstanceID(),
              deadEndRoom.GetInstanceID()
            );

            continue;
          }
        }

        // TODO: Trabalhar probabilidade de geração de cada attachment.
        if (Random.value > 0.2f)
        {
          GenerateDeadEnd(attachment: attachment);

          continue;
        }
      }

      var roomPrefab = manager.GetMapGenerationSettings.GetRandomRoomPrefab();

      var attachmentRoomScript = roomPrefab.GetComponent<RoomScript>();

      var attachmentCorridorScript =
        corridorPrefab.GetComponent<CorridorScript>();

      var corridorLength = attachmentCorridorScript.GetDimensions.z;

      var roomLength = attachmentRoomScript.GetDimensions.z;

      if (
        !IsRoomPlacementAvailable(
          checkArea: attachmentRoomScript.GetDimensions +
            attachmentCorridorScript.GetDimensions,
          attachment.transform
        )
      )
      {
        GenerateDeadEnd(attachment: attachment);

        continue;
      }

      Instantiate(
        corridorPrefab,
        attachment.transform.position + attachment.transform.forward *
          corridorLength / 2f,
        attachment.transform.rotation
      );

      var room = Instantiate(
        roomPrefab,
        attachment.transform.position + attachment.transform.forward *
          (roomLength / 2f + corridorLength),
        attachment.transform.rotation
      );

      room.transform.SetParent(manager.GetGeometry.transform);

      network.AddRoom(room, false);

      var roomScript = room.GetComponent<RoomScript>();

      if (roomScript.roomType == RoomType.boss)
      {
        network.bossRoom = network.roomNodes[room.GetInstanceID()];
      }

      manager.GetRoomNetwork.ConnectRooms(
        gameObject.GetInstanceID(),
        room.GetInstanceID()
      );

      roomScript.GenerateRooms();
    }
  }

  List<GameObject> GetAttachments()
  {
    var attachments = new List<GameObject>();

    var childCount = transform.childCount;

    for (int i = 0; i < childCount; i++)
    {
      var attachment = transform.GetChild(i).gameObject;

      if (attachment.name == "Attachment")
      {
        attachments.Add(attachment);
      }
    }

    if (isRoot)
    {
      attachments.Add(transform.Find("RootAttachment").gameObject);
    }

    return attachments;
  }

  bool IsRoomPlacementAvailable(Vector3 checkArea, Transform origin)
  {
    return !Physics.CheckBox(
      origin.position + origin.forward * checkArea.z / 2f,
      checkArea / 2.01f
    );
  }

  void Start()
  {
    if (isRoot)
    {
      var manager = GameManagerScript.instance;

      manager.GetRoomNetwork.AddRoom(gameObject, true);

      GenerateRooms();

      // TODO: Melhorar workaround pra geração do navmesh funcionar mesmo com a lógica de remover paredes
      StartCoroutine(BakeNavMesh());

      manager.GetPlayer?.GetComponent<PlayerScript>().CalculatePath(true);
    }
  }
}
