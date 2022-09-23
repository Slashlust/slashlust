using UnityEngine;
using UnityEngine.UI;

#nullable enable

public class HitPointsBarScript : MonoBehaviour
{
  [SerializeField]
  RectTransform? fillTransform;
  [SerializeField]
  Text? label;

  // TODO: Melhorar após implementar classe genérica para o inimigo
  EnemyScript? enemyScript;
  EnemyRangedScript? enemyRangedScript;

  void UpdateFillTransform(
    RectTransform fillTransform,
    Text label
  )
  {
    var hitPoints = 0f;
    var initialHitPoints = 0f;

    if (enemyScript != null)
    {
      hitPoints = enemyScript.GetCurrentHitPoints();
      initialHitPoints = enemyScript.initialHitPoints;
    }
    else if (enemyRangedScript != null)
    {
      hitPoints = enemyRangedScript.GetCurrentHitPoints();
      initialHitPoints = enemyRangedScript.initialHitPoints;
    }

    label.text = hitPoints.ToString("0");

    fillTransform.localScale = new Vector3
    {
      x = hitPoints / initialHitPoints,
      y = 1,
      z = 1
    };
  }

  void Awake()
  {
    enemyScript = transform.parent.GetComponent<EnemyScript>();
    enemyRangedScript = transform.parent.GetComponent<EnemyRangedScript>();
  }

  void Update()
  {
    transform.LookAt(Camera.main.transform.position);

    if (fillTransform != null && label != null)
    {
      UpdateFillTransform(
        fillTransform: fillTransform,
        label: label
      );
    }
  }
}
