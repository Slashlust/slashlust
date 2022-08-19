using System.Collections.Generic;
using UnityEngine;

#nullable enable

public class GameManagerScript : MonoBehaviour
{
  [SerializeField]
  GameObject? enemyPrefab;

  List<GameObject> enemies = new List<GameObject>();

  public static GameManagerScript instance = new GameManagerScript();

  public void KillEnemy(GameObject enemy)
  {
    enemies.Remove(enemy);

    Destroy(enemy);
  }

  System.Collections.IEnumerator SpawnLoop(GameObject prefab)
  {
    while (true)
    {
      for (int i = 0; i < 5; i++)
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
