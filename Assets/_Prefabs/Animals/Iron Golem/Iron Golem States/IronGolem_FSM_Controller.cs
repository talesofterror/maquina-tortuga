using System.Collections;
using PixelCrushers.DialogueSystem;
using UnityEngine;
using UnityEngine.AI;

public class IronGolem_FSM_Controller : FSM_BaseController
{
  public Animal_IronGolem animalScript;

  public WaypointSystem waypointSystem;
  public Animator animator;
  public AnimatorStateInfo animatorStateInfo;
  public NavMeshAgent navMeshAgent;
  LayerMask playerLayerMask;
  public Rigidbody rB;

  public IronGolem_FSM_State_Patrol state_Patrol;

  void Awake()
  {
    waypointSystem = GetComponentInChildren<WaypointSystem>();
    animator = GetComponent<Animator>();
    animatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);
    playerLayerMask = LayerMask.GetMask("Player");
    navMeshAgent = GetComponent<NavMeshAgent>();
    rB = GetComponent<Rigidbody>();
    animalScript = GetComponent<Animal_IronGolem>();

    state_Patrol = new IronGolem_FSM_State_Patrol(this);

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
