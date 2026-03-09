using UnityEngine;

public class FSM_RostroDeactivated : FSM_Base
{
  public FSM_RostroDeactivated(FSM_RostroController controller) : base(controller) { }

  public override void Enter()
  {
    Debug.Log("Entering Deactivated State");
    foreach (AnimationState state in controller.anim)
    {
      state.speed = 0; // Pause all animations
    }
    
    controller.laserGenerator.StopAllCoroutines(); // Stop any ongoing laser actions
  }

  public override void Update()
  {

  }

  public override void Exit()
  {
    foreach (AnimationState state in controller.anim)
    {
      state.speed = 1; // Resume all animations
    }
    Debug.Log("Exiting Deactivated State");
  }
}
