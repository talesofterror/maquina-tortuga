using Invector.vCharacterController;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;

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

  public bool movementEnabled = true;

  public LayerMask layerMask;

  void Awake()
  {
    if (PLAYERSingleton.i != null && PLAYERSingleton.i != this)
    {
      Destroy(this.gameObject);
    }
    else
    {
      _playerSingleton = this;
      DontDestroyOnLoad(this.gameObject);
    }

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

    if (movementDisabled)
    {
      vInput.inputAction_Move.Disable();
    }
    else
    {
      vInput.inputAction_Move.Enable();
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

  public void SetControlsActiveState(bool state)
  {
    movementEnabled = state;
  }
}
