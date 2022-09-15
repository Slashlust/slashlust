using UnityEngine;
using UnityEngine.AI;

#nullable enable

// TODO: Fazer classe genérica para os inimigos:
// MeleeEnemyScript, RangedEnemyScript : EnemyScript

public class EnemyRangedScript : MonoBehaviour
{
  NavMeshAgent? agent;
  GameObject? player;

  CajadoScript? cajado;

  [HideInInspector]
  public readonly float initialHitPoints = 100f;
  float hitPoints = 100f;

  float range = 10f;

  void Attack()
  {
    cajado?.Atirar();
  }
  void Die()
  {
    GameManagerScript.instance?.KillEnemy(gameObject);
  }

  public float GetCurrentHitPoints()
  {
    return hitPoints;
  }

  void HandleMovement(NavMeshAgent agent, GameObject player)
  {
    Debug.DrawLine(transform.position, player.transform.position);

    agent.destination = player.transform.position;
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

  void Awake()
  {
    agent = GetComponent<NavMeshAgent>();

    cajado = GetComponent<CajadoScript>();

    player = GameObject.Find("Player");
  }

  void FixedUpdate()
  {
    if (/* agent != null && */ player != null)
    {
      // HandleMovement(agent: agent, player: player);

      var a = player.transform.position - transform.position;

      transform.rotation = Quaternion.Euler(new Vector3 { y = Mathf.Atan2(a.x, a.z) * Mathf.Rad2Deg });
      if (a.magnitude <= range)
      {
        Attack();
      }
    }


  }



}
