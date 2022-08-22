using UnityEngine;
using UnityEngine.UI;

#nullable enable

public class HitPointsBarScript : MonoBehaviour
{
  [SerializeField]
  RectTransform? fillTransform;
  [SerializeField]
  Text? label;

  EnemyScript? enemyScript;

  void UpdateFillTransform(
    RectTransform fillTransform,
    EnemyScript enemyScript,
    Text label
  )
  {
    var hitPoints = enemyScript.GetCurrentHitPoints();

    label.text = hitPoints.ToString("0");

    fillTransform.localScale = new Vector3
    {
      x = hitPoints / enemyScript.initialHitPoints,
      y = 1,
      z = 1
    };
  }

  void Start()
  {
    enemyScript = transform.parent.GetComponent<EnemyScript>();
  }

  void Update()
  {
    transform.LookAt(Camera.main.transform.position);

    if (fillTransform != null && enemyScript != null && label != null)
    {
      UpdateFillTransform(
        fillTransform: fillTransform,
        enemyScript: enemyScript,
        label: label
      );
    }
  }
}
