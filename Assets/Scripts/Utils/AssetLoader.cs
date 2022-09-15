using UnityEngine;

public static class AssetLoader
{
  public static string GetPath(string file)
  {
    return System.IO.Path.Combine(Application.streamingAssetsPath, file);
  }
}
