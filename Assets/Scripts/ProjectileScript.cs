using UnityEngine;

#nullable enable

public class ProjectileScript : MonoBehaviour
{
  void Despawn()
  {
    gameObject.SetActive(false);

    Destroy(gameObject);
  }

  void OnCollisionEnter(Collision collision)
  {
    var collider = collision.collider;

    // TODO: Fazer lógica de refletir o projétil
    // TODO: Otimizar o projétil pra mobile

    if (collider.gameObject.layer == Layers.geometryLayer)
    {
      Despawn();
    }
    else if (collider.name == "Player")
    {
      Despawn();

      GameManagerScript.instance.GetPlayerScript?.TakeDamage(10f);
    }
  }
}
