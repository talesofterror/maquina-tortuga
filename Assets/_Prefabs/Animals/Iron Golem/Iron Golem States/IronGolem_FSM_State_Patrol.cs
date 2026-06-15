using System.Collections;
using UnityEngine;

public class IronGolem_FSM_State_Patrol : FSM_Base
{
  IronGolem_FSM_Controller controller;

  private WaypointSystem waypointSystem;
  private bool running;
  private Vector3 direction;
  public Vector3? interruptPosition;
  Coroutine movementMotorCoroutine = null;
  private IronGolem_FSM_State_Searching searchingSubState;

  public IronGolem_FSM_State_Patrol(IronGolem_FSM_Controller c) : base(c)
  {
    this.controller = c;
  }

  public override void Enter()
  {
    if (waypointSystem == null) waypointSystem = controller.GetComponentInChildren<WaypointSystem>();
    movementMotorCoroutine = null;
    if (subState == null)
    {
      searchingSubState = new IronGolem_FSM_State_Searching(controller);
      SetSubState(searchingSubState);
    }
  }

  public override void Loop()
  {
    if (movementMotorCoroutine == null)
    {
        movementMotorCoroutine = controller.StartCoroutine(IEMovementMotor(controller.transform.position));
    }
    UpdateAnimation();
    UpdateRotation();
  }


  public override void Exit()
  {
    if (movementMotorCoroutine != null) controller.StopCoroutine(movementMotorCoroutine);
    running = false;
    // controller.animator.SetBool("isRunning", false);
    SetSubState(null);
  }

  IEnumerator IEMovementMotor(Vector3? interruptVector = null) // nullable value type
  {

    int activeIndex;

    if (waypointSystem.activeWaypointTarget == null)
    {
      activeIndex = 0;
    }
    else
    {
      activeIndex = waypointSystem.activeWaypointTarget.index;
    }

    for (int i = activeIndex; i < waypointSystem.waypoints.Count; i++)
    {
      Vector3 originVector;
      if (interruptVector.HasValue)
      {
        originVector = interruptVector.GetValueOrDefault(); // nullable value type
        interruptVector = null;
      }
      else originVector = waypointSystem.waypoints[i].location;

      waypointSystem.activeWaypointTarget = waypointSystem.waypoints[i];
      running = true;
      direction = (
          originVector - waypointSystem.waypoints[i].neighborNext.location
      ).normalized;
      float distance = Vector3.Distance(
          originVector,
          waypointSystem.waypoints[i].neighborNext.location
      );
      float calculatedSpeed = distance / controller.animalScript.speed;

      for (float j = 0; j < 1; j += Time.deltaTime / calculatedSpeed)
      {
        controller.rB.MovePosition(
            Vector3.Lerp(originVector, waypointSystem.waypoints[i].neighborNext.location, j)
        );
        yield return null;
      }
      running = false;
      yield return new WaitForSeconds(1);
    }
    movementMotorCoroutine = null;
    waypointSystem.activeWaypointTarget = null;
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
