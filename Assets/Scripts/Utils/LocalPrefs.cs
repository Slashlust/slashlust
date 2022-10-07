using UnityEngine;

#nullable enable

public enum LocalPrefKeys
{
  gamepadEnabled,
}

public static class LocalPrefs
{
  public static bool GetGamepadEnabled()
  {
    return PlayerPrefs.GetInt(
      LocalPrefKeys.gamepadEnabled.ToString()
    ) == 1;
  }

  public static void SetGamepadEnabled(bool gamepadEnabled)
  {
    PlayerPrefs.SetInt(
      LocalPrefKeys.gamepadEnabled.ToString(),
      gamepadEnabled ? 1 : 0
    );
  }
}
