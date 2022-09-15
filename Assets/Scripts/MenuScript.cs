using UnityEngine;
using UnityEngine.UI;

#nullable enable

public class MenuScript : MonoBehaviour
{
  Toggle? gamepadDisabledToggle;

  void SetGamepadEnabled(bool value, Toggle gamepadDisabledToggle)
  {
    gamepadDisabledToggle.isOn = value;

    if (value)
    {
      GameManagerScript.instance.DisableGamepad();
    }
    else
    {
      GameManagerScript.instance.EnableGamepad();
    }
  }

  void Awake()
  {
    gamepadDisabledToggle =
      transform.Find("MenuPanel/Body/ScrollArea/Column/GamepadEnabled/Toggle")
      .GetComponent<Toggle>();
  }

  void Start()
  {
    var gamepadEnabled = LocalPrefs.GetGamepadEnabled();

    if (gamepadDisabledToggle != null)
    {
      SetGamepadEnabled(gamepadEnabled, gamepadDisabledToggle);

      gamepadDisabledToggle.onValueChanged.AddListener((value) =>
      {
        SetGamepadEnabled(value, gamepadDisabledToggle);

        LocalPrefs.SetGamepadEnabled(value);
      });
    }
  }
}
