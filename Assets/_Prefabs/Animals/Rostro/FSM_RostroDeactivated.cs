using UnityEngine;

public class FSM_RostroDeactivated : FSM_Base
{
  public FSM_RostroDeactivated(FSM_RostroController controller) : base(controller) { }

  public override void Enter()
  {
    Debug.Log("Entering Deactivated State");
    foreach (AnimationState state in ((FSM_RostroController)c).anim)
    {
      state.speed = 0; // Pause all animations
    }
    
    ((FSM_RostroController)c).laserGenerator.StopAllCoroutines(); // Stop any ongoing laser actions
  }

  public override void Loop()
  {

  }

  public override void Exit()
  {
    foreach (AnimationState state in ((FSM_RostroController)c).anim)
    {
      state.speed = 1; // Resume all animations
    }
    Debug.Log("Exiting Deactivated State");
  }
}
