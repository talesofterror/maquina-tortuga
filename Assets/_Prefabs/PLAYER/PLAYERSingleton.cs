using Invector.vCharacterController;
using PixelCrushers.DialogueSystem;
using UnityEngine;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public class PLAYERSingleton : MonoBehaviour
{
  private static PLAYERSingleton _playerSingleton;
  public static PLAYERSingleton i
  {
    get { return _playerSingleton; }
  }

  [Header("Player References")]
  public FSM_PlayerStateController stateController;

  public PlayerHealth playerHealth;
  public bool isTakingDamage;

  [Header("Interaction Settings")]
  public float interactionSightDistance = 10;
  public float interactionReachDistance = 2;
  [HideInInspector] public Interactable sightedInteractable;

  [Header("Weapon")]
  public PlayerWeapons playerWeapons;
  public GameObject computer;
  [HideInInspector] public ComputerController computerController;

  [Header("Animation")]
  public PlayerAnimations animations;

  [Header("Movement & Camera")]
  public bool inputDisabled;
  public bool movementDisabled;
  public bool endlessJumping;
  public GameObject cameraTargetGameobject;

  [Header("Layer Masks")]
  public LayerMask layerMask_Player;
  public LayerMask layerMask_Interactable;
  public LayerMask LayerMask_Attackable;

  [HideInInspector]
  public vThirdPersonController vController;

  [HideInInspector]
  public vThirdPersonInput vInput;

  [HideInInspector]
  public Rigidbody rB;

  [HideInInspector]
  public bool playerIsAttacking;

  [Header("Dialogue Settings")]
  public Selector_CustomRaycast_Camera dialogueSelector;
  public SelectorUseStandardUIElements useStandardUIElementsComponent;

  void Awake()
  {
    if (PLAYERSingleton.i != null && PLAYERSingleton.i != this)
    {
      Destroy(this.gameObject);
      return;
    }
    else
    {
      _playerSingleton = this;
      DontDestroyOnLoad(this.gameObject);
    }

    dialogueSelector = GetComponent<Selector_CustomRaycast_Camera>();
    useStandardUIElementsComponent = GetComponent<SelectorUseStandardUIElements>();

    stateController = GetComponent<FSM_PlayerStateController>();

    // Debug.Log("PLAYERSingleton called Awake");
    rB = GetComponent<Rigidbody>();
    vController = GetComponent<vThirdPersonController>();
    vInput = GetComponent<vThirdPersonInput>();
    playerHealth = GetComponent<PlayerHealth>();
    computerController = computer.GetComponent<ComputerController>();
    stateController.state_Casting.computerController = computerController;

    layerMask_Player = LayerMask.GetMask("Player");
    layerMask_Interactable = LayerMask.GetMask("Interactable");

    computer.SetActive(false);
  }

  void OnEnable()
  {
    SceneManager.sceneLoaded += OnSceneLoaded;
  }

  void OnDisable()
  {
    SceneManager.sceneLoaded -= OnSceneLoaded;
  }

  private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
  {
    Debug.Log("Transporting Player to spawn point");
    if (mode == LoadSceneMode.Additive) return;

    // Move player to the spawn point
    Transform SceneLevelSpawn = GameObject.FindWithTag("Respawn").transform;

    if (SceneLevelSpawn != null)
    {
      rB.linearVelocity = Vector3.zero;
      rB.angularVelocity = Vector3.zero;

      rB.position = SceneLevelSpawn.position;
      rB.rotation = SceneLevelSpawn.rotation;

      transform.position = SceneLevelSpawn.position;
      transform.rotation = SceneLevelSpawn.rotation;
    }
    else
    {
      Debug.LogWarning($"Scene '{scene.name}' loaded, but no GameObject with the tag 'Respawn' was found!");
    }
  }

  void OnDestroy()
  {
    if (_playerSingleton == this)
    {
      _playerSingleton = null;
    }
  }

  void Start()
  {

  }

  RigidbodyConstraints cachedRBConstraints;

  void Update()
  {

    if (!ignoreModeChange)
    { ListenForModeChangeInput(); }
    if (ignoreModeChange)
    { ignoreModeChange = false; }

    if (inputDisabled || DialogueManager.IsConversationActive)
    {

      vInput.inputAction_Move.Disable();
      vInput.inputAction_Jump.Disable();
      // vInput.enabled = false;
      // GMSingleton.i.inputManager.inputSystem.Player.Disable();
    }
    else
    {
      vInput.inputAction_Move.Enable();
      vInput.inputAction_Jump.Enable();
      // vInput.enabled = true;
      // GMSingleton.i.inputManager.inputSystem.Player.Enable();
      // vInput.inputAction_Move.Enable();
    }

    // if (movementDisabled)
    // {
    //   cachedRBConstraints = rB.constraints;
    //   rB.constraints = RigidbodyConstraints.FreezePosition;
    // }
    // else
    // {
    //   rB.constraints = RigidbodyConstraints.FreezeRotation;
    // }
  }

  public bool ignoreModeChange = false;

  void ListenForModeChangeInput()
  {
    if (GMSingleton.i.inputManager.modeChange.WasPressedThisFrame())
    {
      if (PLAYERSingleton.i.stateController.currentState != PLAYERSingleton.i.stateController.state_Fight)
      {
        stateController.SwitchState(stateController.state_Fight);
      }
      else
      {
        stateController.SwitchState(stateController.state_Normal);
      }
    }
  }

}
