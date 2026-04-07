using Invector.vCharacterController;
using PixelCrushers.DialogueSystem;
using UnityEngine;

public class PLAYERSingleton : MonoBehaviour 
{
  private static PLAYERSingleton _playerSingleton;
  public static PLAYERSingleton i
  {
    get { return _playerSingleton; }
  }

  public FSM_PlayerStateController stateController;

  public PlayerHealth playerHealth;
  public bool isTakingDamage;

  public float interactionSightDistance = 10;
  public float interactionReachDistance = 2; 

  public PlayerWeapons playerWeapons;
  public PlayerAnimations animations;

  [HideInInspector]
  public vThirdPersonController vController;

  [HideInInspector]
  public vThirdPersonInput vInput;

  [HideInInspector]
  public Rigidbody rB;

  [HideInInspector]
  public bool playerIsAttacking;

  public bool movementDisabled;

  public bool endlessJumping;

  public LayerMask layerMask;

  [Header("Diaglogue Settings")]
  public Selector_CustomRaycast dialogueSelector;
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

    dialogueSelector = GetComponent<Selector_CustomRaycast>();
    useStandardUIElementsComponent = GetComponent<SelectorUseStandardUIElements>();
    
    stateController = GetComponent<FSM_PlayerStateController>();

    // Debug.Log("PLAYERSingleton called Awake");
    rB = GetComponent<Rigidbody>();
    vController = GetComponent<vThirdPersonController>();
    vInput = GetComponent<vThirdPersonInput>();
    playerHealth = GetComponent<PlayerHealth>();
    
    layerMask = LayerMask.GetMask("Player");

  }

  void OnDestroy()
  {
    if (_playerSingleton == this)
    {
      _playerSingleton = null;
    }
  }

  void Start() { }

  void Update()
  {
    ListenForModeChangeInput();

    if (movementDisabled || DialogueManager.IsConversationActive)
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
  }

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
