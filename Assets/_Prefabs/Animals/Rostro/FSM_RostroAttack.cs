using UnityEngine;

public class FSM_RostroAttack : FSM_Base
{

  public FSM_RostroAttack(FSM_RostroController controller) : base(controller)
  {
    // Constructor logic if needed
    // Might be useful for initializing state-specific code
  }

  public override void Enter()
  {

    // Debug.Log("Entering Attack State");
    
    ((FSM_RostroController)c).laserGenerator.StartCoroutine(((FSM_RostroController)c).laserGenerator.ExtendLaser());

  }

  public override void Loop()
  {
    // Pressing P will switch to Attack state
    if (Input.GetKeyDown(KeyCode.P))
    {
      c.SwitchState(((FSM_RostroController)c).Idle);
    }
  }

  public override void Exit()
  {
    // Debug.Log("Exiting Attack State");
    ((FSM_RostroController)c).laserGenerator.StartCoroutine(((FSM_RostroController)c).laserGenerator.RetractLaser());
  }
}
