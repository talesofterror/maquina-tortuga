using System.Collections;
using PixelCrushers.DialogueSystem;
using UnityEngine;
using UnityEngine.AI;

public class IronGolem_FSM_Controller : FSM_BaseController
{
  [HideInInspector] public Animal_IronGolem animalScript;
  [HideInInspector] public WaypointSystem waypointSystem;
  [HideInInspector] public Animator animator;
  [HideInInspector] public AnimatorStateInfo animatorStateInfo;
  [HideInInspector] public NavMeshAgent navMeshAgent;
  [HideInInspector] public Rigidbody rB;
  [HideInInspector] public LayerMask playerLayerMask;
  [HideInInspector] public GameObject focus;

  public float forgettingDistance = 10;

  public IronGolem_FSM_State_Patrol state_Patrol;
  public IronGolem_FSM_State_Alert state_Alert;

  void Awake()
  {
    
    animator = GetComponent<Animator>();
    animatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);
    playerLayerMask = LayerMask.GetMask("Player");
    navMeshAgent = GetComponent<NavMeshAgent>();
    rB = GetComponent<Rigidbody>();
    animalScript = GetComponent<Animal_IronGolem>();

    state_Patrol = new IronGolem_FSM_State_Patrol(this);
    state_Alert = new IronGolem_FSM_State_Alert(this);

    SwitchState(state_Patrol);
  }

  void Start()
  {
  }

  void Update()
  {
    _currentState?.Update();
  }

  public override void SwitchState(FSM_Base newState)
  {
    _currentState?.Exit();
    _currentState = newState;
    _currentState?.Enter();
  }
}
