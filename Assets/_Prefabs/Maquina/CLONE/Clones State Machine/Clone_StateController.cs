using UnityEngine;

public class Clone_StateController : Maquina_BaseController
{

  public Clone_State_Idle state_Idle;
  public Clone_State_Walk state_Walk;

  void Awake()
  {
    state_Idle = new Clone_State_Idle(this);
    state_Walk = new Clone_State_Walk(this);

    interactionDetector = GetComponentInChildren<Maquina_DetectInteractions>();
  }

  void Start()
  {
    direction = transform.forward;
    cachedDirection = direction;

    state_Idle.SetSubState(state_Walk);
    SwitchState(state_Idle);
  }
  void Update()
  {
    _currentState?.Update();
  }
}
