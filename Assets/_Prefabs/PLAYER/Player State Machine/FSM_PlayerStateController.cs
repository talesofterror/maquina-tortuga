using UnityEngine;

public class FSM_PlayerStateController : MonoBehaviour
{
  FSM_PlayerStateBase _currentState;

  public FSM_PlayerState_Normal state_Normal;
  public FSM_PlayerState_Fight state_Fight;

  void Start()
  {
    state_Normal = new FSM_PlayerState_Normal(this);
    state_Fight = new FSM_PlayerState_Fight(this);

    _currentState = state_Normal;
  }


  void Update()
  {
    _currentState?.Update();
  }

  public void SwitchState(FSM_PlayerStateBase state)
  {
    _currentState.Exit();
    _currentState = state;
    _currentState.Enter();
    UISingleton.i.debug.pushMessage("Player entered " + state + " mode", "#22dd77");
  }
}
