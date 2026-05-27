using UnityEditor;
using UnityEngine;

public class IronGolem_FSM_State_Pursue : FSM_Base
{
  IronGolem_FSM_Controller controller;
  Vector3 direction;

  float pathUpdateFrequency;
  float pathUpdateTimer;

  public IronGolem_FSM_State_Pursue(IronGolem_FSM_Controller c) : base(c)
  {
    this.controller = c;
  }

  public override void Enter()
  {
    controller.animator.SetBool("isRunning", true);
    controller.navMeshAgent.isStopped = false;
    // controller.navMeshAgent.updatePosition = true;
    // controller.navMeshAgent.updateRotation = true;
    controller.rB.isKinematic = true;
  }

  public override void Loop()
  {
    // if (controller != null)
    // {
    Vector3 targetPos = PLAYERSingleton.i.transform.position;
    targetPos.y = controller.transform.position.y;
    direction = controller.transform.position - controller.focus.transform.position;

    // Throttle NavMesh destination updates
    pathUpdateTimer -= Time.deltaTime;
    if (pathUpdateTimer <= 0)
    {
      if (controller.navMeshAgent.isActiveAndEnabled)
      {
        controller.navMeshAgent.SetDestination(PLAYERSingleton.i.transform.position);
      }
      pathUpdateTimer = pathUpdateFrequency;
    }

    bool targetTooFar = Vector3.Distance(controller.transform.position, controller.focus.transform.position) > controller.animalScript.forgetDistance;
    bool targetInAttackRange =
        Vector3.Distance(controller.transform.position, controller.focus.transform.position) <= controller.navMeshAgent.stoppingDistance;

    if (targetTooFar)
    {
      controller.navMeshAgent.ResetPath();
      controller.SwitchState(controller.state_Patrol);
    }
    if (targetInAttackRange)
    {
      controller.SwitchState(controller.state_Attack);
    }

    UpdateRotation();
    // }
    // else
    // {
    //   controller.SwitchState(controller.state_Patrol);
    // }
  }
  public override void Exit()
  {
    controller.animator.SetBool("isRunning", false);
    controller.rB.isKinematic = false;
    // controller.navMeshAgent.updatePosition = false;
    // controller.navMeshAgent.updateRotation = false;
    controller.navMeshAgent.isStopped = true;
  }

  void UpdateRotation()
  {
    if (direction.sqrMagnitude > 0)
      controller.transform.rotation = Quaternion.LookRotation(-direction);
  }
}
