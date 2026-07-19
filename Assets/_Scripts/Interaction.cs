using UnityEngine;

public class Interaction
{
  public Interactable i;
  public InteractionType type;
  public string name;
  public string description;
  public Transform transform;

  public Interaction(Interactable _interactable)
  {
    i = _interactable;
  }
  
}
