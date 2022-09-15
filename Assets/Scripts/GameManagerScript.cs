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

  List<GameObject> enemies = new List<GameObject>();
  GameObject? gamepadGroup;
  GameObject? menuPanel;
  GameObject? geometry;
  RoomNetwork roomNetwork = new RoomNetwork();

  MenuState menuState = MenuState.closed;

  public static GameManagerScript instance = default!;

  public MenuState GetMenuState => menuState;
  public GameObject GetGeometry => geometry ?? default!;
  public RoomNetwork GetRoomNetwork => roomNetwork;

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

    var currentFrameRate = Application.targetFrameRate;

    if (currentFrameRate < 60)
    {
      Application.targetFrameRate = 60;
    }

    gamepadGroup = GameObject.Find("Canvas/GamepadGroup");
    menuPanel = GameObject.Find("Canvas/MenuPanel");
    geometry = GameObject.Find("Geometry");
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
    // TODO: Retirar visualização de debug da network gerada
    roomNetwork.DebugDrawNetwork();
  }
}
