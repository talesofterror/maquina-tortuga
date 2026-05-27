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
    PLAYERSingleton.i.animations.animator.SetBool(
      PLAYERSingleton.i.animations.FightStance,
      true);
  }
  public override void Exit()
  {
    PLAYERSingleton.i.animations.animator.SetBool(
      PLAYERSingleton.i.animations.FightStance,
      false);
    currentWeapon.Withdraw();
    currentWeapon = null;
  }

  public override void Loop()
  {
    listenForAttackInput();
  }

  private void listenForAttackInput()
  {
    if (GMSingleton.i.inputManager.attack.WasReleasedThisFrame())
    {
      if (PLAYERSingleton.i.playerIsAttacking == false)
      {
        PLAYERSingleton.i.playerIsAttacking = true;
        currentWeapon.Attack();
      }
    }
  }

  private void DealDamage()
  {
    
  }
}
