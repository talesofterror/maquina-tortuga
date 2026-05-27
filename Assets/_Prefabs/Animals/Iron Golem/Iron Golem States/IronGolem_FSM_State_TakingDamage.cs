using UnityEngine;
using System.Collections;
using PixelCrushers.DialogueSystem;

public class IronGolem_FSM_State_TakingDamage : FSM_Base
{
  IronGolem_FSM_Controller controller;
  private Coroutine takingDamageBehavior;
  private int damageAmount;

  public IronGolem_FSM_State_TakingDamage(IronGolem_FSM_Controller c) : base(c)
  {
    controller = c;
  }

  public override void Enter()
  {
    TakeDamage(damageAmount, controller.focus);
  }
  public override void Loop()
  {

  }
  public override void Exit()
  {

  }

  public override void Option(int option)
  {
    damageAmount = option;
  }

  public void TakeDamage(int amount, GameObject focus)
  {
    controller.focus = focus;
    controller.animalScript.isTakingDamage = true;
    controller.animalScript.hp = controller.animalScript.hp - amount;
    Debug.Log("Iron Golem took " + amount + " damage!");
    DialogueManager.ShowAlert("<b>Iron Golem</b> took <color=#10ccff><b>" + amount + "</color> damage!</b>");

    if (controller.animalScript.hp <= 0)
    {
      controller.SwitchState(controller.state_Dead);
      return;
    }

    controller.animalScript.isTakingDamage = true;

    if (takingDamageBehavior == null)
    {
      takingDamageBehavior = controller.StartCoroutine(TakingDamageCoroutine());
    }

    IEnumerator TakingDamageCoroutine()
    {
      controller.animator.SetTrigger("Knockback");
      yield return new WaitForSeconds(2);
      controller.animator.ResetTrigger("Knockback");

      takingDamageBehavior = null;
      controller.animalScript.isTakingDamage = false;
      controller.SwitchState(controller.state_Pursue);
    }
  }

}
