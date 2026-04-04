using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;

public class FSM_PlayerState_Normal : FSM_PlayerStateBase
{

  public FSM_PlayerState_Normal(FSM_PlayerStateController c) : base(c) { }
  public override void Enter()
  {

  }
  public override void Exit()
  {
    currentInteractable = null;
  }

  public override void Update()
  {
    // _currentSubState?.Update();
    lookForInteractions();
    // if (hasSightedInteractable) listenForInteractionInput();
  }


  Interactable currentInteractable;
  public float raycastDistance = 10f;
  RaycastHit rayHitInteractable;

  [HideInInspector]
  public bool hasSightedInteractable;

  void lookForInteractions()
  {
    // Null Check
    if (hasSightedInteractable && (GMSingleton.i.currentInteraction == null || GMSingleton.i.currentInteraction.i == null))
    {
      hasSightedInteractable = false;
      GMSingleton.i.currentInteraction = null;
    }

    bool rayHit = Physics.Raycast(PLAYERSingleton.i.transform.position + new Vector3(0, 0.5f, 0),
                      PLAYERSingleton.i.transform.forward,
                      out rayHitInteractable,
                      PLAYERSingleton.i.interactionSightDistance);

    if (rayHit)
    {
      if (rayHitInteractable.transform.CompareTag("Interactable"))
      {
        InteractableDetected();
      }
    }
    else
    {
      if (currentInteractable != null)
      {
        currentInteractable.SetSightState(Interactable.SightState.Unsighted);
        currentInteractable = null;
        if (GMSingleton.i.currentInteraction != null) GMSingleton.i.currentInteraction = null;
      }
    }
  }

  private void InteractableDetected()
  {
    if (currentInteractable is null)
    {
      currentInteractable = rayHitInteractable.transform.gameObject.GetComponent<Interactable>();

      if (currentInteractable.sightState == Interactable.SightState.Unsighted)
      {
        currentInteractable.SetSightState(Interactable.SightState.Sighted);
      }
    }
    if (currentInteractable.sightState == Interactable.SightState.Sighted)
    {
      listenForReachability();
    }
  }

  private void listenForReachability()
  {
    if (currentInteractable is null) { return; }
    if (currentInteractable.CanReach())
    {

      if (currentInteractable.reachState != Interactable.ReachState.Reachable)
      {
        currentInteractable.SetReachState(Interactable.ReachState.Reachable);
        currentInteractable.SetAsCurrentReachableInteraction();
        Debug.Log("Listening for intraction input on" + currentInteractable.name);
      }
      else
      {
        listenForInteractionInput();
      }
    }
    else
    {
      currentInteractable.reachState = Interactable.ReachState.Unreachable;
      // Debug.Log("Player is listening for reachability of " + currentInteractable.name);
    }
  }

  void listenForInteractionInput()
  {
    if (GMSingleton.i.inputManager.interaction.WasPressedThisFrame())
    {
      PLAYERSingleton.i.stateController.SwitchState(PLAYERSingleton.i.stateController.state_Interact);
    }
  }
}
