using UnityEngine;

public class Layers
{
  public static readonly int defaultLayer = 0;
  public static readonly int uiLayer = 5;
  public static readonly int enemyLayer = 6;
  public static readonly int geometryLayer = 7;

  public static readonly int enemyMask = ((int)Mathf.Pow(2, enemyLayer));
  public static readonly int geometryMask = ((int)Mathf.Pow(2, geometryLayer));
}