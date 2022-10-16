using UnityEngine;
using UnityEngine.UI;

#nullable enable

public class MenuScript : MonoBehaviour
{
  Toggle? gamepadDisabledToggle;
  Slider? sFXVolumeSlider;

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

  void SetSFXVolume(float value, Slider sFXVolumeSlider)
  {
    sFXVolumeSlider.value = value;
  }

  void Awake()
  {
    gamepadDisabledToggle =
      transform.Find(
        "MenuPanel/Body/ScrollArea/Column/GamepadEnabled/Toggle"
      ).GetComponent<Toggle>();
    sFXVolumeSlider =
      transform.Find(
        "MenuPanel/Body/ScrollArea/Column/SFXVolume/Slider"
      ).GetComponent<Slider>();
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

    var sFXVolume = LocalPrefs.GetSFXVolume();

    if (sFXVolumeSlider != null)
    {
      SetSFXVolume(sFXVolume, sFXVolumeSlider);

      sFXVolumeSlider.onValueChanged.AddListener((value) =>
      {
        SetSFXVolume(value, sFXVolumeSlider);

        LocalPrefs.SetSFXVolume(value);
      });
    }
  }
}
