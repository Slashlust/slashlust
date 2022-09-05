using System.Collections.Generic;
using UnityEngine;

public class RoomScript : MonoBehaviour
{
  [SerializeField]
  bool isRoot = false;
  [SerializeField]
  Vector3 dimensions;

  void GenerateRooms()
  {
    var attachments = GetAttachments();

    var roomPrefabs = GameManagerScript.instance.GetRoomPrefabs;

    foreach (var attachment in attachments)
    {
      var roomPrefab = roomPrefabs[0];

      var attachmentRoomScript = roomPrefab.GetComponent<RoomScript>();

      var room = Instantiate(
        roomPrefab,
        attachment.transform.position +
          attachment.transform.forward * attachmentRoomScript.dimensions.z / 2,
        attachment.transform.rotation
      );
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

  void Start()
  {
    if (isRoot)
    {
      GenerateRooms();
    }
  }
}
