using UnityEngine;

public class IronGolem_FSM_State_Alert : FSM_Base
{
  IronGolem_FSM_Controller controller;
  Vector3 direction;

  public IronGolem_FSM_State_Alert(IronGolem_FSM_Controller c) : base(c)
  {
    this.controller = c;
  }

  public override void Enter()
  {
    Debug.Log($"{controller.transform.name} saw you.");
    controller.animator.SetBool("isRunning", false);
  }

  public override void Loop()
  {
    Vector3 lookTarget = controller.focus.transform.position;
    lookTarget.y = controller.transform.position.y;
    direction = (controller.transform.position - lookTarget).normalized;

    if (Vector3.Distance(controller.transform.position, controller.focus.transform.position) > controller.forgettingDistance)
    {
      Debug.Log($"{controller.name} + alert Loop(): distance check");
      controller.SwitchState(controller.state_Patrol);
      return;
    }

    UpdateRotation();
  }


  public override void Exit()
  {
    controller.state_Patrol.interruptPosition = controller.transform.position;
  }

  void UpdateRotation()
  {
    if (direction.sqrMagnitude > 0)
      controller.transform.rotation = Quaternion.LookRotation(-direction);
  }

}
