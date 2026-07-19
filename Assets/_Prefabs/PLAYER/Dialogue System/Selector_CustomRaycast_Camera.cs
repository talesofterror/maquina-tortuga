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
        return new Vector3(Screen.width / 2, (Screen.height / 2) + UISingleton.i.interactableSelectionHeight);
    }
  }

  [Header("Selector UI")]
  public Canvas uiCanvas;
  public RectTransform reticleRect;
  public Vector2 reticleScreenOffset;

  [Header("Raycast Settings")]
  public Vector3 rayDirectionEulerOffset = Vector3.zero;

  public float stickySelectionDeadzone = 2.0f;
  public float closerTargetThreshold = 0.5f;

  protected override void Run3DRaycast()
  {
    Ray ray = UnityEngine.Camera.main.ScreenPointToRay(GetSelectionPoint());
    lastRay = ray;
// Apply direction offset to both raycasts only when zooming
    Vector3 rayDirection = ray.direction;
    if (CAMERASingleton.i.zooming && rayDirectionEulerOffset != Vector3.zero)
    {
      Quaternion offsetRotation = Quaternion.Euler(rayDirectionEulerOffset);
      rayDirection = offsetRotation * rayDirection;
    }
    Ray offsetRay = new Ray(ray.origin, rayDirection);

    // New Variable rayCastDistance is used below for the raycasts instead of maxSelectionDistance to be able to set it to infinity (if using DistanceFrom.GameObject) instead of maxSelectionDistance:
    // Credit: Daniel D. (Thank you!)
    float raycastDistance = (distanceFrom == DistanceFrom.GameObject) ? Mathf.Infinity : maxSelectionDistance;

    if (raycastAll)
    {

      // Run RaycastAll:
      if (lastHits == null) lastHits = new RaycastHit[MaxHits];
      numLastHits = Physics.RaycastNonAlloc(offsetRay, lastHits, raycastDistance, layerMask);
      bool foundUsable = false;
      Usable closestNewUsable = null;
      float closestNewDistance = Mathf.Infinity;
      float currentSelectionDistance = Mathf.Infinity;
      bool currentSelectionStillVisible = false;

      for (int i = 0; i < numLastHits; i++)
      {
        var hit = lastHits[i];
        float hitDistance = (distanceFrom == DistanceFrom.Camera) ? hit.distance
            : (distanceFrom == DistanceFrom.GameObject || actorTransform == null)
                ? Vector3.Distance(gameObject.transform.position, hit.collider.transform.position)
                : Vector3.Distance(actorTransform.position, hit.collider.transform.position);
        if (selection == hit.collider.gameObject)
        {
          currentSelectionStillVisible = true;
          currentSelectionDistance = hitDistance;
          // Keep current selection if within deadzone
          if (hitDistance <= stickySelectionDeadzone)
          {
            foundUsable = true;
            distance = hitDistance;
            lastHit = hit;
            break;
          }
        }
        else
        {
          Usable hitUsable = hit.collider.GetComponent<Usable>();
          if (hitUsable != null && hitUsable.enabled && hitDistance <= maxSelectionDistance)
          {
            // Track the closest new usable for comparison
            if (hitDistance < closestNewDistance)
            {
              closestNewUsable = hitUsable;
              closestNewDistance = hitDistance;
            }
          }
        }
      }

      // If current selection wasn't found in deadzone, check if we should switch to a closer target
      if (!foundUsable && closestNewUsable != null)
      {
        // Switch to new target if current selection is not visible or new target is significantly closer
        if (!currentSelectionStillVisible || (closestNewDistance + closerTargetThreshold < currentSelectionDistance))
        {
          foundUsable = true;
          distance = closestNewDistance;
          SetCurrentUsable(closestNewUsable);
          lastHit = lastHits[0]; // Update lastHit to the new selection (you may want to find the exact hit)
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
      if (Physics.SphereCast(offsetRay, 0.5f, out hit, maxSelectionDistance, layerMask))
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
            // Sticky selection: only switch if new target is significantly closer
            if (usable != null)
            {
              float currentDistance = (distanceFrom == DistanceFrom.Camera) ? Vector3.Distance(Camera.main.transform.position, usable.transform.position)
                  : (distanceFrom == DistanceFrom.GameObject || actorTransform == null)
                      ? Vector3.Distance(gameObject.transform.position, usable.transform.position)
                      : Vector3.Distance(actorTransform.position, usable.transform.position);
              // Only switch if new target is closer by threshold amount
              if (distance + closerTargetThreshold >= currentDistance)
              {
                SetSelectorElementsActive(true);
                lastHit = hit;
                return; // Keep current selection
              }
            }
            // Debug.Log("Usable hit: " + hitUsable.GetName());
            SetCurrentUsable(hitUsable);
            SetSelectorElementsActive(true);
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

    // RETICLE CODE
    // if (reticleRect != null)
    // {
    //   if (CAMERASingleton.i.zooming && lastHit.collider != null)
    //   {
    //     Vector3 screenPos = Camera.main.WorldToScreenPoint(lastHit.point);
    //     if (screenPos.z > 0f)
    //     {
    //       reticleRect.gameObject.SetActive(true);
    //       reticleRect.position = screenPos + (Vector3)reticleScreenOffset;
    //     }
    //     else
    //     {
    //       reticleRect.gameObject.SetActive(false);
    //     }
    //   }
    //   else
    //   {
    //     reticleRect.gameObject.SetActive(false);  // Hide reticle when zooming or no hit
    //   }
    // }

    // Refresh selector visuals each frame while a usable is selected so colors update in real time
    if (this.usable != null)
    {
      SetSelectorElementsActive(true);
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

  void SetSelectorElementsActive(bool state)
  {
    // If a dialogue/conversation is active, always hide selector elements
    if (DialogueManager.isConversationActive)
    {
      UISingleton.i.selectorElements.gameObject.SetActive(false);
      UISingleton.i.selectorName.text = "";
      UISingleton.i.selectorUseMessage.text = defaultUseMessage;
      return;
    }

    if (CurrentUsable == null)
    {
      UISingleton.i.selectorElements.gameObject.SetActive(false);
      return;
    }
    bool inUseRange = (distance <= CurrentUsable.maxUseDistance);
    if (state == true)
    {
      UISingleton.i.selectorElements.gameObject.SetActive(true);
      // Set raw text and use TMP color properties for real-time updates
      UISingleton.i.selectorName.text = usable.GetName();
      Color nameColor;
      if (!UnityEngine.ColorUtility.TryParseHtmlString(inUseRange ? "#FF0072" : "#777777", out nameColor)) nameColor = Color.white;
      UISingleton.i.selectorName.color = nameColor;

      string useMsg;
      if (inUseRange)
      {
        useMsg = string.IsNullOrEmpty(usable.overrideUseMessage) ? defaultUseMessage : usable.overrideUseMessage;
      }
      else
      {
        useMsg = "Get closer to interact";
      }
      UISingleton.i.selectorUseMessage.text = DialogueManager.GetLocalizedText(useMsg);
      Color msgColor;
      if (!UnityEngine.ColorUtility.TryParseHtmlString(inUseRange ? "#10ccff" : "#777777", out msgColor)) msgColor = Color.white;
      UISingleton.i.selectorUseMessage.color = msgColor;
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
      // bool inUseRange = (distance <= usable.maxUseDistance);
      bool inUseRange = (distance <= PLAYERSingleton.i.interactionReachDistance); // makes usable.maxUseDistance useless?
      guiStyle.normal.textColor = inUseRange ? inRangeColor : outOfRangeColor;
      if (string.IsNullOrEmpty(heading))
      {
        heading = usable.GetName();
        useMessage = DialogueManager.GetLocalizedText(string.IsNullOrEmpty(usable.overrideUseMessage) ? defaultUseMessage : usable.overrideUseMessage);
      }

      // original reticule code for onGUI
      // Texture2D reticleTexture = inUseRange ? reticle.inRange : reticle.outOfRange;
      // if (reticleTexture != null) GUI.Label(new Rect(0.5f * (Screen.width - reticle.width), 0.5f * (Screen.height - reticle.height), reticle.width, reticle.height), reticleTexture);

      // New reticule code -- reticule lands on the usable and adapts to camera position
      // But maybe I just won't use the reticule
      // Vector3 screenPos = Camera.main.WorldToScreenPoint(usable.transform.position);

      // Ray rayToUsable = Camera.main.ScreenPointToRay(screenPos);
      // Debug.DrawLine(rayToUsable.origin, rayToUsable.origin + rayToUsable.direction * 100f, Color.yellow);

      // if (reticleTexture != null && screenPos.z > 0)  // z > 0 means it's in front of camera
      // {
      //   GUI.Label(new Rect(screenPos.x - reticle.width * 0.5f, screenPos.y - reticle.height * 0.5f, reticle.width, reticle.height), reticleTexture);
      // }

    }
  }

}
