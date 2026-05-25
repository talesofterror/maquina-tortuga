using System.Collections;
using PixelCrushers.DialogueSystem;
using UnityEngine;
using UnityEngine.AI;


[SelectionBase]
public class Animal_IronGolem : MonoBehaviour, I_Animal
{
  [Header("ScriptableObject")]
  public SO_IronGolem stats;
  public IronGolem_FSM_Controller controller;

  [Header("Stats")]
  public int _hp;
  public int _ap;
  public int _mp;
  public bool dead;
  public bool resurrectable = false;
  public float resurrectionDelay = 30;

  public int hp
  {
    get { return _hp; }
    set { _hp = value; }
  }

  public int ap
  {
    get { return _ap; }
    set { _ap = value; }
  }

  public int mp
  {
    get { return _mp; }
    set { _mp = value; }
  }

  [Header("Movement Settings")]
  public float speed;
  public float alertDuration;

  [Header("Attack Settings")]
  [HideInInspector]
  public bool attacking;
  public float attackCooldown;
  public float smashRadius;
  public float smashDamageDelay;
  public float smashDamageDuration;
  public IronGolem_SmashDetector smashDetector;
  public float smashThrustForce;
  public float smashKnockbackForce = 300f;

  [Header("Detection Settings")]
  [SerializeField]
  float sightHeight;

  [SerializeField]
  float sightDistance;

  public float forgetDistance;

  [SerializeField]
  float scanSpeed;

  [SerializeField]
  float scanAngle;

  [Header("Refinement Settings")]
  [SerializeField]
  float pathUpdateFrequency;
  float pathUpdateTimer;
  LayerMask playerLayerMask;
  RaycastHit playerRaycastHit;

  [HideInInspector]
  public EnemyMode mode;

  [HideInInspector]
  public EnemyMode _currentMode;

  [HideInInspector]
  public bool inTransit;

  [HideInInspector]
  public bool running;

  [HideInInspector]
  public Vector3 direction;

  private bool returningFromInterrupt;

  private bool alertBehaviorActive;

  [HideInInspector]
  public Rigidbody rB;

  [Header("Gizmo Settings")]
  public Color _waypointSystemLineColor = Color.red;
  public Color _waypointMarkerColor = Color.magenta;

  private WaypointSystem waypointSystem;
  private Animator animator;
  public AnimatorStateInfo animatorStateInfo;
  private NavMeshAgent navMeshAgent;
  private Coroutine movementMotorCoroutine;
  private Coroutine initPlayerDetected;
  private Coroutine alertStartBehavior;
  private Coroutine attackBehavior;
  public bool isTakingDamage = false;
  private Coroutine takingDamageBehavior;

  void Awake()
  {
    if (stats != null)
    {
      _hp = stats.maxHP;
      _ap = stats.maxAP;
      speed = stats.speed;
      alertDuration = stats.alertDuration;
      attackCooldown = stats.attackCooldown;
      smashRadius = stats.smashRadius;
      smashDamageDelay = stats.smashDamageDelay;
      smashDamageDuration = stats.smashDamageDuration;
      smashThrustForce = stats.smashThrustForce;
      sightHeight = stats.sightHeight;
      sightDistance = stats.sightDistance;
      forgetDistance = stats.forgetDistance;
      scanSpeed = stats.scanSpeed;
      scanAngle = stats.scanAngle;
      pathUpdateFrequency = stats.pathUpdateFrequency;
    }

    waypointSystem = GetComponentInChildren<WaypointSystem>();

    // mode = EnemyMode.Patrol;
    rB = GetComponent<Rigidbody>();
    animator = GetComponent<Animator>();
    animatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);
    playerLayerMask = LayerMask.GetMask("Player");
    navMeshAgent = GetComponent<NavMeshAgent>();
    smashDetector = GetComponentInChildren<IronGolem_SmashDetector>(true);
    smashDetector.golem = this;
  }

  public void TakeDamage(int amount, GameObject focus)
  {
    controller.cachedState = controller._currentState;
    controller.focus = focus;
    controller.SwitchState(controller.state_TakingDamage, amount);
    //   controller.focus = focus;
    //   isTakingDamage = true;
    //   hp = hp - amount;
    //   Debug.Log("Iron Golem took " + amount + " damage!");
    //   DialogueManager.ShowAlert("Iron Golem took " + amount + " damage!");

    //   if (hp <= 0)
    //   {
    //     controller.SwitchState(controller.state_Dead);
    //     return;
    //   }

    //   Debug.Log(transform.name + " switched to Damage mode.");

    //   isTakingDamage = true;

    //   if (takingDamageBehavior == null)
    //   {
    //     takingDamageBehavior = StartCoroutine(TakingDamageCoroutine());
    //   }

    //   IEnumerator TakingDamageCoroutine()
    //   {
    //     animator.SetTrigger("Knockback");
    //     yield return new WaitForSeconds(3);
    //     animator.ResetTrigger("Knockback");

    //     takingDamageBehavior = null;
    //     isTakingDamage = false;
    //   }
    // }
  }
}
