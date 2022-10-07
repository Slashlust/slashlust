using UnityEngine;

#nullable enable

public enum LocalPrefKeys
{
  gamepadDisabled,
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
}
