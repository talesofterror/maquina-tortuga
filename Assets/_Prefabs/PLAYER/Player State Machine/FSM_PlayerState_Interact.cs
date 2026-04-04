using UnityEngine;

public class FSM_PlayerState_Interact : FSM_PlayerStateBase
{
  public FSM_PlayerState_Interact(FSM_PlayerStateController c) : base(c) { }

  public override void Enter()
  {
    Debug.Log("Entering Interact Mode");
    if (GMSingleton.i.currentInteraction is null) 
      Debug.Log("Player could not interact -> GM.currentInteraction is null!");
    else Interact(GMSingleton.i.currentInteraction.type);
    controller.SwitchState(controller.state_Normal);
  }
  public override void Exit()
  {
    Debug.Log("Exiting Interact Mode");
    // GMSingleton.i.currentInteraction = null;
  }
  public override void Update()
  {

  }

  public static void Interact(InteractionType type)
  {
    if (type == InteractionType.Warp)
    {
      GMSingleton.i.currentInteraction.i.gameObject.GetComponent<SceneLoader>().loadLevel();
    }
  }
}
