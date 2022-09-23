using UnityEngine;

#nullable enable

/// Thanks for downloading my projectile gun script! :D
/// Feel free to use it in any project you like!
/// 
/// The code is fully commented but if you still have any questions
/// don't hesitate to write a yt comment
/// or use the #coding-problems channel of my discord server
/// 
/// Dave
public class RangedAttackScript : MonoBehaviour
{
  [SerializeField]
  GameObject? projectilePrefab;

  [SerializeField]
  Transform? attackPoint;

  [SerializeField]
  [Min(.05f)]
  float secondsBeteweenShots = 1f;

  [SerializeField]
  [Min(.05f)]
  float projectileForce = 10f;

  bool isOnAttackCooldown = false;

  public void Attack()
  {
    if (isOnAttackCooldown)
    {
      return;
    }

    if (attackPoint != null)
    {
      StartCoroutine(AttackRoutine(attackPoint));
    }
  }

  System.Collections.IEnumerator AttackRoutine(Transform attackPoint)
  {
    isOnAttackCooldown = true;

    var currentBullet = Instantiate(
      projectilePrefab,
      attackPoint.position,
      attackPoint.rotation
    );

    currentBullet?.GetComponent<Rigidbody>()
      .AddForce(projectileForce * transform.forward, ForceMode.Impulse);

    yield return new WaitForSeconds(secondsBeteweenShots);

    isOnAttackCooldown = false;
  }
}
