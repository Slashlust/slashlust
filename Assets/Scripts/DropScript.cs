using UnityEngine;

#nullable enable

public class DropScript : MonoBehaviour
{
  [SerializeField]
  [Min(0)]
  float healHitpoints = 20f;

  void OnCollisionEnter(Collision collision)
  {
    if (collision.collider.name == "DropCollider")
    {
      GameManagerScript.instance?.GetPlayerScript?.Heal(healHitpoints);

      Destroy(gameObject);
    }
  }
}
