using Unity.VisualScripting;
using UnityEngine;

public class FSM_PlayerState_Normal : FSM_PlayerStateBase
{

  public FSM_PlayerState_Normal(FSM_PlayerStateController c) : base(c) { }
  public override void Enter()
  {

  }
  public override void Exit()
  {

  }

  public override void Update()
  {
    // _currentSubState?.Update();
    lookForInteractions();
    if (isTargettingInteractable) listenForInteractionInput();
  }


  RaycastHit rayHitInteractable;
  // Interactable currentInteractable;
  public float raycastDistance = 10f;
  
  [HideInInspector]
  public bool isTargettingInteractable;

  void lookForInteractions()
  {
    // Null Check
    if (isTargettingInteractable && (GMSingleton.i.currentInteraction == null || GMSingleton.i.currentInteraction.i == null))
    {
      isTargettingInteractable = false;
      GMSingleton.i.currentInteraction = null;
    }

    if (
    Physics.Raycast(PLAYERSingleton.i.transform.position + new Vector3(0, 0.5f, 0),
                      PLAYERSingleton.i.transform.forward,
                      out rayHitInteractable,
                      PLAYERSingleton.i.interactionSightDistance))
    {
      if (rayHitInteractable.transform.CompareTag("Interactable"))
      {
        if (!isTargettingInteractable)
        {
          Interactable interaction = rayHitInteractable.transform.GetComponent<Interactable>();
          if (interaction != null)
          {
            interaction.Focused();
            Debug.Log("Currently looking at " + interaction._name);
            isTargettingInteractable = true;
          }
        }
      }
      else
      {
        isTargettingInteractable = false;
        GMSingleton.i.currentInteraction = null;
      }
    }
    else
    {
      isTargettingInteractable = false;
      GMSingleton.i.currentInteraction = null;
    }
  }

  void listenForInteractionInput()
  {
    if (GMSingleton.i.inputManager.interaction.WasPressedThisFrame())
    {
      if (isTargettingInteractable)
      {
        // Null Check
        if (GMSingleton.i.currentInteraction == null || GMSingleton.i.currentInteraction.i == null)
        {
          isTargettingInteractable = false;
          return;
        }

        Debug.Log("interaction button pressed");
        Debug.Log(GMSingleton.i.currentInteraction);
        Debug.Log(GMSingleton.i);
        if (canInteract(GMSingleton.i.currentInteraction.i.transform.position))
        {
          Debug.Log("Can interacting with " + GMSingleton.i.currentInteraction.name);
          PLAYERSingleton.i.stateController.SwitchState(PLAYERSingleton.i.stateController.state_Interact);
        }
      }
    }
  }

  public bool canInteract(Vector3 targetPosition)
  {
    if (Vector3.Distance(PLAYERSingleton.i.transform.position, targetPosition) < raycastDistance
        && PLAYERSingleton.i.stateController.currentState != PLAYERSingleton.i.stateController.state_Fight)
    {
      return true;
    }
    else
      return false;
  }



}
