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
  Vector3 characterVelocity;
  Animator? anima;
  GameObject? model;
  Transform? cameraTransform;

  float lastMoveTimestamp;
  int killCount;
  bool attackLock;

  System.Collections.IEnumerator AttackRoutine()
  {
    anima?.SetBool("attack", true);
    attackLock = true;

    yield return new WaitForSeconds(0.4f);

    HandleAttack();

    anima?.SetBool("attack", false);
    attackLock = false;
  }

  void HandleAttack()
  {
    var offset = characterVelocity.normalized;

    var colliders = Physics.OverlapSphere(
      transform.position + offset,
      1f,
      0b1000000
    );

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

  void HandleCameraTransforms(Transform cameraTransform)
  {
    var movementVector = characterVelocity;

    cameraTransform.localPosition =
      Vector3.Lerp(
        cameraTransform.localPosition,
        movementVector.normalized,
        .02f
      );
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

  void HandleModelAnimation(GameObject model)
  {
    model.transform.rotation =
      Quaternion.Lerp(
        model.transform.rotation,
        Quaternion.Euler(
          0f,
          Mathf.Atan2(characterVelocity.x, characterVelocity.z) * Mathf.Rad2Deg,
          0f
        ),
      .1f
    );
  }

  public void Fire(InputAction.CallbackContext context)
  {
    if (attackLock)
    {
      return;
    }

    if (context.performed)
    {
      var value = context.ReadValue<float>();

      StartCoroutine(AttackRoutine());
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

    var tempCharacterVelocity = controller?.velocity ?? Vector3.zero;

    if (tempCharacterVelocity.magnitude > .1f)
    {
      characterVelocity = tempCharacterVelocity;
    }

    anima?.SetFloat("speed", (controller?.velocity.magnitude ?? 0f) / 1.2f);
  }

  void Awake()
  {
    controller = GetComponent<CharacterController>();
    playerInput = GetComponent<PlayerInput>();
    model = transform.Find("DogPolyart").gameObject;
    anima = model.GetComponent<Animator>();
    cameraTransform = Camera.main.transform;
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

    if (model != null)
    {
      HandleModelAnimation(model);
    }

    if (cameraTransform != null)
    {
      HandleCameraTransforms(cameraTransform);
    }
  }
}
