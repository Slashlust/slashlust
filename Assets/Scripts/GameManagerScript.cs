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
  [SerializeField]
  EnemyDropSettings enemyDropSettings = default!;
  [SerializeField]
  GameObject? floatingTextPrefab;

  public bool isNavMeshBaked = false;

  // Referência.
  public GameObject? currentRoom;

  GameObject? gamepadGroup;
  GameObject? menuPanel;
  GameObject? geometry;
  GameObject? player;
  PlayerScript? playerScript;
  MinimapScript? minimapScript;
  RoomNetwork roomNetwork = new RoomNetwork();
  List<GameObject> enemies = new List<GameObject>();
  HashSet<GameObject> visitedRooms = new HashSet<GameObject>();

  // State.
  MenuState menuState = MenuState.closed;
  ControlState controlState = ControlState.gamepad;

  public static GameManagerScript instance = default!;

  // Getters de state.
  public MenuState GetMenuState => menuState;
  public ControlState GetControlState => controlState;
  public List<GameObject> GetEnemies => enemies;

  // Getters de tipo primitivo.
  public string GetTargetControlScheme => controlState == ControlState.keyboard
    ? ControlSchemes.keyboardMouse : ControlSchemes.gamepad;

  // Getters de referência.
  public GameObject GetGeometry => geometry ?? default!;
  public RoomNetwork GetRoomNetwork => roomNetwork;
  public GameObject? GetPlayer => player;
  public PlayerScript? GetPlayerScript => playerScript;
  public MinimapScript? GetMinimapScript => minimapScript;
  public MapGenerationSettings GetMapGenerationSettings =>
    mapGenerationSettings;
  public EnemySpawnSettings GetEnemySpawnSettings => enemySpawnSettings;
  public EnemyDropSettings GetEnemyDropSettings => enemyDropSettings;

  public void AttemptEnemySpawn()
  {
    currentRoom?.GetComponent<RoomScript>()?.UpdateDifficulty();

    if (currentRoom == null || visitedRooms.Contains(currentRoom))
    {
      return;
    }

    visitedRooms.Add(currentRoom);

    if (enemySpawnSettings.isEnemySpawnEnabled)
    {
      currentRoom.GetComponent<RoomScript>()?.SpawnEnemies();
    }
  }

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

  public void SpawnFloatingText(Vector3 position, string text, Color? color)
  {
    if (floatingTextPrefab != null)
    {
      var script = Instantiate(floatingTextPrefab, position + Vector3.up, Quaternion.identity)
        .GetComponent<FloatingTextScript>();

      script.UpdateText(text);

      if (color != null)
      {
        script.UpdateColor((Color)color!);
      }
    }
  }

  void Awake()
  {
    instance = this;

    gamepadGroup = GameObject.Find("Canvas/GamepadGroup");
    menuPanel = GameObject.Find("Canvas/MenuPanel");
    geometry = GameObject.Find("Geometry");
    player = GameObject.Find("Player");
    playerScript = player.GetComponent<PlayerScript>();
    minimapScript = GameObject.Find("Canvas/Minimap")
      .GetComponent<MinimapScript>();

    HandleConfigInitialization();
  }

  void OnGUI()
  {
    GUI.Label(new Rect(100, 36, 100, 20), $"Enemies alive: {enemies.Count}");
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
