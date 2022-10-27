using UnityEngine;

#nullable enable

public class DropScript : MonoBehaviour
{
  [SerializeField]
  PlayerBuffs playerBuffs = default!;

  void OnCollisionEnter(Collision collision)
  {
    if (collision.collider.name == "DropCollider")
    {
      var playerScript = GameManagerScript.instance?.GetPlayerScript;

      if (playerScript != null)
      {
        var buffs = playerScript.GetPlayerBuffs();

        buffs.initialHitPoints += playerBuffs.initialHitPoints;
        playerScript.Heal(playerBuffs.baseHitPoints);
        buffs.baseDamageBuff += playerBuffs.baseDamageBuff;
        buffs.damageMultiplierBuff += playerBuffs.damageMultiplierBuff;
        buffs.baseMovementSpeedBuff += playerBuffs.baseMovementSpeedBuff;
        buffs.movementSpeedMultiplierBuff += playerBuffs.movementSpeedMultiplierBuff;
        buffs.baseAttackRangeBuff += playerBuffs.baseAttackRangeBuff;
        buffs.attackRangeMultiplierBuff += playerBuffs.attackRangeMultiplierBuff;

        playerScript.TriggerUpdateStats();
      }

      Destroy(gameObject);
    }
  }
}
