using System;
using PixelCrushers.DialogueSystem;
using Unity.VisualScripting;
using UnityEditor.Animations;
using UnityEngine;

[System.Serializable]
public class Interactable : MonoBehaviour
{
  public string _name;
  public InteractionType type;
  public string description;
  Interaction instance;

  // * STATES
  public enum SightState { Unsighted, Sighted }
  public enum ReachState { Reachable, Unreachable }

  public SightState sightState = SightState.Unsighted;
  public ReachState reachState = ReachState.Unreachable;

  DialogueSystemTrigger dialogueSystemTrigger;


  // * Outline Layers
  protected int defaultLayer;
  protected int outlineLayer;

  void Start()
  {
    defaultLayer = gameObject.layer;
    outlineLayer = LayerMask.NameToLayer("Outlined");
    if (TryGetComponent(out DialogueSystemTrigger dialogueSystemTrigger))
    {
      dialogueSystemTrigger.maxConversationDistance = GetComponent<Usable>().maxUseDistance;
    }
  }


  public static void Interact(InteractionType type)
  {
    if (type == InteractionType.Warp)
    {
      // GMSingleton.i.currentInteraction.i.gameObject.GetComponent<SceneLoader>().loadLevel();
      PLAYERSingleton.i.stateController.SwitchState(PLAYERSingleton.i.stateController.state_Interact);
      PLAYERSingleton.i.dialogueSelector.UseCurrentSelection();
      // PLAYERSingleton.i.stateController.SwitchState(PLAYERSingleton.i.stateController.state_Normal);
    }
    if (type == InteractionType.Friend)
    {
      PLAYERSingleton.i.stateController.SwitchState(PLAYERSingleton.i.stateController.state_Interact);
      PLAYERSingleton.i.dialogueSelector.UseCurrentSelection();
      // DialogueManager.StartConversation("Introductory exchange");
      // PLAYERSingleton.i.stateController.SwitchState(PLAYERSingleton.i.stateController.state_Normal);
    }
  }

  public void SetSightState(SightState newState)
  {
    if (sightState == newState) return;

    sightState = newState;

    if (sightState == SightState.Unsighted)
    {
      SetReachState(ReachState.Unreachable);
    }
    if (sightState == SightState.Sighted)
    {
      // Debug.Log(this.name + " has been spotted");
    }
  }
  public void SetReachState(ReachState newState)
  {
    if (reachState == newState) return;

    reachState = newState;

    if (reachState == ReachState.Reachable)
    {
      Debug.Log(this.name + " is now reachable");
    }
    if (reachState == ReachState.Unreachable)
    {
      // GMSingleton.i.currentInteraction = null;
    }
  }

  public bool CanReach()
  {
    if (Vector3.Distance(
        PLAYERSingleton.i.transform.position, transform.position) < PLAYERSingleton.i.interactionReachDistance
        && PLAYERSingleton.i.stateController.currentState != PLAYERSingleton.i.stateController.state_Fight)
    {
      return true;
    }
    else return false;

  }

  public void SetAsCurrentReachableInteraction()
  {
    UISingleton.i.debug.pushMessage("Setting GM.currentInteraction to: " + _name);

    GMSingleton.i.currentInteraction = new Interaction(this);
  }


  // * Outline settings
  public Color customOutlineColor = Color.blue;
  public float customOutlineThickness = 2f;
  private MaterialPropertyBlock _propBlock;
  public void SetOutline(bool show)
  {
    Renderer[] renderers = GetComponentsInChildren<Renderer>();
    _propBlock = new MaterialPropertyBlock();

    for (int i = 0; i < renderers.Length; i++)
    {
      renderers[i].gameObject.layer = show ? outlineLayer : defaultLayer;
      renderers[i].GetPropertyBlock(_propBlock);
      _propBlock.SetColor("_OutlineColor", customOutlineColor);
      _propBlock.SetFloat("_OutlineWidth", customOutlineThickness);
      renderers[i].SetPropertyBlock(_propBlock);
    }
  }

}
