using UnityEngine;
using System.Collections;

public class IronGolem_FSM_State_Attack : FSM_Base
{

  IronGolem_FSM_Controller controller;
  Vector3 direction;

  Coroutine attackBehavior;
  bool animationActive = false;
  bool pauseRotation = false;

  public IronGolem_FSM_State_Attack(IronGolem_FSM_Controller c) : base(c)
  {
    this.controller = c;
  }

  public override void Enter()
  {
    controller.animalScript.attacking = true;
  }
  public override void Loop()
  {
    bool targetTooFar = Vector3.Distance(controller.transform.position, controller.focus.transform.position) > controller.animalScript.forgetDistance;
    bool targetInAttackRange =
        Vector3.Distance(controller.transform.position, controller.focus.transform.position) <= controller.navMeshAgent.stoppingDistance;

    DetectTargetDistance(targetTooFar, targetInAttackRange);

    UpdateAnimation();
    UpdateRotation();

  }

  public override void Exit()
  {
    controller.animalScript.attacking = false;
    animationActive = false;
    if (attackBehavior != null)
    {
      controller.StopCoroutine(attackBehavior);
      attackBehavior = null;
    }
    controller.animator.SetBool("isAttacking", false);
  }

  private void DetectTargetDistance(bool targetTooFar, bool targetInAttackRange)
  {
    if (attackBehavior != null) return;
    if (targetTooFar)
    {
      controller.navMeshAgent.ResetPath();
      controller.SwitchState(controller.state_Patrol);
    }

    if (targetInAttackRange)
    {
      Debug.Log($"{controller.transform.name} is in attacking range!");
      if (attackBehavior == null)
      {
        attackBehavior = controller.StartCoroutine(AttackCoroutine());
      }
    }
    else
    {
      controller.SwitchState(controller.state_Pursue);
    }
  }

  IEnumerator AttackCoroutine()
  {
    animationActive = true;
    pauseRotation = true;

    yield return null;

    float currentAnimLength = controller.animator.GetCurrentAnimatorStateInfo(0).length;

    yield return new WaitForSeconds(controller.animalScript.smashDamageDelay);

    for (float t = 0; t < controller.animalScript.smashDamageDuration; t += Time.deltaTime)
    {
      DoPlayerDamage(castSmashSphere(controller.animalScript.smashRadius));
      yield return null;
    }
    pauseRotation = false;

    float remainingTime = currentAnimLength - (controller.animalScript.smashDamageDelay + controller.animalScript.smashDamageDuration);
    if (remainingTime > 0)
      yield return new WaitForSeconds(remainingTime);

    controller.rB.isKinematic = true;
    animationActive = false;

    yield return new WaitForSeconds(controller.animalScript.attackCooldown);

    attackBehavior = null;
  }


  bool castSmashSphere(float radius)
  {
    return Physics.CheckSphere(
      controller.animalScript.smashDetector.gameObject.transform.position,
      radius,
      PLAYERSingleton.i.layerMask_Player);
  }

  public void DoPlayerDamage(bool contactMade)
  {
    if (contactMade)
    {
      PLAYERSingleton.i.playerHealth.TakeDamage(10);
      PLAYERSingleton.i.playerHealth.DamageKnockback(
         controller.rB.position,
         controller.animalScript.smashKnockbackForce
      );
    }
  }

  void UpdateRotation()
  {
    if (pauseRotation) return;
    direction = controller.transform.position - controller.focus.transform.position;
    if (direction.sqrMagnitude > 0)
      controller.transform.rotation = Quaternion.LookRotation(-direction);
  }

  private void UpdateAnimation()
  {
    if (animationActive)
    {
      controller.animator.SetBool("isAttacking", true);
    }
    else
    {
      controller.animator.SetBool("isAttacking", false);
    }
  }

}
