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

  public abstract void Enter();
  public abstract void Exit();

  public virtual void Update()
  {
    _currentSubState?.Update();
  }

  protected void SetSubState(FSM_PlayerStateBase subState)
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
