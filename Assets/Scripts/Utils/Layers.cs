using UnityEngine;

public class Layers
{
  public static int defaultLayer = 0;
  public static int uiLayer = 5;
  public static int enemyLayer = 6;
  public static int geometryLayer = 7;

  public static int enemyMask = ((int)Mathf.Pow(2, enemyLayer));
  public static int geometryMask = ((int)Mathf.Pow(2, geometryLayer));
}