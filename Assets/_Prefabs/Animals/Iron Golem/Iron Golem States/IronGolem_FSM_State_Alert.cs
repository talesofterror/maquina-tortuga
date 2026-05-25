using UnityEngine;

public class IronGolem_FSM_State_Alert : FSM_Base
{
  IronGolem_FSM_Controller controller;
  Vector3 direction;
  float timer = 0;
  bool alertTimedOut;

  public IronGolem_FSM_State_Alert(IronGolem_FSM_Controller c) : base(c)
  {
    this.controller = c;
  }

  public override void Enter()
  {
    Debug.Log($"{controller.transform.name} saw you.");

    alertTimedOut = false;
    timer = Time.time;
  }

  public override void Loop()
  {
    if (controller.focus != null)
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
    }

    if (Time.time > timer + 2)
    {
      controller.SwitchState(controller.state_Pursue);
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
