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
  public IronGolem_FSM_State_Pursue state_Pursue;
  public IronGolem_FSM_State_Attack state_Attack;
  public IronGolem_FSM_State_TakingDamage state_TakingDamage;
  public IronGolem_FSM_State_Dead state_Dead;

  public FSM_Base cachedState;

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
    state_Pursue = new IronGolem_FSM_State_Pursue(this);
    state_Attack = new IronGolem_FSM_State_Attack(this);
    state_TakingDamage = new IronGolem_FSM_State_TakingDamage(this);
    state_Dead = new IronGolem_FSM_State_Dead(this);

    SwitchState(state_Patrol);
  }

  void Start()
  {
  }

  void Update()
  {
    _currentState?.Update();

    if (!animalScript.dead) ListenForDeath();
  }

  public override void SwitchState(FSM_Base newState, int option = 0)
  {
    _currentState?.Exit();
    _currentState = newState;
    if (option != 0) _currentState.Option(option);
    _currentState?.Enter();
  }

  void ListenForDeath ()
  {
    if (animalScript.hp <= 0)
    {
      SwitchState(state_Dead);
    }
  }
}

/*

  IronGolem_FSM_Controller controller;

  IronGolem_FSM_State_[[[STATE]]] (IronGolem_FSM_Controller c) : base (c)
  {
    controller = c;
  }
  
  public override void Enter()
  {
    
  }
  public override void Loop()
  {
    
  }
  public override void Exit()
  {
    
  }

*/
