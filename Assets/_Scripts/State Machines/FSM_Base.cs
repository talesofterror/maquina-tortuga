using UnityEngine;

public abstract class FSM_Base
{
  protected FSM_BaseController c;

  public FSM_Base superState;
  public FSM_Base subState;

  public FSM_Base(FSM_BaseController controller, Coroutine laserCoroutine = null)
  {
    this.c = controller;
  }

  public abstract void Enter();   // Initialization logic
  public virtual void Update()
  {
    subState?.Update();
    Loop();
  }  
  public abstract void Loop();

  public abstract void Exit();    // Cleanup logic

  public void SetSubState(FSM_Base _subState)
  {
    subState = _subState;
    // subState?.SetSuperState(this);
    subState?.Enter();
  }
  protected void SetSuperState(FSM_Base _superState)
  {
    superState = _superState;
  }
}
