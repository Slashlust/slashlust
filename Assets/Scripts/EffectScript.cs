using System.Collections.Generic;
using UnityEngine;
using Unity.AI.Navigation;

#nullable enable
class EffectScript : MonoBehaviour
{
  System.Collections.IEnumerator DestroyRoutine()
  {
    yield return new WaitForSeconds(2f);
    Destroy(gameObject);
  }
  void Start()
  {
    StartCoroutine(DestroyRoutine());
  }
}
