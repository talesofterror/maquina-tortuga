using PixelCrushers.DialogueSystem;
using Unity.VisualScripting;
using UnityEngine;

public class FSM_PlayerState_Interact : FSM_PlayerStateBase
{

  public FSM_PlayerState_Interact(FSM_PlayerStateController c) : base(c) { }

  Interactable interactable;

  public override void Enter()
  {
    // * pause movement
    // * stop animations
    // * timeScale = 0?

    Debug.Log("Entering Interact Mode");

    PLAYERSingleton.i.inputDisabled = true;
    CAMERASingleton.i.cameraSwitchDisabled = true;

    interactable = PLAYERSingleton.i.sightedInteractable;
    if (interactable.type == InteractionType.Warp)
    {
      string warpDestination = interactable.gameObject.GetComponent<SceneLoader>().sceneToLoad;
      DialogueLua.SetVariable("StationName", warpDestination);
    }
  }
  public override void Exit()
  {
    interactable = null;
    PLAYERSingleton.i.inputDisabled = false;
    PLAYERSingleton.i.ignoreModeChange = true;
    CAMERASingleton.i.cameraSwitchDisabled = false;
    if (DialogueManager.IsConversationActive) DialogueManager.StopAllConversations();
    Debug.Log("Exiting Interact Mode");
    // GMSingleton.i.currentInteraction = null;
  }

  

  public override void Loop()
  {
    // if (GMSingleton.i.inputManager.ui_Submit.WasPressedThisFrame())
    // {
    //   if (GMSingleton.i.currentInteraction is null)
    //     Debug.Log("Player could not interact -> GM.currentInteraction is null!");
    //   else Interactable.Interact(GMSingleton.i.currentInteraction.type);
    // }
    if (GMSingleton.i.inputManager.ui_Cancel.WasPressedThisFrame())
    {
      Debug.Log("Interaction cancled");
      PLAYERSingleton.i.stateController.SwitchState(PLAYERSingleton.i.stateController.state_Normal);
    }
  }

}
