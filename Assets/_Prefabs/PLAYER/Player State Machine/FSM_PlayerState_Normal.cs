using UnityEngine;

public class FSM_PlayerState_Normal : FSM_PlayerStateBase
{

  public FSM_PlayerState_Normal(FSM_PlayerStateController c) : base(c) { }
  public override void Enter()
  {
    
  }
  public override void Exit()
  {
    
  }

  public override void Update()
  {
    _currentSubState?.Update();
  }

}
