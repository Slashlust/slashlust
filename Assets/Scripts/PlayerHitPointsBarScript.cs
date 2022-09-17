using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
#nullable enable
    public class PlayerHitPointsBarScript : MonoBehaviour
    {
        [SerializeField]
        RectTransform? fillTransform;
        [SerializeField]
        Text? label;

        PlayerScript? playerScript;

        void UpdateFillTransform(
          RectTransform fillTransform,
          PlayerScript playerScript,
          Text label
        )
        {
            var hitPoints = playerScript.GetCurrentHitPoints();

            label.text = hitPoints.ToString("0");

            fillTransform.localScale = new Vector3
            {
                x = hitPoints / playerScript.initialHitPoints,
                y = 1,
                z = 1
            };
        }

        void Start()
        {
            playerScript = transform.parent.GetComponent<PlayerScript>();
        }

        void Update()
        {
            transform.LookAt(Camera.main.transform.position);

            if (fillTransform != null && playerScript != null && label != null)
            {
                UpdateFillTransform(
                  fillTransform: fillTransform,
                  playerScript: playerScript,
                  label: label
                );
            }
        }
    }
}
