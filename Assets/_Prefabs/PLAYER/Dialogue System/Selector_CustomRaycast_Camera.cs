using UnityEngine;
using PixelCrushers.DialogueSystem;
using PixelCrushers;
using Unity.VisualScripting;

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
        return new Vector3(Screen.width / 2, (Screen.height / 2) + UISingleton.i.interactableSelectionHeight);
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
      if (Physics.SphereCast(ray, 0.5f, out hit, maxSelectionDistance, layerMask))
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
            // Debug.Log("Usable hit: " + hitUsable.GetName());


            SetCurrentUsable(hitUsable);
          }
        }
        else
        {
          UISingleton.i.selectorElements.gameObject.SetActive(false);
          DeselectTarget();
        }
      }
      else
      {
        UISingleton.i.selectorElements.gameObject.SetActive(false);
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

      SetSelectorElementsActive(true);
      OnSelectedUsableObject(usable);
    }
  }

  void SetSelectorElementsActive(bool state)
  {
    if (state == true)
    {
      UISingleton.i.selectorElements.gameObject.SetActive(true);
      UISingleton.i.selectorName.text = usable.GetName();
      UISingleton.i.selectorUseMessage.text = DialogueManager.GetLocalizedText(string.IsNullOrEmpty(usable.overrideUseMessage) ? defaultUseMessage : usable.overrideUseMessage);
    }
    else
    {
      UISingleton.i.selectorElements.gameObject.SetActive(false);
      UISingleton.i.selectorName.text = "";
      UISingleton.i.selectorUseMessage.text = defaultUseMessage;
    }
  }

  public void DeselectTarget_External()
  {
    DeselectTarget();
  }

  public override void OnGUI()
  {
    if (!enabled) return;
    if (!useDefaultGUI) return;
    if (guiStyle == null && (Event.current.type == EventType.Repaint || usable != null))
    {
      SetGuiStyle();
    }
    if (usable != null)
    {
      bool inUseRange = (distance <= usable.maxUseDistance);
      guiStyle.normal.textColor = inUseRange ? inRangeColor : outOfRangeColor;
      if (string.IsNullOrEmpty(heading))
      {
        heading = usable.GetName();
        useMessage = DialogueManager.GetLocalizedText(string.IsNullOrEmpty(usable.overrideUseMessage) ? defaultUseMessage : usable.overrideUseMessage);
      }
      // PixelCrushers.DialogueSystem.UnityGUI.UnityGUITools.DrawText(new Rect(0, 0, Screen.width, Screen.height), heading, guiStyle, textStyle, textStyleColor);
      // PixelCrushers.DialogueSystem.UnityGUI.UnityGUITools.DrawText(new Rect(0, guiStyleLineHeight, Screen.width, Screen.height), useMessage, guiStyle, textStyle, textStyleColor);
      Texture2D reticleTexture = inUseRange ? reticle.inRange : reticle.outOfRange;
      if (reticleTexture != null) GUI.Label(new Rect(0.5f * (Screen.width - reticle.width), 0.5f * (Screen.height - reticle.height), reticle.width, reticle.height), reticleTexture);


    }
  }

}
