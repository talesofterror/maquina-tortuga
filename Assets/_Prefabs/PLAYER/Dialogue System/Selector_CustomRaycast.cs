using PixelCrushers.DialogueSystem;
using UnityEngine;

public class Selector_CustomRaycast : Selector
{
  public StandardUISelectorElements selectorElements;

  protected override void Run3DRaycast()
  {

    // Define custom ray here (e.g., a curved ray, or offset from eyes)
    // Ray ray = new Ray(transform.position, transform.forward);

    Ray ray = Camera.main.ScreenPointToRay(GetSelectionPoint());
    float raycastDistance = (distanceFrom == DistanceFrom.GameObject) ? Mathf.Infinity : maxSelectionDistance;
    lastRay = ray;

    RaycastHit hit;

    if (Physics.Raycast(ray, out hit, raycastDistance, layerMask))
    // if (Physics.Raycast(ray, out hit, PLAYERSingleton.i.interactionSightDistance, layerMask))
    // if (Physics.SphereCast(transform.position, 2, transform.forward, out hit, PLAYERSingleton.i.interactionSightDistance, layerMask))
    {
      if (!DialogueManager.isConversationActive)
      {
        if (hit.collider.CompareTag("Interactable"))
        {
          Usable hitUsable = hit.collider.GetComponent<Usable>();
          if (hitUsable != null && hitUsable.enabled)
          {
            // Set the distance so SelectorUseStandardUIElements can evaluate IsUsableInRange()
            distance = (distanceFrom == DistanceFrom.Camera) ? hit.distance
              : (distanceFrom == DistanceFrom.GameObject || actorTransform == null)
                  ? Vector3.Distance(gameObject.transform.position, hit.collider.transform.position)
                  : Vector3.Distance(actorTransform.position, hit.collider.transform.position);
            
            // Only fire the selection logic if the usable actually changed
            if (selection != hit.collider.gameObject)
            {
              SetCurrentUsable(hitUsable);
            }
            return;
          }
          else
          {
            // Debug.LogWarning("Selector CustomRaycast: Hit an 'Interactable' object, but it either lacks a Usable component or it's disabled on: " + hit.collider.gameObject.name);
          }
        }
      }

      DeselectTarget();
    }
    else
    {
      DeselectTarget();
    }
  }

}