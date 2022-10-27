#nullable enable

[System.Serializable]
public class PlayerBuffs
{
  // Initial hit points
  public float initialHitPoints = 100f;

  // Hit points
  public float baseHitPoints = 10f;

  // Damage
  public float baseDamageBuff = 20f;
  public float damageMultiplierBuff = 1f;

  // Movement speed
  public float baseMovementSpeedBuff = 4f;
  public float movementSpeedMultiplierBuff = 1f;

  // Attack range
  public float baseAttackRangeBuff = 1f;
  public float attackRangeMultiplierBuff = 1f;
}
