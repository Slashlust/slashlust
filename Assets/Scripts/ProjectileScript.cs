using UnityEngine;

#nullable enable

public class ProjectileScript : MonoBehaviour
{
  [SerializeField]
  [Min(0f)]
  float damage = 10f;

  void Despawn()
  {
    gameObject.SetActive(false);

    Destroy(gameObject);
  }

  void OnCollisionEnter(Collision collision)
  {
    var collider = collision.collider;

    if (collider.gameObject.layer == Layers.geometryLayer)
    {
      Despawn();
    }
    else if (collider.name == "Player")
    {
      Despawn();

      GameManagerScript.instance.GetPlayerScript?.TakeDamage(damage);
    }
  }
}
