using UnityEngine;

public class FSM_PlayerStateController : MonoBehaviour
{
  public FSM_PlayerStateBase currentState;

  public FSM_PlayerState_Normal state_Normal;
  public FSM_PlayerState_Fight state_Fight;
  public FSM_PlayerState_Interact state_Interact;

  void Start()
  {
    state_Normal = new FSM_PlayerState_Normal(this);
    state_Fight = new FSM_PlayerState_Fight(this);
    state_Interact = new FSM_PlayerState_Interact(this);

    currentState = state_Normal;
  }


  void Update()
  {
    currentState?.Update();
  }

  public void SwitchState(FSM_PlayerStateBase state)
  {
    currentState.Exit();
    currentState = state;
    currentState.Enter();
    UISingleton.i.debug.pushMessage("Player entered " + state + " mode", "#22dd77");
  }
}
