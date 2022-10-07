using UnityEngine;
using UnityEngine.AI;

#nullable enable

public class EnemyScript : MonoBehaviour
{
  NavMeshAgent? agent;
  GameObject? player;

  [HideInInspector]
  public readonly float initialHitPoints = 100f;
  float hitPoints = 100f;

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
