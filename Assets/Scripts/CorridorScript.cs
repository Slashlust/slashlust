using UnityEngine;

#nullable enable

public class CorridorScript : MonoBehaviour
{
  [SerializeField]
  Vector3 dimensions;

  public Vector3 GetDimensions => dimensions;
}
