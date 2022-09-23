using UnityEngine;

#nullable enable

public class ProjectileScript : MonoBehaviour
{
  void Despawn()
  {
    Destroy(gameObject);
  }

  void OnCollisionEnter(Collision collision)
  {
    var collider = collision.collider;

    // TODO: Fazer lógica de refletir o projétil

    if (collider.gameObject.layer == Layers.geometryLayer)
    {
      Despawn();
    }
    else if (collider.name == "Player")
    {
      var playerScript = GameManagerScript.instance.GetPlayer?
        .GetComponent<PlayerScript>();

      playerScript?.TakeDamage(10f);

      Despawn();
    }
  }
}
