using UnityEngine;
using UnityEngine.AI;

#nullable enable

public class EnemyScript : MonoBehaviour
{
  NavMeshAgent? agent;

  void HandleMovement(NavMeshAgent agent)
  {
    var player = GameObject.Find("Player");

    if (player == null)
    {
      return;
    }

    Debug.DrawLine(transform.position, player.transform.position);

    agent.destination = player.transform.position;
  }

  void Awake()
  {
    agent = GetComponent<NavMeshAgent>();
  }

  void Start()
  {

  }

  void FixedUpdate()
  {
    if (agent != null)
    {
      HandleMovement(agent: agent);
    }
  }
}
