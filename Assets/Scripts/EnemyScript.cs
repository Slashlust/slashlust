using UnityEngine;
using UnityEngine.AI;

#nullable enable

public class EnemyScript : MonoBehaviour
{
  NavMeshAgent? agent;
  GameObject? player;

  float hitPoints = 100;

  void Die()
  {
    GameManagerScript.instance?.KillEnemy(gameObject);
  }

  void HandleMovement(NavMeshAgent agent, GameObject player)
  {
    Debug.DrawLine(transform.position, player.transform.position);

    agent.destination = player.transform.position;
  }

  public bool InflictDamage(float damage)
  {
    var newHitPoints = hitPoints - damage;

    if (hitPoints <= 0)
    {
      return false;
    }

    if (newHitPoints <= 0)
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

    player = GameObject.Find("Player");
  }

  void FixedUpdate()
  {
    if (agent != null && player != null)
    {
      HandleMovement(agent: agent, player: player);
    }
  }
}
