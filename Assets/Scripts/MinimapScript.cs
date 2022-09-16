using UnityEngine;

#nullable enable

public class MinimapScript : MonoBehaviour
{
  [SerializeField]
  [Range(.1f, 10f)]
  float minimapScale = 1f;

  RectTransform? contentTransform;

  void HandleContentTransform(RectTransform contentTransform)
  {
    var player = GameManagerScript.instance.GetPlayer;

    if (player == null)
    {
      return;
    }

    var position = player.transform.position;

    var offset = new Vector2 { x = position.x, y = position.z } / -minimapScale;

    contentTransform.localPosition = offset;
  }

  public void Layout()
  {
    // TODO: Fazer o layout do minimapa usando a room network
    // TODO: Adicionar referência do minimap script no game manager script
  }

  void Awake()
  {
    contentTransform = transform.Find("Background/Content")
      .GetComponent<RectTransform>();
  }

  void Update()
  {
    Debug.Log(contentTransform);

    if (contentTransform != null)
    {
      HandleContentTransform(contentTransform);
    }
  }
}
