using System.Collections;
using UnityEngine;

public class FpsCounterScript : MonoBehaviour
{
  float fps = 0f;
  float ups = 0f;
  float f = 0f;
  float u = 0f;

  IEnumerator ResetLoop()
  {
    while (true)
    {
      fps = f;
      ups = u;
      f = 0f;
      u = 0f;
      yield return new WaitForSecondsRealtime(1f);
    }
  }

  void FixedUpdate()
  {
    u++;
  }

  void OnGUI()
  {
    GUI.Label(new Rect(16, 16, 100, 20), $"FPS {fps}");
    GUI.Label(new Rect(16, 36, 100, 20), $"UPS {ups}");
  }

  void Start()
  {
    StartCoroutine(ResetLoop());
  }

  void Update()
  {
    f++;
  }
}
