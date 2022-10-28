using UnityEngine;
using UnityEngine.UI;

#nullable enable

public class FloatingTextScript : MonoBehaviour
{
  [SerializeField]
  Text? text;

  public void UpdateColor(Color color)
  {
    if (text != null)
    {
      text.color = color;
    }
  }

  public void UpdateText(string newText)
  {
    if (text != null)
    {
      text.text = newText;
    }
  }

  void LateUpdate()
  {
    transform.LookAt(Camera.main.transform);
  }

  void Start()
  {
    Destroy(gameObject, 1.1f);
  }
}
