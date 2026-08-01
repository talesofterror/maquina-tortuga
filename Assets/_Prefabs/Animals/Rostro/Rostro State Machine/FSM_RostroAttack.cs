using UnityEngine;

public class FSM_RostroAttack : FSM_Base
{
  private readonly FSM_RostroController controller;

  public FSM_RostroAttack(FSM_RostroController c) : base(c)
  {
    controller = c;
  }

  public override void Enter()
  {
    // controller.laserGenerator?.StartCoroutine(controller.laserGenerator.ExtendLaser());
  }

  private float yet = 0f;
  // private float interval = 0.3f;

  public override void Loop()
  {
    yet += Time.deltaTime;

    if (yet >= controller.animalRostro.stats.projectileInterval)
    {
      yet -= controller.animalRostro.stats.projectileInterval;

      foreach (IntervalProjector projector in controller.animalRostro.projector)
      {
        projector.FireProjectile();
      }
    }
  }

  public override void Exit()
  {
    // controller.laserGenerator?.StartCoroutine(controller.laserGenerator.RetractLaser());
  }
}
