using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

#nullable enable

public class MenuScript : MonoBehaviour
{
  static public MenuScript instance = default!;

  Toggle? gamepadDisabledToggle;
  Slider? sFXVolumeSlider;
  Slider? musicVolumeSlider;
  Button? quitButton;
  GameObject? deathCard;
  GameObject? successCard;

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

  public void ShowDeathCard()
  {
    deathCard?.SetActive(true);
  }

  public void ShowSuccessCard()
  {
    successCard?.SetActive(true);
  }

  void Awake()
  {
    instance = this;

    deathCard = transform.Find("DeathCard").gameObject;
    deathCard?.SetActive(false);

    successCard = transform.Find("SuccessCard").gameObject;
    successCard?.SetActive(false);

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
    quitButton =
      transform.Find(
        "MenuPanel/Body/ScrollArea/Column/Quit/Button"
      ).GetComponent<Button>();
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

    if (quitButton != null)
    {
      quitButton.onClick.AddListener(() =>
      {
        SceneManager.LoadScene(((int)GameScenes.menuScene), LoadSceneMode.Single);
      });
    }
  }
}
