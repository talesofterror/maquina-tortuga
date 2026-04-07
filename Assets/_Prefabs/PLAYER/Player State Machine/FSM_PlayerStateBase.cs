using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public abstract class FSM_PlayerStateBase
{
  protected FSM_PlayerStateController controller;

  protected FSM_PlayerStateBase _currentSubState;
  protected FSM_PlayerStateBase _superState;

  // public FSM_PlayerStateBase(FSM_PlayerStateController controller)
  // {
  //   this.controller = controller;
  // }
  // * as above so below?
  public FSM_PlayerStateBase(FSM_PlayerStateController c) => controller = c;

  public virtual void Enter() {}
  public virtual void Exit() {}

  public void Update()
  {
    _currentSubState?.Update();
    Update_Additive();
  }

  public virtual void Update_Additive()
  {

  }

  public void SetSubState(FSM_PlayerStateBase subState)
  {
    _currentSubState = subState;
    subState.SetSuperState(this);
    subState.Enter();
  }
  protected void SetSuperState(FSM_PlayerStateBase superState)
  {
    _superState = superState;
  }
}
