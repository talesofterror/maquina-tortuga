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
    
    ((FSM_RostroController)controller).laserGenerator.StartCoroutine(((FSM_RostroController)controller).laserGenerator.ExtendLaser());

  }

  public override void Update()
  {
    // Pressing P will switch to Attack state
    if (Input.GetKeyDown(KeyCode.P))
    {
      controller.SwitchState(((FSM_RostroController)controller).Idle);
    }
  }

  public override void Exit()
  {
    // Debug.Log("Exiting Attack State");
    ((FSM_RostroController)controller).laserGenerator.StartCoroutine(((FSM_RostroController)controller).laserGenerator.RetractLaser());
  }
}
