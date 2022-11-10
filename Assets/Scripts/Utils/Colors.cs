using UnityEngine;

#nullable enable

public static class Colors
{
  public static Color jet = Parse("#2A2826");
  public static Color roseMadder = Parse("#Df2935");
  public static Color blueCrayola = Parse("#3772FF");
  public static Color sunglow = Parse("#FDCA40");
  public static Color spanishGreen = Parse("#018E42");
  public static Color orangePantone = Parse("#FE5F00");

  static Color Parse(string hex)
  {
    Color color;

    ColorUtility.TryParseHtmlString(hex, out color);

    return color;
  }
}
