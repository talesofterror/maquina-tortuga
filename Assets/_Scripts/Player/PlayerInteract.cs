using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteract : MonoBehaviour
{
  public float raycastDistance = 10f;
  RaycastHit rayHitInteractable;
  [HideInInspector] public bool isTargettingInteractable;

  void FixedUpdate()
  {

    if (Physics.Raycast(transform.position, transform.forward, out rayHitInteractable, raycastDistance))
    {
      if (rayHitInteractable.transform.CompareTag("Interactable") && !isTargettingInteractable)
      {
        isTargettingInteractable = true;
        rayHitInteractable.transform.GetComponentInParent<Interactable>().Focused();
        Debug.Log("RAYCAST HIT");
        Debug.Log("isTargettingInteractable: " + isTargettingInteractable);
        Debug.Log("Player Mode: " + PLAYERSingleton.i.playerMode);
      }
    }
    else
    {
      isTargettingInteractable = false;
    }
  }

  void Update()
  {
    if (GMSingleton.i.inputManager.interaction.WasPressedThisFrame())
    {
      if (isTargettingInteractable)
      {
        if (canInteract(GMSingleton.i.currentInteraction.transform.position))
        {
          GMSingleton.Interact(GMSingleton.i.currentInteraction.type);
          
          Debug.Log("RAYCAST HIT");
          Debug.Log("isTargettingInteractable: " + isTargettingInteractable);
          Debug.Log("Player Mode: " + PLAYERSingleton.i.playerMode);
        }
      }
    }
  }

  public bool canInteract(Vector3 targetPosition)
  {
    if (Vector3.Distance(PLAYERSingleton.i.transform.position, targetPosition) < raycastDistance && PLAYERSingleton.i.playerMode != PlayerMode.Fight)
    {
      return true;
    }
    else return false;
  }

}
