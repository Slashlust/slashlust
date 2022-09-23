using UnityEngine;
using UnityEngine.AI;

#nullable enable

public class EnemyScript : MonoBehaviour
{
  [Min(10f)]
  public readonly float initialHitPoints = 100f;

  [Min(1f)]
  public float maxRange = 5f;

  [Min(1f)]
  public float minRange = 5f;

  public EnemyType enemyType = EnemyType.melee;

  NavMeshAgent? agent;
  GameObject? player;
  RangedAttackScript? rangedAttackScript;

  float hitPoints = 0f;

  public float GetCurrentHitPoints => hitPoints;

  void Die()
  {
    GameManagerScript.instance?.KillEnemy(gameObject);
  }

  void HandleAttack(GameObject player)
  {
    if (enemyType == EnemyType.melee)
    {
      MeleeAttack();
    }
    else
    {
      var diff = player.transform.position - transform.position;

      transform.rotation = Quaternion.Euler(new Vector3
      {
        y = Mathf.Atan2(diff.x, diff.z) * Mathf.Rad2Deg
      });

      if (diff.magnitude <= maxRange && diff.magnitude >= minRange)
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

    if (hitPoints <= 0f)
    {
      return false;
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
    // TODO: Implementar ataque melee
  }

  void RangedAttack()
  {
    rangedAttackScript?.Attack();
  }

  void Awake()
  {
    hitPoints = initialHitPoints;

    agent = GetComponent<NavMeshAgent>();

    rangedAttackScript = GetComponent<RangedAttackScript>();

    player = GameObject.Find("Player");
  }

  void FixedUpdate()
  {
    if (agent != null && player != null)
    {
      if (enemyType == EnemyType.melee)
      {
        HandleMovement(agent: agent, player: player);
      }
      else
      {
        HandleAttack(player);
      }
    }
  }
}
