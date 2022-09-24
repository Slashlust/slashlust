using System.Collections.Generic;
using UnityEngine;
using Unity.AI.Navigation;

#nullable enable

public class GameManagerScript : MonoBehaviour
{
  [SerializeField]
  MapGenerationSettings mapGenerationSettings = default!;
  [SerializeField]
  EnemySpawnSettings enemySpawnSettings = default!;

  public bool isNavMeshBaked = false;

  // Referência.
  public GameObject? currentRoom;

  GameObject? gamepadGroup;
  GameObject? menuPanel;
  GameObject? geometry;
  GameObject? player;
  MinimapScript? minimapScript;
  RoomNetwork roomNetwork = new RoomNetwork();
  List<GameObject> enemies = new List<GameObject>();

  // State.
  MenuState menuState = MenuState.closed;
  ControlState controlState = ControlState.gamepad;

  public static GameManagerScript instance = default!;

  // Getters de state.
  public MenuState GetMenuState => menuState;
  public ControlState GetControlState => controlState;

  // Getters de tipo primitivo.
  public string GetTargetControlScheme => controlState == ControlState.keyboard
    ? ControlSchemes.keyboardMouse : ControlSchemes.gamepad;

  // Getters de referência.
  public GameObject GetGeometry => geometry ?? default!;
  public RoomNetwork GetRoomNetwork => roomNetwork;
  public GameObject? GetPlayer => player;
  public MinimapScript? GetMinimapScript => minimapScript;
  public MapGenerationSettings GetMapGenerationSettings =>
    mapGenerationSettings;
  public EnemySpawnSettings GetEnemySpawnSettings => enemySpawnSettings;

  public void BakeNavMesh()
  {
    GetComponent<NavMeshSurface>().BuildNavMesh();

    isNavMeshBaked = true;
  }

  public void DisableGamepad()
  {
    if (gamepadGroup == null)
    {
      return;
    }

    gamepadGroup.SetActive(false);

    controlState = ControlState.keyboard;

    LocalPrefs.SetGamepadDisabled(true);
  }

  public void DisableMenu()
  {
    if (menuPanel == null)
    {
      return;
    }

    menuPanel.SetActive(false);

    menuState = MenuState.closed;
  }

  public void EnableGamepad()
  {
    if (gamepadGroup == null)
    {
      return;
    }

    gamepadGroup.SetActive(true);

    controlState = ControlState.gamepad;

    LocalPrefs.SetGamepadDisabled(false);
  }

  public void EnableMenu()
  {
    if (menuPanel == null)
    {
      return;
    }

    menuPanel.SetActive(true);

    menuState = MenuState.open;
  }

  void HandleConfigInitialization()
  {
    var currentFrameRate = Application.targetFrameRate;

    if (Application.isMobilePlatform)
    {
      if (currentFrameRate < 60)
      {
        Application.targetFrameRate = 60;
      }
    }
  }

  public void KillEnemy(GameObject enemy)
  {
    enemies.Remove(enemy);

    Destroy(enemy);
  }

  System.Collections.IEnumerator SpawnLoop()
  {
    while (true)
    {
      if (enemySpawnSettings.isEnemySpawnEnabled)
      {
        for (int i = 0; i < enemySpawnSettings.spawnBatchSize; i++)
        {
          var prefab = enemySpawnSettings.GetRandomEnemyPrefab();

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

            var position = new Vector3(x: vector2.x, y: 4f, z: vector2.y);

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

            enemies.Add(enemy);

            break;
          }
        }
      }

      yield return new WaitForSeconds(enemySpawnSettings.spawnInterval);
    }
  }

  void Awake()
  {
    instance = this;

    gamepadGroup = GameObject.Find("Canvas/GamepadGroup");
    menuPanel = GameObject.Find("Canvas/MenuPanel");
    geometry = GameObject.Find("Geometry");
    player = GameObject.Find("Player");
    minimapScript = GameObject.Find("Canvas/Minimap")
      .GetComponent<MinimapScript>();

    HandleConfigInitialization();
  }

  void OnGUI()
  {
    GUI.Label(new Rect(100, 36, 100, 20), $"Enemies alive: {enemies.Count}");
  }

  void Start()
  {
    StartCoroutine(SpawnLoop());
  }

  void Update()
  {
#if UNITY_EDITOR
    roomNetwork.DebugDrawNetwork();

    roomNetwork.DebugDrawEdges();

    if (roomNetwork.targetPath != null)
    {
      roomNetwork.DebugDrawPath(roomNetwork.targetPath);
    }
#endif
  }
}
