using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Video;

#nullable enable

public class PlayerScript : MonoBehaviour
{
  // Referência.
  CharacterController? controller;
  PlayerInput? playerInput;
  Vector2 currentMoveInput;
  Vector2 processedMoveInput;
  Vector3 characterVelocity;
  Animator? anima;
  GameObject? model;
  Transform? cameraTransform;
  Vector2 currentLookInput;
  VideoPlayer? videoPlayer;
  PlayerBuffs playerBuffs = new PlayerBuffs();

  int killCount;
  bool attackLock;
  bool isDead = false;

  // Getters de referência.
  public PlayerInput? GetPlayerInput => playerInput;
  public PlayerBuffs GetPlayerBuffs() => playerBuffs;
  InventoryHolder? InventoryHolder;

  System.Collections.IEnumerator AttackRoutine()
  {
    if (!isDead)
    {
      anima?.SetBool("attack", true);
      attackLock = true;

      yield return new WaitForSeconds(0.2f);

      SoundManagerScript.instance.PlaySwordSwing();

      yield return new WaitForSeconds(0.2f);

      HandleAttack();

      anima?.SetBool("attack", false);
      attackLock = false;

      if (playerInput?.actions["Look"].ReadValue<Vector2>().magnitude > .5f)
      {
        StartCoroutine(AttackRoutine());
      }
    }
  }

  public void Back(InputAction.CallbackContext context)
  {
    if (context.performed)
    {
      OnMenuClick();
    }
  }

  // Cálculo do caminho da sala do player até a sala destino.
  // Só atualiza o minimapa caso seja necessário.
  public void CalculatePath(bool isFirstRender)
  {
    bool minimapRedrawn = false;

    var manager = GameManagerScript.instance;

    var network = manager.GetRoomNetwork;

    RaycastHit hit;
    if (Physics.Raycast(transform.position, Vector3.down, out hit, 5f, Layers.geometryMask))
    {
      // O raycast deu hit.
      var parent = hit.collider.transform.parent;

      // Não é um corredor.
      if (parent.name != "Corridor(Clone)")
      {
        // Atualiza a sala atual.
        manager.currentRoom = parent.gameObject;

        manager.AttemptEnemySpawn();

        // Verifica se faz sentido procurar um caminho.
        if (manager.currentRoom != null && network.bossRoom != null)
        {
          var start = network.roomNodes[manager.currentRoom.GetInstanceID()];

          var end = network.bossRoom;

          // Calcula o caminho.

          var bossRoomPath = network.Bfs(start, end);

          if (network.bossRoomPath?.Count != bossRoomPath?.Count)
          {
            // O tamanho dos caminhos é diferente.
            // É uma certeza mais barata do que loopar conferindo se são iguais.
            if (network.bossRoomPath != null && bossRoomPath != null)
            {
              // Os paths não são null, compará-los.

              var oldPath = "";

              foreach (var item in network.bossRoomPath)
              {
                oldPath += $"{item.room.GetInstanceID()} ";
              }

              var newPath = "";

              foreach (var item in bossRoomPath)
              {
                newPath += $"{item.room.GetInstanceID()} ";
              }

              if (oldPath != newPath)
              {
                // Os paths são diferentes, atualizar o antigo.

                network.bossRoomPath = bossRoomPath;

                if (network.mostDifficultRoom != null)
                {
                  network.difficultRoomPath =
                    network.AStar(start, network.mostDifficultRoom);
                }

                minimapRedrawn = true;

                manager.GetMinimapScript?.Layout(network);
              }
            }
            else
            {
              // Um dos paths é null, aceitar novo path.

              network.bossRoomPath = bossRoomPath;

              if (network.mostDifficultRoom != null)
              {
                network.difficultRoomPath =
                  network.AStar(start, network.mostDifficultRoom);
              }

              minimapRedrawn = true;

              manager.GetMinimapScript?.Layout(network);
            }
          }
        }
      }
    }

    if (isFirstRender && !minimapRedrawn)
    {
      manager.GetMinimapScript?.Layout(network);
    }
  }

  System.Collections.IEnumerator CalculatePathLoop()
  {
    while (true)
    {
      if (isDead)
      {
        break;
      }

      CalculatePath(false);

      yield return new WaitForSeconds(1f);
    }
  }

  void Die()
  {
    isDead = true;

    GameManagerScript.instance.UnsetPLayer();

    transform.Find("DogPolyart").gameObject.SetActive(false);
    transform.Find("PlayerHitPointsBar").gameObject.SetActive(false);
    transform.Find("DropCollider").gameObject.SetActive(false);

    MenuScript.instance.ShowDeathCard();
  }

  public void Fire(InputAction.CallbackContext context)
  {
    if (isDead)
    {
      return;
    }

    if (GameManagerScript.instance.GetMenuState == MenuState.open)
    {
      return;
    }

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
    InventoryHolder = GetComponent<InventoryHolder>();
    var weaponDamage = 1f;
    if (InventoryHolder.InventorySystem.InventorySlots[0].ItemData != null)
    {
      weaponDamage = InventoryHolder.InventorySystem.InventorySlots[0].ItemData.Damage;
    }
    Debug.Log("weaponDamage = " + weaponDamage);
    var offset2d = currentLookInput.normalized;
    var offset = new Vector3
    {
      x = offset2d.x,
      z = offset2d.y,
    };

    var colliders = Physics.OverlapSphere(
      transform.position + offset,
      playerBuffs.baseAttackRangeBuff,
      Layers.enemyMask
    );

    foreach (var collider in colliders)
    {
      var enemy = collider.transform.parent.gameObject;

      var enemyDied = false;

      var enemyScript = enemy.GetComponent<EnemyScript>();

      SoundManagerScript.instance.PlaySwordHit();

      if (enemyScript != null)
      {
        var damage = weaponDamage * playerBuffs.baseDamageBuff * playerBuffs.damageMultiplierBuff;
        Debug.Log("totalDamage = " + damage);

        enemyDied = enemyScript.InflictDamage(damage);

        GameManagerScript.instance
          .SpawnFloatingText(
            collider.transform.position,
            damage.ToString("0"),
            null
          );
      }

      if (enemyDied)
      {
        killCount++;

        SoundManagerScript.instance.PlayNpcDeath();
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

    if (LocalPrefs.GetGamepadDisabled())
    {
      GameManagerScript.instance.DisableGamepad();
    }

    if (videoPlayer != null)
    {
      HandleVideoConfigInitialization(videoPlayer);
    }

    TriggerUpdateStats();
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

  void HandleMouseAndKeyboardInput()
  {
    if (isDead)
    {
      return;
    }

    if (GameManagerScript.instance.GetControlState == ControlState.keyboard)
    {
      var mousePosition = Mouse.current.position;

      var xRatio = (mousePosition.x.ReadValue() / Screen.width) - .5f;
      var yRatio = (mousePosition.y.ReadValue() / Screen.height) - .5f;

      currentLookInput = new Vector2 { x = xRatio, y = yRatio };
    }
  }

  void HandleMovement(CharacterController controller)
  {
    if (isDead)
    {
      return;
    }

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

    controller.SimpleMove(
      Vector3.ClampMagnitude(move, 1f) *
      playerBuffs.baseMovementSpeedBuff *
      playerBuffs.movementSpeedMultiplierBuff
    );
  }

  void HandleVideoConfigInitialization(VideoPlayer videoPlayer)
  {
    var videoPath = Application.isMobilePlatform
      ? "Video/black-hole-mobile.mp4" : "Video/black-hole.mp4";

    videoPlayer.source = VideoSource.Url;
    videoPlayer.url = AssetLoader.GetPath(videoPath);

    videoPlayer.Prepare();
    videoPlayer.prepareCompleted += (source) =>
    {
      videoPlayer.Play();
    };
  }

  public void Heal(float healHitPoints)
  {
    var newHitPoints = playerBuffs.baseHitPoints + healHitPoints;

    if (newHitPoints > playerBuffs.initialHitPoints)
    {
      playerBuffs.baseHitPoints = playerBuffs.initialHitPoints;
    }
    else
    {
      playerBuffs.baseHitPoints = newHitPoints;
    }
  }

  public void Look(InputAction.CallbackContext context)
  {
    if (isDead)
    {
      return;
    }

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

  // Método usado para resetar o input do gamepad, evitando o um problema
  // relacionado ao input.
  public void Move()
  {
    if (isDead)
    {
      return;
    }

    var value = Vector2.zero;

    currentMoveInput = value;
  }

  public void Move(InputAction.CallbackContext context)
  {
    if (isDead)
    {
      return;
    }

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

  public void TakeDamage(float damage)
  {
    if (isDead)
    {
      return;
    }

    var newHitPoints = playerBuffs.baseHitPoints - damage;

    GameManagerScript.instance.SpawnFloatingText(
      transform.position,
      damage.ToString("- 0"),
      Colors.roseMadder
    );

    SoundManagerScript.instance.PlayHitMarker();

    if (newHitPoints <= 0)
    {
      Die();
    }
    else
    {
      playerBuffs.baseHitPoints = newHitPoints;
    }
  }

  public void TriggerUpdateStats()
  {
    StatsScript.instance?.UpdateStats(playerBuffs);
  }

  void Awake()
  {
    controller = GetComponent<CharacterController>();
    playerInput = GetComponent<PlayerInput>();
    model = transform.Find("DogPolyart").gameObject;
    anima = model.GetComponent<Animator>();
    cameraTransform = Camera.main.transform;
    videoPlayer = Camera.main.gameObject.GetComponent<VideoPlayer>();

    playerBuffs.baseHitPoints = playerBuffs.initialHitPoints;
  }

  void OnGUI()
  {
    GUI.Label(new Rect(100, 16, 100, 20), $"Kill count: {killCount}");
  }

  void SetWeapon()
  {
    InventoryHolder = GetComponent<InventoryHolder>();
    var equippedWeaponName = "empty";
    if (InventoryHolder.InventorySystem.InventorySlots[0].ItemData != null)
    {
      equippedWeaponName = InventoryHolder.InventorySystem.InventorySlots[0].ItemData.DisplayName;
    }
    var weapons = transform.Find("DogPolyart/root/pelvis/Weapon").gameObject;

    for (int i = 0; i < weapons.transform.childCount; i++)
    {
      GameObject child = weapons.transform.GetChild(i).gameObject;
      if (child.name != equippedWeaponName)
      {
        child.SetActive(false);
      }
      else
      {
        child.SetActive(true);
      }
    }
  }

  void Start()
  {
    HandleConfigInitialization();
    StartCoroutine(CalculatePathLoop());
  }

  void Update()
  {
    SetWeapon();
    var manager = GameManagerScript.instance;

    switch (manager.GetMenuState)
    {
      case MenuState.closed:
        HandleMouseAndKeyboardInput();

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
