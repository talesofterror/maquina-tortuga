using UnityEngine;

public class FSM_PlayerStateController : MonoBehaviour
{
  public FSM_PlayerStateBase currentState;
  public FSM_PlayerStateBase cachedState;

  public FSM_PlayerState_Normal state_Normal;
  public FSM_PlayerState_Looking state_Looking;
  public FSM_PlayerState_Fight state_Fight;
  public FSM_PlayerState_Sliding state_Sliding;
  public FSM_PlayerState_Interact state_Interact;
  public FSM_PlayerState_Casting state_Casting;
  public FSM_PlayerState_Dead state_Dead;

  void Awake()
  {
    state_Normal = new FSM_PlayerState_Normal(this);
    state_Looking = new FSM_PlayerState_Looking(this);
    state_Fight = new FSM_PlayerState_Fight(this);
    state_Interact = new FSM_PlayerState_Interact(this);
    state_Casting = new FSM_PlayerState_Casting(this);
    state_Sliding = new FSM_PlayerState_Sliding(this);
    state_Dead = new FSM_PlayerState_Dead(this);
  }

  void Start()
  {
    SwitchState(state_Normal);
  }

  public bool ignoreModeChange = false;
  void Update()
  {
    currentState?.Update();
    currentState?.Loop();
    if (!ignoreModeChange)
    { ListenForModeChangeInput(); }
    if (ignoreModeChange)
    { ignoreModeChange = false; }

  }


  void ListenForModeChangeInput()
  {
    // fight mode
    if (GMSingleton.i.inputManager.modeChange.WasPressedThisFrame())
    {
      if (currentState != state_Fight)
      {
        SwitchState(state_Fight);
      }
      else
      {
        SwitchState(state_Normal);
      }
    }

    // slide mode
    if (currentState == state_Normal || currentState == state_Fight)
    {
      if (GMSingleton.i.inputManager.dash.WasPressedThisFrame())
      {
        cachedState = currentState;
        SwitchState(state_Sliding);
      }
    }
  }

  public void BeNormal()
  {
    SwitchState(state_Normal);
  }

  public void SwitchState(FSM_PlayerStateBase state)
  {
    currentState?.Exit();
    currentState = state;
    currentState.Enter();
    UISingleton.i.debug.pushMessage("Player entered " + state + " mode", "#22dd77");
  }
}
