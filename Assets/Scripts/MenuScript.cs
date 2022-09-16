using UnityEngine;
using UnityEngine.UI;

#nullable enable

public class MenuScript : MonoBehaviour
{
  Toggle? gamepadDisabledToggle;

  void SetGamepadDisabled(bool value, Toggle gamepadDisabledToggle)
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
    var gamepadDisabled = LocalPrefs.GetGamepadDisabled();

    if (gamepadDisabledToggle != null)
    {
      SetGamepadDisabled(gamepadDisabled, gamepadDisabledToggle);

      gamepadDisabledToggle.onValueChanged.AddListener((value) =>
      {
        SetGamepadDisabled(value, gamepadDisabledToggle);

        LocalPrefs.SetGamepadDisabled(value);
      });
    }
  }
}
