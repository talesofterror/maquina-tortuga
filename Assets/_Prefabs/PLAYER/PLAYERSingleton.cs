using Invector.vCharacterController;
using UnityEngine;
using UnityEngine.Animations;

public class PLAYERSingleton : MonoBehaviour
{
    private static PLAYERSingleton _playerSingleton;
    public static PLAYERSingleton i
    {
        get { return _playerSingleton; }
    }

    public PlayerHealth playerHealth;
    public bool isTakingDamage;

    public PlayerInteract playerInteract;
    public PlayerFightMode playerFightMode;

    public PlayerWeapons playerWeapons;
    public PlayerAnimations animations;

    [HideInInspector]
    public vThirdPersonController vController;

    [HideInInspector]
    public vThirdPersonInput vInput;

    [HideInInspector]
    public Rigidbody rB;

    [HideInInspector]
    public PlayerMode playerMode;

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

        // Debug.Log("PLAYERSingleton called Awake");
        rB = GetComponent<Rigidbody>();
        vController = GetComponent<vThirdPersonController>();
        vInput = GetComponent<vThirdPersonInput>();
        playerHealth = GetComponent<PlayerHealth>();

        playerMode = PlayerMode.Normal;
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

        if (playerMode == PlayerMode.Fight)
        {
            ListenForFightInput();
        }

        if (movementDisabled)
        {
            vInput.inputAction_Move.Disable();
        }
        else
        {
            vInput.inputAction_Move.Enable();
        }
    }

    void ListenForFightInput()
    {
        if (GMSingleton.i.inputManager.attack.WasReleasedThisFrame())
        {
            playerIsAttacking = true;
            string id = "SlashTrigger";
            StartCoroutine(
                animations.WaitAndFreeze(PLAYERSingleton.i.animations.stateInfo.length, id)
            );
            Invoke("SetPlayerIsAttackingFalse", PLAYERSingleton.i.animations.stateInfo.length);
        }
    }

    void SetPlayerIsAttackingFalse()
    {
        playerIsAttacking = false;
    }

    void ListenForModeChangeInput()
    {
        if (
            (
                GMSingleton.i.inputManager.modeChange.WasReleasedThisFrame()
                && playerMode == PlayerMode.Normal
            ) && !playerInteract.isTargettingInteractable
        )
        {
            playerFightMode.Activate();
        }
        else if (
            (
                GMSingleton.i.inputManager.modeChange.WasReleasedThisFrame()
                && playerMode == PlayerMode.Fight
            ) && !playerInteract.isTargettingInteractable
        )
        {
            playerFightMode.Deactivate(PlayerMode.Normal);
        }
    }

    public void SetControlsActiveState(bool state)
    {
        movementEnabled = state;
    }
}
