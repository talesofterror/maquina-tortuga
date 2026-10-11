using System.Collections;
using UnityEngine;

public class Clone_State_Idle : FSM_Base
{

  Clone_StateController controller;
  Coroutine facingCoroutine;
  public float facingInterpolationDuration = 0.25f;

  public Clone_State_Idle(Clone_StateController c) : base(c)
  {
    this.controller = c;
  }

  public override void Enter()
  {
    controller.cachedDirection = controller.transform.forward;
  }

  Vector3 targetDirection;
  public override void Loop()
  {
    AttentionBehavior();
  }

  private void AttentionBehavior()
  {
    if (controller.attentionTarget == null)
    {
      if (facingCoroutine != null)
      {
        controller.StopCoroutine(facingCoroutine);
        facingCoroutine = null;
      }

      facingCoroutine = controller.StartCoroutine(InterpolateFacing(controller.cachedDirection));
      return;
    }

    targetDirection = controller.attentionTarget.transform.position - controller.transform.position;
    if (targetDirection.sqrMagnitude <= 0.01f)
    {
      return;
    }

    if (facingCoroutine == null)
    {
      facingCoroutine = controller.StartCoroutine(InterpolateFacing(targetDirection));
    }
    else
    {
      controller.SetFacingDirection(controller.attentionTarget.transform.position);
    }
  }

  IEnumerator InterpolateFacing(Vector3 targetDirection)
  {
    yield return controller.StartCoroutine(UTILITY.InterpolateDirections(
      controller.transform,
      controller.transform.forward,
      targetDirection,
      facingInterpolationDuration,
      false));

    facingCoroutine = null;
  }

  public override void Exit()
  {
    if (facingCoroutine != null)
    {
      controller.StopCoroutine(facingCoroutine);
      facingCoroutine = null;
    }
  }
}
