using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class Interactable : MonoBehaviour
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

  public void Focused()
  {
    UISingleton.i.debug.pushMessage("Interactable in sight: " + _name);
    
    GMSingleton.i.currentInteraction = new Interaction(this) ;
  }

}
