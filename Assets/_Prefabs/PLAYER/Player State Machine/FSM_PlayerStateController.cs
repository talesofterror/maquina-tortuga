using UnityEngine;

public class FSM_PlayerStateController : MonoBehaviour
{
  public FSM_PlayerStateBase currentState;

  public FSM_PlayerState_Normal state_Normal;
  public FSM_PlayerState_Looking state_Looking;
  public FSM_PlayerState_Fight state_Fight;
  public FSM_PlayerState_Interact state_Interact;

  void Awake()
  {
    state_Normal = new FSM_PlayerState_Normal(this);
    state_Looking = new FSM_PlayerState_Looking(this);
    state_Fight = new FSM_PlayerState_Fight(this);
    state_Interact = new FSM_PlayerState_Interact(this);
  }

  void Start()
  {
    currentState = state_Normal;
    state_Normal.SetSubState(state_Looking);
  }

  void Update()
  {
    currentState?.Update();
    currentState?.Loop();
  }

  public void SwitchState(FSM_PlayerStateBase state)
  {
    currentState.Exit();
    currentState = state;
    currentState.Enter();
    UISingleton.i.debug.pushMessage("Player entered " + state + " mode", "#22dd77");
  }
}
