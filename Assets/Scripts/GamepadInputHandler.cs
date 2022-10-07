using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.OnScreen;

#nullable enable

public class GamepadInputHandler : OnScreenControl, IPointerDownHandler, IPointerUpHandler
{
  System.Collections.IEnumerator InputDelay()
  {
    yield return new WaitForEndOfFrame();

    ResetInput();
  }

  void ResetInput()
  {
    SendValueToControl(Vector2.zero);

    var playerScript = GameManagerScript.instance.GetPlayer?
      .GetComponent<PlayerScript>();

    playerScript?.Move();
  }

  public void OnPointerUp(PointerEventData data)
  {
    ResetInput();

    StartCoroutine(InputDelay());
  }

  public void OnPointerDown(PointerEventData data)
  {
  }

  [InputControl(layout = "Vector2")]
  [SerializeField]
  private string m_ControlPath = default!;

  protected override string controlPathInternal
  {
    get => m_ControlPath;
    set => m_ControlPath = value;
  }
}
