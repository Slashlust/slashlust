using UnityEngine;
using UnityEngine.UI;

#nullable enable

public class MenuScript : MonoBehaviour
{
  Toggle? gamepadEnabledToggle;

  void SetGamepadEnabled(bool value, Toggle gamepadEnabledToggle)
  {
    gamepadEnabledToggle.isOn = value;

    if (value)
    {
      GameManagerScript.instance.EnableGamepad();
    }
    else
    {
      GameManagerScript.instance.DisableGamepad();
    }
  }

  void Awake()
  {
    gamepadEnabledToggle =
      transform.Find("MenuPanel/Body/ScrollArea/Column/GamepadEnabled/Toggle")
      .GetComponent<Toggle>();
  }

  void Start()
  {
    var gamepadEnabled = LocalPrefs.GetGamepadEnabled();

    if (gamepadEnabledToggle != null)
    {
      SetGamepadEnabled(gamepadEnabled, gamepadEnabledToggle);

      gamepadEnabledToggle.onValueChanged.AddListener((value) =>
      {
        SetGamepadEnabled(value, gamepadEnabledToggle);

        LocalPrefs.SetGamepadEnabled(value);
      });
    }
  }
}
