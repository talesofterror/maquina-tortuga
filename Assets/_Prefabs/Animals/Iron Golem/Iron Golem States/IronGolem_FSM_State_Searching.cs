using UnityEngine;

public class IronGolem_FSM_State_Searching : FSM_Base
{
  IronGolem_FSM_Controller controller;


  [SerializeField] float scanSpeed = 5;
  [SerializeField] float scanAngle = 45;
  float sightDistance = 10;
  float sightHeight = 1;
  RaycastHit playerRaycastHit;

  public IronGolem_FSM_State_Searching(IronGolem_FSM_Controller c) : base(c)
  {
    this.controller = c;
  }

  public override void Enter()
  {
    
  }

  public override void Loop()
  {
    if (PlayerDetected())
    {
      Debug.Log("player detected");
      controller.focus = PLAYERSingleton.i.gameObject;
      controller.SwitchState(controller.state_Alert);
    }
  }


  public override void Exit()
  {
    
  }


  bool PlayerDetected()
  {
    float angle = Mathf.Sin(Time.time * scanSpeed) * scanAngle;
    Vector3 direction = Quaternion.Euler(0, angle, 0) * controller.transform.forward;

    bool playerSighted = Physics.Raycast(
        controller.transform.position + new Vector3(0, sightHeight, 0),
        direction,
        out playerRaycastHit,
        sightDistance,
        controller.playerLayerMask
    );
    return playerSighted;
  }
}
