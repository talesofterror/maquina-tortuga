using UnityEngine;

public class FSM_RostroDeactivated : FSM_Base
{
  public FSM_RostroDeactivated(FSM_RostroController controller) : base(controller) { }

  public override void Enter()
  {
    Debug.Log("Entering Deactivated State");
    foreach (AnimationState state in controller.anim)
    {
      // state.speed = 0; // Pause all animations
    }
  }

  public override void Update()
  {
    // // Pressing P will switch back to Idle state
    // if (Input.GetKeyDown(KeyCode.P))
    // {
    //   controller.SwitchState(controller.Idle);
    // }
  }

  public override void Exit()
  {
    Debug.Log("Exiting Deactivated State");
  }
}
