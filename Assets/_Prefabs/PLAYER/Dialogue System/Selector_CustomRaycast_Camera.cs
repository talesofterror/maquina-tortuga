using UnityEngine;
using PixelCrushers.DialogueSystem;
using PixelCrushers;

public class Selector_CustomRaycast_Camera : Selector
{
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
        return new Vector3(Screen.width / 2, (Screen.height / 2) + 50);
    }
  }

  protected override void Run3DRaycast()
  {
    Ray ray = UnityEngine.Camera.main.ScreenPointToRay(GetSelectionPoint());
    lastRay = ray;

    // New Variable rayCastDistance is used below for the raycasts instead of maxSelectionDistance to be able to set it to infinity (if using DistanceFrom.GameObject) instead of maxSelectionDistance:
    // Credit: Daniel D. (Thank you!)
    float raycastDistance = (distanceFrom == DistanceFrom.GameObject) ? Mathf.Infinity : maxSelectionDistance;

    if (raycastAll)
    {

      // Run RaycastAll:
      if (lastHits == null) lastHits = new RaycastHit[MaxHits];
      numLastHits = Physics.RaycastNonAlloc(ray, lastHits, raycastDistance, layerMask);
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

      // Cast a ray and see what we hit:
      RaycastHit hit;
      // if (Physics.Raycast(ray, out hit, maxSelectionDistance, layerMask))
      if (Physics.SphereCast(ray, 5f, out hit, maxSelectionDistance, layerMask))
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

  public override void SetCurrentUsable(Usable usable)
  {
    if (usable == this.usable) return;
    if (usable == null)
    {
      DeselectTarget();
    }
    else
    {
      if (this.usable != null && this.usable != usable) DeselectTarget();
      this.usable = usable;
      usable.disabled -= OnUsableDisabled;
      usable.disabled += OnUsableDisabled;
      selection = usable.gameObject;
      heading = string.Empty;
      useMessage = string.Empty;
      OnSelectedUsableObject(usable);
    }
  }
}
