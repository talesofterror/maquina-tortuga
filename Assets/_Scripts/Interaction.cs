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
    this.name = _interactable._name;
    this.type = _interactable.type;
    this.transform = _interactable.transform;
    this.description = _interactable.description;
  }
  
}
