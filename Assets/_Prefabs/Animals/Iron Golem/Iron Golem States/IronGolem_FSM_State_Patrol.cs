using System.Collections;
using UnityEngine;

public class IronGolem_FSM_State_Patrol : FSM_Base
{
  IronGolem_FSM_Controller controller;

  private bool running;
  private Vector3 direction;
  Coroutine movementMotorCoroutine = null;
  private IronGolem_FSM_State_Searching searchingSubState;

  public IronGolem_FSM_State_Patrol(IronGolem_FSM_Controller c) : base(c)
  {
    this.controller = c;
    searchingSubState = new IronGolem_FSM_State_Searching(controller);
  }

  public override void Enter()
  {
    Debug.Log($"{controller.transform.name} entered patrol mode");
    SetSubState(searchingSubState);
  }

  public override void Loop()
  {
    if (movementMotorCoroutine == null)
    {
      movementMotorCoroutine = controller.StartCoroutine(IEMovementMotor());
    }

    UpdateAnimation();
    UpdateRotation();
  }


  public override void Exit()
  {
    controller.StopAllCoroutines();
    searchingSubState = null;
  }

  IEnumerator IEMovementMotor(Vector3? interruptVector = null) // nullable value type
  {

    int activeIndex;

    if (controller.waypointSystem.activeWaypointTarget == null)
    {
      activeIndex = 0;
    }
    else
    {
      activeIndex = controller.waypointSystem.activeWaypointTarget.index;
    }

    for (int i = activeIndex; i < controller.waypointSystem.waypoints.Count; i++)
    {
      Vector3 originVector;
      if (interruptVector.HasValue)
      {
        originVector = interruptVector.GetValueOrDefault(); // nullable value type
        interruptVector = null;
      }
      else originVector = controller.waypointSystem.waypoints[i].location;

      controller.waypointSystem.activeWaypointTarget = controller.waypointSystem.waypoints[i];
      running = true;
      direction = (
          originVector - controller.waypointSystem.waypoints[i].neighborNext.location
      ).normalized;
      float distance = Vector3.Distance(
          originVector,
          controller.waypointSystem.waypoints[i].neighborNext.location
      );
      float calculatedSpeed = distance / controller.animalScript.speed;

      for (float j = 0; j < 1; j += Time.deltaTime / calculatedSpeed)
      {
        controller.rB.MovePosition(
            Vector3.Lerp(originVector, controller.waypointSystem.waypoints[i].neighborNext.location, j)
        );
        yield return null;
      }
      running = false;
      yield return new WaitForSeconds(1);
    }
    movementMotorCoroutine = null;
    controller.waypointSystem.activeWaypointTarget = null;
  }

  void UpdateAnimation()
  {
    if (running)
      controller.animator.SetBool("isRunning", true);
    else if (!running)
    {
      controller.animator.SetBool("isRunning", false);
    }
  }

  void UpdateRotation()
  {
    if (direction.sqrMagnitude > 0)
      controller.transform.rotation = Quaternion.LookRotation(-direction);
  }

}
