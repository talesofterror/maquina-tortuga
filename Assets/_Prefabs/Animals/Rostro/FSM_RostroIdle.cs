using UnityEngine;

public class FSM_RostroIdle : FSM_Base
{
  public FSM_RostroIdle(FSM_RostroController controller) : base(controller)
  {
    // Constructor logic if needed
    // Might be useful for initializing state-specific code
  }

  public override void Enter()
  {

    // Debug.Log("Entering Idle State");

  }

  public override void Loop()
  {
    // Pressing P will switch to Idle state
    if (Input.GetKeyDown(KeyCode.P))
    {
      // controller.SwitchState(controller.Attack);
      c.SwitchState(((FSM_RostroController)c).Attack);
    }
  }

  public override void Exit()
  {
    // Debug.Log("Exiting Idle State");
  }
}
