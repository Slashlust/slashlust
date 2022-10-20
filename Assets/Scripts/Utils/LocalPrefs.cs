using UnityEngine;

#nullable enable

public enum LocalPrefKeys
{
  gamepadDisabled,
  musicVolume,
  sFXVolume,
}

public static class LocalPrefs
{
  public static bool GetGamepadDisabled()
  {
    return PlayerPrefs.GetInt(
      LocalPrefKeys.gamepadDisabled.ToString()
    ) == 1;
  }

  public static void SetGamepadDisabled(bool gamepadDisabled)
  {
    PlayerPrefs.SetInt(
      LocalPrefKeys.gamepadDisabled.ToString(),
      gamepadDisabled ? 1 : 0
    );
  }

  public static float GetSFXVolume()
  {
    return PlayerPrefs.GetFloat(
      LocalPrefKeys.sFXVolume.ToString(),
      .5f
    );
  }

  public static void SetSFXVolume(float sFXVolume)
  {
    PlayerPrefs.SetFloat(
      LocalPrefKeys.sFXVolume.ToString(),
      sFXVolume
    );
  }

  public static float GetMusicVolume()
  {
    return PlayerPrefs.GetFloat(
      LocalPrefKeys.musicVolume.ToString(),
      .5f
    );
  }

  public static void SetMusicVolume(float musicVolume)
  {
    PlayerPrefs.SetFloat(
      LocalPrefKeys.musicVolume.ToString(),
      musicVolume
    );
  }
}
