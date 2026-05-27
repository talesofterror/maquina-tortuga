using PixelCrushers.DialogueSystem;
using UnityEngine;

public class FSM_PlayerState_Interact : FSM_PlayerStateBase
{
  public FSM_PlayerState_Interact(FSM_PlayerStateController c) : base(c) { }

  public override void Enter()
  {
    // * pause movement
    // * stop animations
    // * timeScale = 0?

    PLAYERSingleton.i.movementDisabled = true;
    CAMERASingleton.i.cameraSwitchDisabled = true;
    Debug.Log(">> Press Submit to Interact.");
    Debug.Log(">> Press Cancel to Exit.");

  }
  public override void Exit()
  {

    PLAYERSingleton.i.movementDisabled = false;
    CAMERASingleton.i.cameraSwitchDisabled = false;
    Debug.Log("Exiting Interact Mode");
    // GMSingleton.i.currentInteraction = null;
  }
  public override void Loop()
  {
    if (GMSingleton.i.inputManager.ui_Submit.WasPressedThisFrame())
    {
      Debug.Log("Entering Interact Mode");
      if (GMSingleton.i.currentInteraction is null)
        Debug.Log("Player could not interact -> GM.currentInteraction is null!");
      else Interact(GMSingleton.i.currentInteraction.type);
      controller.SwitchState(controller.state_Normal);
    }
    if (GMSingleton.i.inputManager.ui_Cancel.WasPressedThisFrame())
    {
      PLAYERSingleton.i.stateController.SwitchState(PLAYERSingleton.i.stateController.state_Normal);
    }
  }

  public static void Interact(InteractionType type)
  {
    if (type == InteractionType.Warp)
    {
      GMSingleton.i.currentInteraction.i.gameObject.GetComponent<SceneLoader>().loadLevel();
    }
    if (type == InteractionType.Friend)
    {
      PLAYERSingleton.i.dialogueSelector.UseCurrentSelection();
      // DialogueManager.StartConversation("Introductory exchange");
    }
  }
}
