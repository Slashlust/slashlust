using System.Collections.Generic;
using UnityEngine;

#nullable enable

public class RoomScript : MonoBehaviour
{
  [SerializeField]
  bool isRoot = false;
  [SerializeField]
  Vector3 dimensions;

  public Vector3 GetDimensions => dimensions;

  public void GenerateRooms()
  {
    var manager = GameManagerScript.instance;

    var attachments = GetAttachments();

    var roomPrefabs = manager.GetMapGenerationSettings.roomPrefabs;

    var corridorPrefab = manager.GetMapGenerationSettings.corridorPrefab!;

    var deadEndPrefab = manager.GetMapGenerationSettings.deadEndPrefab!;

    void GenerateDeadEnd(GameObject attachment)
    {
      Instantiate(
        deadEndPrefab,
        attachment.transform.position,
        attachment.transform.rotation
      );
    }

    foreach (var attachment in attachments)
    {
      if (
        manager.GetRooms.Count >= manager.GetMapGenerationSettings.minRoomCount
      )
      {
        // TODO: Trabalhar probabilidade de geração de cada attachment.
        if (Random.value > 0.2f)
        {
          GenerateDeadEnd(attachment: attachment);

          continue;
        }
      }

      var roomPrefab =
        GameManagerScript.instance.GetMapGenerationSettings
        .GetRandomRoomPrefab();

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

      // TODO: Usar ou remover corredor.
      var corridor = Instantiate(
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

      manager.GetRooms.Add(room);

      room.GetComponent<RoomScript>().GenerateRooms();
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
      GenerateRooms();

      GameManagerScript.instance.BakeNavMesh();
    }
  }
}
