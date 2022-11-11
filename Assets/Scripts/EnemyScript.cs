using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

#nullable enable

public class EnemyScript : MonoBehaviour
{
  [Min(1f)]
  public float initialHitPoints = 100f;
  [Min(.1f)]
  public float maxRange = 5f;
  [Min(0f)]
  public float minRange = 5f;

  public EnemyType enemyType = EnemyType.melee;

  public GameObject? deathEffectPrefab;

  public bool isBoss = false;

  NavMeshAgent? agent;
  GameObject? player;
  MeleeAttackScript? meleeAttackScript;
  RangedAttackScript? rangedAttackScript;

  float hitPoints = 0f;

  public float GetCurrentHitPoints => hitPoints;

  void Die()
  {
    DropItems();

    GameManagerScript.instance?.KillEnemy(gameObject);

    if (isBoss)
    {
      GameManagerScript.instance?.ClearGame();
    }
  }

  void DropItems()
  {
    var dropPrefabs =
      GameManagerScript.instance?.GetEnemyDropSettings.RandomizeDropPrefabs() ??
      new List<GameObject>();

    foreach (var item in dropPrefabs)
    {
      Instantiate(
        item,
        new Vector3
        {
          x = transform.position.x,
          z = transform.position.z,
        },
        Quaternion.identity
      );
    }
  }

  void HandleAttack(GameObject player)
  {
    var diff = player.transform.position - transform.position;

    transform.rotation = Quaternion.Euler(new Vector3
    {
      y = Mathf.Atan2(diff.x, diff.z) * Mathf.Rad2Deg
    });

    if (diff.magnitude <= maxRange && diff.magnitude >= minRange)
    {
      if (enemyType == EnemyType.melee)
      {
        MeleeAttack();
      }
      else
      {
        RangedAttack();
      }
    }
  }

  void HandleMovement(NavMeshAgent agent, GameObject player)
  {
#if UNITY_EDITOR
    Debug.DrawLine(transform.position, player.transform.position);
#endif

    if (GameManagerScript.instance.isNavMeshBaked)
    {
      agent.destination = player.transform.position;
    }
  }

  public bool InflictDamage(float damage)
  {
    var newHitPoints = hitPoints - damage;

    // Spawna particula de sangue no inimigo.
    if (deathEffectPrefab != null)
    {
      Instantiate(deathEffectPrefab, transform.position, transform.rotation);
    }

    if (newHitPoints <= 0f)
    {
      Die();

      return true;
    }
    else
    {
      hitPoints = newHitPoints;
    }

    return false;
  }

  void MeleeAttack()
  {
    meleeAttackScript?.Attack();
  }

  void RangedAttack()
  {
    rangedAttackScript?.Attack();
  }

  System.Collections.IEnumerator UpdatePlayer()
  {
    while (true)
    {
      player = GameManagerScript.instance.GetPlayer;

      yield return new WaitForSeconds(1f);
    }
  }

  void Awake()
  {
    hitPoints = initialHitPoints;

    agent = GetComponent<NavMeshAgent>();

    meleeAttackScript = GetComponent<MeleeAttackScript>();
    rangedAttackScript = GetComponent<RangedAttackScript>();
  }

  void Start()
  {
    StartCoroutine(UpdatePlayer());
  }

  void FixedUpdate()
  {
    if (agent != null && player != null)
    {
      if (enemyType == EnemyType.melee)
      {
        HandleMovement(agent: agent, player: player);
      }

      HandleAttack(player);
    }
  }
}
