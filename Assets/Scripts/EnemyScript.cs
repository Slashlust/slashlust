using UnityEngine;
using UnityEngine.AI;

#nullable enable

// TODO: Adicionar efeito de fumação na morte do inimigo

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

  NavMeshAgent? agent;
  GameObject? player;
  MeleeAttackScript? meleeAttackScript;
  RangedAttackScript? rangedAttackScript;

  float hitPoints = 0f;

  public float GetCurrentHitPoints => hitPoints;

  void Die()
  {
    GameManagerScript.instance?.KillEnemy(gameObject);
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

    // TODO: Implementar lógica de movimentação

    if (GameManagerScript.instance.isNavMeshBaked)
    {
      agent.destination = player.transform.position;
    }
  }

  public bool InflictDamage(float damage)
  {
    var newHitPoints = hitPoints - damage;

    //Spawna particula de sangue no inimigo
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

  void Awake()
  {
    hitPoints = initialHitPoints;

    agent = GetComponent<NavMeshAgent>();

    meleeAttackScript = GetComponent<MeleeAttackScript>();
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

      HandleAttack(player);
    }
  }
}
