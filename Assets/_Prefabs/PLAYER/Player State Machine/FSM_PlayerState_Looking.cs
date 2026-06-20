using PixelCrushers.DialogueSystem;
using UnityEngine;

public class FSM_PlayerState_Looking : FSM_PlayerStateBase
{
  // Start is called once before the first execution of Update after the MonoBehaviour is created
  public FSM_PlayerState_Looking(FSM_PlayerStateController c) : base(c) { }

  public override void Enter()
  {
    UISingleton.i.debug.pushMessage("~_________~~", "#acafff");
    UISingleton.i.debug.pushMessage("~~~~___~~~~~", "#acafff");
    UISingleton.i.debug.pushMessage("_~((o_o)~~__", "#acafff");
    UISingleton.i.debug.pushMessage("~~~~---~~~~~", "#acafff");
    UISingleton.i.debug.pushMessage("~----------~", "#acafff");
    // Debug.Log("~_________~~");
    // Debug.Log("~~~~___~~~~~");
    // Debug.Log("_~((o_o)~~__");
    // Debug.Log("~~~~---~~~~~");
    // Debug.Log("~----------~");
  }

  public override void Exit()
  {
    sightedInteractable = null;
  }

  public override void Loop()
  {
    lookForInteractions();
  }

  Interactable sightedInteractable;
  public float raycastDistance = 10f;
  RaycastHit rayHitInteractable;

  [HideInInspector]
  public bool hasSightedInteractable;

  Usable sightedUsable;

  void lookForInteractions()
  {
    if (DialogueManager.isConversationActive) return;
    // Null Check
    if (hasSightedInteractable && (GMSingleton.i.currentInteraction == null || GMSingleton.i.currentInteraction.i == null))
    {
      hasSightedInteractable = false;
    }

    // bool rayHit = Physics.Raycast(PLAYERSingleton.i.transform.position + new Vector3(0, 0.5f, 0),
    bool rayHit = Physics.SphereCast(PLAYERSingleton.i.transform.position + new Vector3(0, 0.5f, 0),
                      0.3f,
                      PLAYERSingleton.i.transform.forward,
                      out rayHitInteractable,
                      PLAYERSingleton.i.interactionSightDistance);

    if (rayHit)
    {

      // * Listen for Interactable.Sighted

      if (rayHitInteractable.transform.CompareTag("Interactable"))
      {
        listenForSight();
      }
    }
    else
    {
      if (sightedInteractable != null)
      {
        sightedInteractable.SetSightState(Interactable.SightState.Unsighted);
        sightedInteractable = null;
        sightedUsable = null;
        if (GMSingleton.i.currentInteraction != null) GMSingleton.i.currentInteraction = null;
      }
    }
  }

  private void listenForSight()
  {
    if (sightedInteractable == null)
    {
      GMSingleton.i.currentInteraction = new Interaction(rayHitInteractable.transform.gameObject.GetComponent<Interactable>());
      sightedInteractable = GMSingleton.i.currentInteraction.i;

      if (sightedInteractable.sightState == Interactable.SightState.Unsighted)
      {
        sightedInteractable.SetSightState(Interactable.SightState.Sighted);
      }
    }
    if (sightedInteractable.sightState == Interactable.SightState.Sighted)
    {
      sightedUsable = sightedInteractable.gameObject.GetComponent<Usable>();
      listenForReachability();
    }
  }

  private void listenForReachability()
  {
    if (sightedInteractable == null) { return; }
    if (sightedInteractable.CanReach())
    {

      // Don't modify UI strings here; Selector_CustomRaycast_Camera updates text and colors each frame.

      if (sightedInteractable.reachState != Interactable.ReachState.Reachable)
      {
        sightedInteractable.SetReachState(Interactable.ReachState.Reachable);
        sightedInteractable.SetAsCurrentReachableInteraction();
        Debug.Log("Listening for intraction input on" + sightedInteractable.name);
      }
      else
      {
        listenForInteractionInput();
      }
    }
    else
    {
      sightedInteractable.reachState = Interactable.ReachState.Unreachable;
      // Debug.Log("Player is listening for reachability of " + currentInteractable.name);
    }
  }

  void listenForInteractionInput()
  {
    if (GMSingleton.i.inputManager.interaction.WasPressedThisFrame())
    {
      // PLAYERSingleton.i.stateController.SwitchState(PLAYERSingleton.i.stateController.state_Interact);
      Interactable.Interact(sightedInteractable.type);
    }
  }
}
