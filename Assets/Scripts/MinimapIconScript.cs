using UnityEngine;
using UnityEngine.UI;

#nullable enable

public class MinimapIconScript : MonoBehaviour
{
  public void Paint(Color color)
  {
    transform.Find("Icon").GetComponent<Image>().color = color;
  }
}
