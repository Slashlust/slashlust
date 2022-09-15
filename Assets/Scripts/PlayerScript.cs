using UnityEngine;
using UnityEngine.InputSystem;

#nullable enable

public class PlayerScript : MonoBehaviour
{
  CharacterController? controller;
  PlayerInput? playerInput;
  Vector2 currentMoveInput;
  Vector2 processedMoveInput;
  Vector3 characterVelocity;
  Animator? anima;
  GameObject? model;
  Transform? cameraTransform;
  Vector2 currentLookInput;

  int killCount;
  bool attackLock;

  public PlayerInput? GetPlayerInput => playerInput;

  System.Collections.IEnumerator AttackRoutine()
  {
    anima?.SetBool("attack", true);
    attackLock = true;

    yield return new WaitForSeconds(0.4f);

    HandleAttack();

    anima?.SetBool("attack", false);
    attackLock = false;

    if (playerInput?.actions["Look"].ReadValue<Vector2>().magnitude > .5f)
    {
      StartCoroutine(AttackRoutine());
    }
  }

  public void Back(InputAction.CallbackContext context)
  {
    if (context.performed)
    {
      OnMenuClick();
    }
  }

  public void Fire(InputAction.CallbackContext context)
  {
    if (GameManagerScript.instance.GetMenuState == MenuState.open)
    {
      return;
    }

    // TODO: Melhorar mecânica de ataque e autoswing com delay antes e após ataque e lock melhor
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

  void HandleAttack()
  {
    var offset2d = currentLookInput.normalized;

    var offset = new Vector3
    {
      x = offset2d.x,
      z = offset2d.y,
    };

    var colliders = Physics.OverlapSphere(
      transform.position + offset,
      1f,
      Layers.enemyMask
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

  public void HandleCharacterControllerUpdate()
  {
    var tempCharacterVelocity = controller?.velocity ?? Vector3.zero;

    if (tempCharacterVelocity.magnitude > .1f)
    {
      characterVelocity = tempCharacterVelocity;
    }
  }

  void HandleConfigInitialization()
  {
    GameManagerScript.instance.DisableMenu();

    if (!LocalPrefs.GetGamepadEnabled())
    {
      GameManagerScript.instance.DisableGamepad();
    }
  }

  void HandleModelAnimation(GameObject model)
  {
    var vector = attackLock ? new Vector3
    {
      x = currentLookInput.x,
      z = currentLookInput.y,
    } : characterVelocity;

    model.transform.rotation =
      Quaternion.Lerp(
        model.transform.rotation,
        Quaternion.Euler(
          0f,
          Mathf.Atan2(vector.x, vector.z) * Mathf.Rad2Deg,
          0f
        ),
      attackLock ? .4f : .1f
    );

    anima?.SetFloat("speed", (controller?.velocity.magnitude ?? 0f) / 1.2f);
  }

  void HandleMovement(CharacterController controller)
  {
    var targetMoveInput =
      GameManagerScript.instance.GetMenuState == MenuState.open
      ? Vector2.zero : currentMoveInput;

    processedMoveInput = Vector2.Lerp(
      processedMoveInput,
      targetMoveInput,
      .08f
    );

    var moveInput = processedMoveInput;

    var move = moveInput.y * transform.forward + moveInput.x * transform.right;

    controller.SimpleMove(Vector3.ClampMagnitude(move, 1f) * 4f);
  }

  public void Look(InputAction.CallbackContext context)
  {
    var value = context.ReadValue<Vector2>();

    if (
      playerInput?.currentControlScheme !=
        GameManagerScript.instance.GetTargetControlScheme
    )
    {
      return;
    }

    currentLookInput = value;

    if (currentLookInput.magnitude > .5f && !attackLock)
    {
      StartCoroutine(AttackRoutine());
    }
  }

  public void Move(InputAction.CallbackContext context)
  {
    var value = context.ReadValue<Vector2>();

    if (
      playerInput?.currentControlScheme !=
        GameManagerScript.instance.GetTargetControlScheme
    )
    {
      return;
    }

    currentMoveInput = value;
  }

  public void OnMenuClick()
  {
    if (GameManagerScript.instance.GetMenuState == MenuState.closed)
    {
      GameManagerScript.instance.EnableMenu();
    }
    else
    {
      GameManagerScript.instance.DisableMenu();
    }
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

  void Start()
  {
    HandleConfigInitialization();
  }

  void Update()
  {
    var manager = GameManagerScript.instance;

    switch (manager.GetMenuState)
    {
      case MenuState.closed:

        // TODO: Mover

        if (manager.GetControlState == ControlState.keyboard)
        {
          var mousePosition = Mouse.current.position;

          var xRatio = (mousePosition.x.ReadValue() / Screen.width) - .5f;
          var yRatio = (mousePosition.y.ReadValue() / Screen.height) - .5f;

          currentLookInput = new Vector2 { x = xRatio, y = yRatio };
        }

        break;
      case MenuState.open:

        break;
    }

    HandleCharacterControllerUpdate();

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
