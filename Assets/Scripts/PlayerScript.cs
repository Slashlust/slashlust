using UnityEngine;
using UnityEngine.InputSystem;

#nullable enable

public class PlayerScript : MonoBehaviour
{
  CharacterController? controller;

  Vector2 currentMoveInput;
  Vector2 processedMoveInput;

  void HandleAttack()
  {
    var colliders = Physics.OverlapSphere(transform.position, 1f, 0b1000000);

    foreach (var collider in colliders)
    {
      Destroy(collider.transform.parent.gameObject);
    }
  }

  void HandleMovement(CharacterController controller)
  {
    var targetMoveInput = currentMoveInput;

    processedMoveInput = Vector2.Lerp(
      processedMoveInput,
      targetMoveInput,
      .05f
    );

    var moveInput = processedMoveInput;

    var move = moveInput.y * transform.forward + moveInput.x * transform.right;

    controller.SimpleMove(Vector3.ClampMagnitude(move, 1f) * 6f);
  }

  public void Fire(InputAction.CallbackContext context)
  {
    var value = context.ReadValue<float>();

    HandleAttack();
  }

  public void Move(InputAction.CallbackContext context)
  {
    var value = context.ReadValue<Vector2>();

    currentMoveInput = value;
  }

  void Awake()
  {
    controller = GetComponent<CharacterController>();
  }

  void Start()
  {

  }

  void Update()
  {
    if (controller != null)
    {
      HandleMovement(controller: controller);
    }
  }
}
