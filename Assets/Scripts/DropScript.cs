using System.Collections.Generic;
using UnityEngine;

#nullable enable

public enum BuffType
{
  attackRange,
  damage,
  hitpoints,
  movementSpeed,
}

public class DropScript : MonoBehaviour
{
  [SerializeField]
  PlayerBuffs playerBuffs = default!;
  [SerializeField]
  List<BuffType> buffTypes = new List<BuffType>();

  void OnCollisionEnter(Collision collision)
  {
    if (collision.collider.name == "DropCollider")
    {
      var manager = GameManagerScript.instance;

      var playerScript = manager?.GetPlayerScript;

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

        int index = 0;
        foreach (var item in buffTypes)
        {
          var text = "";
          var color = Color.white;

          switch (item)
          {
            case BuffType.attackRange:
              text = "+ Alcance";
              color = Colors.spanishGreen;
              break;
            case BuffType.damage:
              text = "+ Dano";
              color = Colors.orangePantone;
              break;
            case BuffType.hitpoints:
              text = "+ Vida";
              color = Colors.roseMadder;
              break;
            case BuffType.movementSpeed:
              text = "+ Velocidade";
              color = Colors.blueCrayola;
              break;
          }

          manager?.SpawnFloatingText(
            transform.position + Vector3.up * index * .3f,
            text,
            color
          );

          SoundManagerScript.instance.PlayGulp();

          index++;
        }
      }

      Destroy(gameObject);
    }
  }
}
