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
    listenForInteractionInput();
  }


  RaycastHit rayHitInteractable;
  Interactable currentInteractable;
  public float raycastDistance = 10f;
  // [HideInInspector]
  public bool isTargettingInteractable;

  void listenForInteractionInput()
  {
    castRay();
    
    if (GMSingleton.i.inputManager.interaction.WasPressedThisFrame())
    {
      if (isTargettingInteractable)
      {
        if (canInteract(GMSingleton.i.currentInteraction.transform.position))
        {
          GMSingleton.Interact(GMSingleton.i.currentInteraction.type);
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

  void castRay()
  {
    if (
    Physics.Raycast(
        PLAYERSingleton.i.transform.position,
        PLAYERSingleton.i.transform.forward,
        out rayHitInteractable,
        raycastDistance)
)
    {
      if (rayHitInteractable.transform.CompareTag("Interactable")
          && !isTargettingInteractable)
      {
        currentInteractable = rayHitInteractable.transform.GetComponentInParent<Interactable>();
        if (currentInteractable != null)
        {
          isTargettingInteractable = true;
          currentInteractable.Focused();
        }
      }
    }
    else
    {
      isTargettingInteractable = false;
      currentInteractable = null;
    }
  }

}
