using UnityEngine;
using PixelCrushers.DialogueSystem;
using System.Collections;

public class FSM_PlayerState_Dead : FSM_PlayerStateBase
{

  public FSM_PlayerState_Dead(FSM_PlayerStateController c) : base(c) { }

  Coroutine resurrectionTimerCoroutine;

  public override void Enter()
  {
    Debug.Log("Player is dead!");
    UISingleton.i.debug.pushMessage("x_x;;");

    PLAYERSingleton.i.inputDisabled = true;
    PLAYERSingleton.i.ignoreModeChange = true;
    CAMERASingleton.i.cameraSwitchDisabled = true;
    if (DialogueManager.IsConversationActive) DialogueManager.StopAllConversations();

    if (resurrectionTimerCoroutine == null)
    {
      resurrectionTimerCoroutine = PLAYERSingleton.i.StartCoroutine(ResurrectionTimer());
    }

    PLAYERSingleton.i.animations.animator.SetBool("Dead", true);
  }

  IEnumerator ResurrectionTimer()
  {
    yield return new WaitForSeconds(PLAYERSingleton.i.playerHealth.deathDuration);
    yield return null;
    PLAYERSingleton.i.stateController.SwitchState(PLAYERSingleton.i.stateController.state_Normal);
  }

  public override void Exit()
  {
    resurrectionTimerCoroutine = null;
    PLAYERSingleton.i.inputDisabled = false;
    PLAYERSingleton.i.ignoreModeChange = false;
    CAMERASingleton.i.cameraSwitchDisabled = false;
    PLAYERSingleton.i.transform.position = GameObject.FindWithTag("Respawn").transform.position;
    PLAYERSingleton.i.transform.rotation = GameObject.FindWithTag("Respawn").transform.rotation;
    PLAYERSingleton.i.animations.animator.SetBool("Dead", false);
    PLAYERSingleton.i.playerHealth.ResetHealth();
  }

  public override void Loop()
  {

  }

}
