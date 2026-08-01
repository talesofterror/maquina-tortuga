using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public abstract class FSM_PlayerStateBase
{
  protected FSM_PlayerStateController controller;

  protected FSM_PlayerStateBase subState;
  protected FSM_PlayerStateBase superState;

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
    subState?.Loop();
    Loop();
  }

  public virtual void Loop()
  {

  }

  public void SetSubState(FSM_PlayerStateBase _subState)
  {
    this.subState?.Exit();
    this.subState = _subState;
    _subState.SetSuperState(this);
    _subState.Enter();
  }
  protected void SetSuperState(FSM_PlayerStateBase superState)
  {
    this.superState = superState;
  }
}
