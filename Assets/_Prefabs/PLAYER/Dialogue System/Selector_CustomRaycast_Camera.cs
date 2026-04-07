using UnityEngine;
using PixelCrushers.DialogueSystem;
using PixelCrushers;

public class Selector_CustomRaycast_Camera : Selector
{
  [Tooltip("Radius of the sphere used for selection casting.")]
  public float sphereCastRadius = 2f;

  protected override Vector3 GetSelectionPoint()
  {
    switch (selectAt)
    {
      case SelectAt.MousePosition:
        return InputDeviceManager.GetMousePosition();
      case SelectAt.CustomPosition:
        return CustomPosition;
      default:
      case SelectAt.CenterOfScreen:
        return new Vector3(Screen.width / 2, Screen.height / 2 + 10);
    }
  }

  protected override void Run3DRaycast()
  {
    Ray ray = UnityEngine.Camera.main.ScreenPointToRay(GetSelectionPoint());
    lastRay = ray;

    float raycastDistance = (distanceFrom == DistanceFrom.GameObject) ? Mathf.Infinity : maxSelectionDistance;

    if (raycastAll)
    {
      // Run SphereCastAll (using SphereCastNonAlloc for performance):
      if (lastHits == null) lastHits = new RaycastHit[MaxHits];
      numLastHits = Physics.SphereCastNonAlloc(ray, sphereCastRadius, lastHits, raycastDistance, layerMask);
      bool foundUsable = false;
      for (int i = 0; i < numLastHits; i++)
      {
        var hit = lastHits[i];
        float hitDistance = (distanceFrom == DistanceFrom.Camera) ? hit.distance
            : (distanceFrom == DistanceFrom.GameObject || actorTransform == null)
                ? Vector3.Distance(gameObject.transform.position, hit.collider.transform.position)
                : Vector3.Distance(actorTransform.position, hit.collider.transform.position);
        if (selection == hit.collider.gameObject)
        {
          foundUsable = true;
          distance = hitDistance;
          break;
        }
        else
        {
          Usable hitUsable = hit.collider.GetComponent<Usable>();
          if (hitUsable != null && hitUsable.enabled && hitDistance <= maxSelectionDistance)
          {
            foundUsable = true;
            distance = hitDistance;
            SetCurrentUsable(hitUsable);
            break;
          }
        }
      }
      if (!foundUsable)
      {
        DeselectTarget();
      }
    }
    else
    {
      // Cast a sphere and see what we hit:
      RaycastHit hit;
      if (Physics.SphereCast(ray, sphereCastRadius, out hit, maxSelectionDistance, layerMask))
      {
        distance = (distanceFrom == DistanceFrom.Camera) ? hit.distance
            : (distanceFrom == DistanceFrom.GameObject || actorTransform == null)
                ? Vector3.Distance(gameObject.transform.position, hit.collider.transform.position)
                : Vector3.Distance(actorTransform.position, hit.collider.transform.position);
        Usable hitUsable = hit.collider.GetComponent<Usable>();
        if (hitUsable != null && hitUsable.enabled)
        {
          if (selection != hit.collider.gameObject)
          {
            SetCurrentUsable(hitUsable);
          }
        }
        else
        {
          DeselectTarget();
        }
      }
      else
      {
        DeselectTarget();
      }
      lastHit = hit;
    }
  }
}
