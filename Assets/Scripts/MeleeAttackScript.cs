using UnityEngine;

#nullable enable

public class MeleeAttackScript : MonoBehaviour
{
  [SerializeField]
  [Min(.05f)]
  float secondsBeteweenAttacks = 1f;

  bool isOnAttackCooldown = false;

  public void Attack()
  {
    if (isOnAttackCooldown)
    {
      return;
    }

    // TODO: Implementar mudança de parâmetros de animação

    StartCoroutine(AttackRoutine());
  }

  System.Collections.IEnumerator AttackRoutine()
  {
    isOnAttackCooldown = true;

    GameManagerScript.instance.GetPlayer?
      .GetComponent<PlayerScript>().TakeDamage(5f);

    yield return new WaitForSeconds(secondsBeteweenAttacks);

    isOnAttackCooldown = false;
  }
}
