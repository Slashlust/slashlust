using UnityEngine;

#nullable enable

public class PlayerScript : MonoBehaviour
{
  CharacterController? controller;

  void Awake()
  {
    controller = GetComponent<CharacterController>();
  }

  void Start()
  {

  }

  void Update()
  {

  }
}
