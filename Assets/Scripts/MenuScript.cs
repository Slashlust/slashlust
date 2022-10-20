using UnityEngine;
using UnityEngine.UI;

#nullable enable

public class MenuScript : MonoBehaviour
{
  Toggle? gamepadDisabledToggle;
  Slider? sFXVolumeSlider;
  Slider? musicVolumeSlider;

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

  void SetMusicVolume(float value, Slider musicVolumeSlider)
  {
    musicVolumeSlider.value = value;

    SoundManagerScript.instance.SetMusicAudioSourceVolume(value);
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
    musicVolumeSlider =
      transform.Find(
        "MenuPanel/Body/ScrollArea/Column/MusicVolume/Slider"
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

    var musicVolume = LocalPrefs.GetMusicVolume();

    if (musicVolumeSlider != null)
    {
      SetMusicVolume(musicVolume, musicVolumeSlider);

      musicVolumeSlider.onValueChanged.AddListener((value) =>
      {
        SetMusicVolume(value, musicVolumeSlider);

        LocalPrefs.SetMusicVolume(value);
      });
    }
  }
}
