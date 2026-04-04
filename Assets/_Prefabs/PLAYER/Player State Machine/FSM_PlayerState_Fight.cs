using UnityEngine;
using UnityEngine.Rendering;

public class FSM_PlayerState_Fight : FSM_PlayerStateBase
{

  public FSM_PlayerState_Fight(FSM_PlayerStateController c) : base(c) { }

  public PlayerWeapon_BASE currentWeapon;

  public override void Enter()
  {
    currentWeapon = PLAYERSingleton.i.playerWeapons.currentWeapon;
    currentWeapon.Draw();
    currentWeapon.StartAnimation();
  }
  public override void Exit()
  {
    currentWeapon.Withdraw();
    currentWeapon.StopAnimation();
    currentWeapon = null;
  }

  public override void Update()
  {
    listenForAttackInput();
    
  }

  private static void listenForAttackInput()
  {
    if (GMSingleton.i.inputManager.attack.WasReleasedThisFrame())
    {
      PLAYERSingleton.i.playerIsAttacking = true;
      string id = "SlashTrigger";
      PLAYERSingleton.i.StartCoroutine(
          PLAYERSingleton.i.animations.PlayAndFreeze(PLAYERSingleton.i.animations.stateInfo.length, id)
      );
      // PLAYERSingleton.i.Invoke("SetPlayerIsAttackingFalse", PLAYERSingleton.i.animations.stateInfo.length);
    }
  }

  private void DealDamage ()
  {
    
  }
}
