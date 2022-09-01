using UnityEngine;
using UnityEngine.InputSystem;

#nullable enable

public class PlayerScript : MonoBehaviour
{
  CharacterController? controller;
  PlayerInput? playerInput;
  Vector2 currentMoveInput;
  Vector2 processedMoveInput;
  Vector2 lastMove;

  private Animator anima;

  float lastMoveTimestamp;
  int killCount;

  void HandleAttack()
  {
    var colliders = Physics.OverlapSphere(transform.position, 1f, 0b1000000);

    foreach (var collider in colliders)
    {
      var enemyDied = collider.transform.parent.gameObject
        .GetComponent<EnemyScript>().InflictDamage(20);

      if (enemyDied)
      {
        killCount++;
      }
    }
  }

  void HandleMovement(CharacterController controller)
  {
    var targetMoveInput = currentMoveInput;

    processedMoveInput = Vector2.Lerp(
      processedMoveInput,
      targetMoveInput,
      .08f
    );

    var moveInput = processedMoveInput;

    var move = moveInput.y * transform.forward + moveInput.x * transform.right;

    controller.SimpleMove(Vector3.ClampMagnitude(move, 1f) * 4f);
  }

  public void Fire(InputAction.CallbackContext context)
  {
    if (context.performed)
    {
      var value = context.ReadValue<float>();

      HandleAttack();
    }
  }

  public void Move(InputAction.CallbackContext context)
  {
    var value = context.ReadValue<Vector2>();

    if (value.magnitude == 0)
    {
      if (lastMove.magnitude == 0)
      {
        currentMoveInput = Vector2.zero;
      }
    }
    else
    {
      currentMoveInput = value;
    }

    lastMove = value;
    lastMoveTimestamp = Time.timeSinceLevelLoad;
  }

  public void MoveCheck()
  {
    var value = playerInput?.actions["Move"].ReadValue<Vector2>();

    if (
      value?.magnitude == 0 &&
      lastMove.magnitude == 0 &&
      Time.timeSinceLevelLoad - lastMoveTimestamp > .01f
    )
    {
      currentMoveInput = Vector2.zero;
    }
    anima.SetFloat("speed", controller.velocity.magnitude/1.2f);
  }

  void Awake()
  {
    controller = GetComponent<CharacterController>();
    playerInput = GetComponent<PlayerInput>();
    anima = transform.Find("DogPolyart").GetComponent<Animator>();
  }

  void OnGUI()
  {
    GUI.Label(new Rect(100, 16, 100, 20), $"Kill count: {killCount}");
  }

  void Update()
  {
    MoveCheck();

    if (controller != null)
    {
      HandleMovement(controller: controller);
    }
  }
}
