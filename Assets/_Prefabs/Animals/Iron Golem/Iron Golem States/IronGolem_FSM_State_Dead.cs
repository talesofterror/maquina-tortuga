using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class IronGolem_FSM_State_Dead : FSM_Base
{
  IronGolem_FSM_Controller controller;

  Coroutine resurrectionTimer;

  public IronGolem_FSM_State_Dead (IronGolem_FSM_Controller c) : base (c)
  {
    this.controller = c;
  }

  public override void Enter()
  {
    Debug.Log($"{controller.transform.name} died.");
    controller.animator.SetTrigger("Die");
    // controller.focus = null;
    controller.animalScript.dead = true;
    if (controller.animalScript.resurrectable) resurrectionTimer = controller.StartCoroutine(ResurrectionCountdown());
  }

  public override void Loop()
  {
    
  }

  public override void Exit()
  {
    
  }

  IEnumerator ResurrectionCountdown ()
  {
    yield return new WaitForSeconds(controller.animalScript.resurrectionDelay);

    controller.animalScript.hp = controller.animalScript.stats.maxHP;
    controller.animalScript.dead = false;
    // controller.animator.SetBool("isDead", false);
    controller.animator.ResetTrigger("Die");
    controller.SwitchState(controller.state_Patrol);
  }

}
