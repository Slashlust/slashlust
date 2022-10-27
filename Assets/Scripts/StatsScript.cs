using UnityEngine;
using UnityEngine.UI;

#nullable enable

public class StatsScript : MonoBehaviour
{
  Text? damageText;
  Text? movementSpeedText;
  Text? attackRangeText;

  public static StatsScript instance = default!;

  public void UpdateStats(PlayerBuffs buffs)
  {
    if (damageText != null)
    {
      damageText.text =
        (buffs.baseDamageBuff * buffs.damageMultiplierBuff)
        .ToString("0.00");
    }

    if (movementSpeedText != null)
    {
      movementSpeedText.text =
        (buffs.baseMovementSpeedBuff * buffs.movementSpeedMultiplierBuff)
        .ToString("0.00");
    }

    if (attackRangeText != null)
    {
      attackRangeText.text =
        (buffs.baseAttackRangeBuff * buffs.attackRangeMultiplierBuff)
        .ToString("0.00");
    }
  }

  void Awake()
  {
    instance = this;

    damageText = transform.Find("Damage").GetComponent<Text>();
    movementSpeedText = transform.Find("MovementSpeed").GetComponent<Text>();
    attackRangeText = transform.Find("AttackRange").GetComponent<Text>();
  }
}
