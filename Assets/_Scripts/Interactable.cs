using Unity.VisualScripting;
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

  void Update()
  {

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
      Debug.Log(this.name + " has been spotted");
    }
  }
  public void SetReachState(ReachState newState)
  {
    if (reachState == newState) return;

    reachState = newState;

    if (reachState == ReachState.Reachable)
    {
      // SetAsCurrentReachableInteraction();
      Debug.Log(this.name + " is now reachable");
    }
    if (reachState == ReachState.Unreachable)
    {
      // GMSingleton.i.currentInteraction = null;
    }
  }

  public bool CanReach()
  {
    // if (GMSingleton.i.currentInteraction != null || GMSingleton.i.currentInteraction.i != null)
    // {
      if (Vector3.Distance(
          PLAYERSingleton.i.transform.position, transform.position) < PLAYERSingleton.i.interactionReachDistance
          && PLAYERSingleton.i.stateController.currentState != PLAYERSingleton.i.stateController.state_Fight)
      {
        return true;
      }
      else return false;
    // }
    // else
    //   return false;
  }

  public void SetAsCurrentReachableInteraction()
  {
    UISingleton.i.debug.pushMessage("Setting GM.currentInteraction to: " + _name);

    GMSingleton.i.currentInteraction = new Interaction(this);
  }

}
