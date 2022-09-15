using UnityEngine;

#nullable enable

public enum LocalPrefKeys
{
  gamepadDisabled,
}

public static class LocalPrefs
{
  public static bool GetGamepadEnabled()
  {
    return PlayerPrefs.GetInt(
      LocalPrefKeys.gamepadDisabled.ToString()
    ) == 0;
  }

  public static void SetGamepadEnabled(bool gamepadEnabled)
  {
    PlayerPrefs.SetInt(
      LocalPrefKeys.gamepadDisabled.ToString(),
      gamepadEnabled ? 0 : 1
    );
  }
}
