using System.Collections.Generic;
using UnityEngine;

#nullable enable

public class RoomScript : MonoBehaviour
{
  [SerializeField]
  bool isRoot = false;
  [SerializeField]
  Vector3 dimensions;

  public float difficultyIndex = 0f;
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
            // Chance de conectar 2 salas com um corredor.
            if (
              Random.value <=
              manager.GetMapGenerationSettings.roomConnectionChance
            )
            {
              var deadEnd = hit.collider.transform.parent.gameObject;

              var deadEndRoom = deadEnd.GetComponent<DeadEndScript>().room;

              // Removendo a layer de geometry do dead end para evitar que ele
              // impacte nos raycasts ou geração do navmesh
              deadEnd.layer = Layers.defaultLayer;

              Destroy(deadEnd);

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
        }

        if (Random.value > 0.2f)
        {
          GenerateDeadEnd(attachment: attachment);

          continue;
        }
      }

      var blockBossRoomSpawn = false;

      if (network.hasBossRoomSpawned)
      {
        blockBossRoomSpawn = true;
      }
      else if (network.roomNodes.Count <= 4)
      {
        blockBossRoomSpawn = true;
      }

      var spawnChance =
        network.roomNodes.Count /
        (manager.GetMapGenerationSettings.minRoomCount - 2f);

      var shouldBossRoomSpawn = blockBossRoomSpawn
        ? false
        : Random.value < spawnChance;

      var roomPrefab = shouldBossRoomSpawn
        ? manager.GetMapGenerationSettings.bossRoomPrefab!
        : manager.GetMapGenerationSettings.GetRandomRoomPrefab();

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
        network.hasBossRoomSpawned = true;

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

  public void SpawnEnemies()
  {
    if (GameManagerScript.instance.isGameBeaten)
    {
      return;
    }

    var manager = GameManagerScript.instance;
    var settings = manager.GetEnemySpawnSettings;

    var batchSize =
      settings.spawnBatchSize *
      settings.difficultyIndexWeight *
      settings.difficultyCurve.Evaluate(difficultyIndex / 20f);

    Debug.Log(
      $"Current room difficulty: {difficultyIndex}: {batchSize} enemies."
    );

    if (roomType == RoomType.boss)
    {
      var bossPrefab = settings.bossPrefab;

      SpawnEnemy(prefab: bossPrefab, manager: manager);
    }

    for (int i = 0; i < batchSize; i++)
    {
      var prefab = settings.GetRandomEnemyPrefab();

      SpawnEnemy(prefab: prefab, manager: manager);
    }
  }

  void SpawnEnemy(GameObject prefab, GameManagerScript manager)
  {
    var tries = 0;

    while (true)
    {
      tries++;

      // Sistema de segurança para não cair em loop.
      if (tries > 10)
      {
        break;
      }

      var vector2 = Random.insideUnitCircle * 10f;

      var position = new Vector3(
        x: vector2.x + transform.position.x,
        y: 4f,
        z: vector2.y + transform.position.z
      );

      RaycastHit hit;
      if (
        !Physics.Raycast(
          position,
          Vector3.down,
          out hit,
          10f,
          Layers.geometryMask
        )
      )
      {
        continue;
      }

      // Não deixar o inimigo spawnar em cima de paredes.
      if (hit.point.y > 1f)
      {
        continue;
      }

      var enemy = Instantiate(
        prefab,
        hit.point,
        Quaternion.identity
      );

      manager.GetEnemies.Add(enemy);

      break;
    }
  }

  public void UpdateDifficulty()
  {
    StatsScript.instance.UpdateDifficulty(difficultyIndex);
  }

  void Awake()
  {
    var magnitude = transform.position.magnitude;

    var newDifficultyIndex = roomType == RoomType.boss
      ? magnitude * 2f + 200f
      : magnitude;

    difficultyIndex = newDifficultyIndex;
  }

  void Start()
  {
    if (isRoot)
    {
      var manager = GameManagerScript.instance;

      manager.GetRoomNetwork.AddRoom(gameObject, true);

      GenerateRooms();

      // Workaround pra geração do navmesh funcionar mesmo com a lógica de
      // remover paredes
      StartCoroutine(BakeNavMesh());

      manager.GetPlayerScript?.CalculatePath(true);
    }
  }
}
