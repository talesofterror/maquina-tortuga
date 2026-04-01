using UnityEngine;

public class FSM_PlayerState_Fight : FSM_PlayerStateBase
{

  public FSM_PlayerState_Fight(FSM_PlayerStateController c) : base(c) { }

  public override void Enter()
  {
    PLAYERSingleton.i.playerWeapons.weapon.SetActive(true);
    PLAYERSingleton.i.playerMode = PlayerMode.Fight;
    PLAYERSingleton.i.animations.animator.SetBool(
        PLAYERSingleton.i.animations.FightStance,
        true
    );
  }
  public override void Exit()
  {
    PLAYERSingleton.i.playerWeapons.weapon.SetActive(false);
    // PLAYERSingleton.i.playerMode = mode;
    PLAYERSingleton.i.animations.animator.SetBool(
        PLAYERSingleton.i.animations.FightStance,
        false
    );
    UISingleton.i.debug.pushMessage("** Normal Mode activated.");
  }

  public override void Update()
  {
    if (GMSingleton.i.inputManager.attack.WasReleasedThisFrame())
    {
      PLAYERSingleton.i.playerIsAttacking = true;
      string id = "SlashTrigger";
      PLAYERSingleton.i.StartCoroutine(
          PLAYERSingleton.i.animations.WaitAndFreeze(PLAYERSingleton.i.animations.stateInfo.length, id)
      );
      PLAYERSingleton.i.Invoke("SetPlayerIsAttackingFalse", PLAYERSingleton.i.animations.stateInfo.length);
    }
  }
}
