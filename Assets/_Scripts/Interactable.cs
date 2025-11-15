using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class Interactable : MonoBehaviour, I_Interactable
{
  public string _name;
  public InteractionType type;
  public string description;
  Interaction instance;

  // public GameObject trigger { get; set; }

  void Start()
  {
    // trigger = transform.Find("InteractableTrigger").gameObject;
  }

  public Interaction Focused()
  {
    instance = new Interaction(this);
    UISingleton.i.debug.pushMessage("Interactable in range: " + instance.name);
    UISingleton.i.debug.pushMessage(instance.description);
    GMSingleton.i.currentInteraction = new Interaction(this);
    return instance;
  }
}
