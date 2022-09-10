using System.Collections.Generic;
using UnityEngine;
using Unity.AI.Navigation;

#nullable enable

public class GameManagerScript : MonoBehaviour
{
  [SerializeField]
  GameObject? enemyPrefab;
  [SerializeField]
  List<GameObject>? roomPrefabs;
  [SerializeField]
  GameObject? corridorPrefab;
  [SerializeField]
  GameObject? deadEndPrefab;

  [SerializeField]
  int minRoomCount;

  List<GameObject> enemies = new List<GameObject>();
  List<GameObject> rooms = new List<GameObject>();
  GameObject? gamepadGroup;
  GameObject? menuPanel;

  MenuState menuState = MenuState.closed;

  public static GameManagerScript instance = default!;
  public List<GameObject> GetRoomPrefabs =>
    roomPrefabs ?? new List<GameObject> { };
  public GameObject GetCorridorPrefab => corridorPrefab ?? default!;
  public GameObject GetDeadEndPrefab => deadEndPrefab ?? default!;
  public List<GameObject> GetRooms => rooms;
  public int GetMinRoomCount => minRoomCount;
  public MenuState GetMenuState => menuState;

  public void BakeNavMesh()
  {
    GetComponent<NavMeshSurface>().BuildNavMesh();
  }

  public void DisableGamepad()
  {
    if (gamepadGroup == null)
    {
      return;
    }

    gamepadGroup.SetActive(false);

    LocalPrefs.SetGamepadEnabled(false);
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

    LocalPrefs.SetGamepadEnabled(true);
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

  public void KillEnemy(GameObject enemy)
  {
    enemies.Remove(enemy);

    Destroy(enemy);
  }

  System.Collections.IEnumerator SpawnLoop(GameObject prefab)
  {
    while (true)
    {
      for (int i = 0; i < 1; i++)
      {
        var vector2 = Random.insideUnitCircle * 5;

        var enemy = Instantiate(
          prefab,
          new Vector3(x: vector2.x, y: 2f, z: vector2.y),
          Quaternion.identity
        );

        enemies.Add(enemy);
      }

      yield return new WaitForSeconds(10);
    }
  }

  void Awake()
  {
    instance = this;

    var currentFrameRate = Application.targetFrameRate;

    if (currentFrameRate < 60)
    {
      Application.targetFrameRate = 60;
    }

    gamepadGroup = GameObject.Find("Canvas/GamepadGroup");
    menuPanel = GameObject.Find("Canvas/MenuPanel");
  }

  void OnGUI()
  {
    GUI.Label(new Rect(100, 36, 100, 20), $"Enemies alive: {enemies.Count}");
  }

  void Start()
  {
    if (enemyPrefab != null)
    {
      StartCoroutine(SpawnLoop(prefab: enemyPrefab));
    }
  }
}
