using UnityEngine;

public class IronGolem_FSM_State_Searching : FSM_Base
{
  IronGolem_FSM_Controller controller;

  public IronGolem_FSM_State_Searching(IronGolem_FSM_Controller c) : base(c)
  {
    this.controller = c;
  }

  public override void Enter()
  {
    Debug.Log($"{controller.transform.name} is searching.");

  }

  public override void Loop()
  {
    
  }


  public override void Exit()
  {
    
  }
}
