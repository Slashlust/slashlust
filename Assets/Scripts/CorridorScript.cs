using UnityEngine;

public class CorridorScript : MonoBehaviour
{
  [SerializeField]
  Vector3 dimensions;

  public Vector3 GetDimensions => dimensions;
}
