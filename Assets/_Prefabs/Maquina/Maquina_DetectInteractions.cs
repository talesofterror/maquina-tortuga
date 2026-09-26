using UnityEngine;

public class Maquina_DetectInteractions : MonoBehaviour
{

  Maquina_BaseController controller;
  Vector3 targetDirection;

  void Awake()
  {
    controller = GetComponentInParent<Maquina_BaseController>();
    if (controller == null)
    {
      Debug.LogWarning(transform.name + " did not find its controller!");
    }
  }


  void OnTriggerEnter(Collider other)
  {

    if (other.CompareTag("Player"))
    {
      Debug.Log(transform.name + " sees you.");
      controller.attentionTarget = other.gameObject;
    }

  }

  void OnTriggerExit(Collider other)
  {
    controller.attentionTarget = null;
  }


}
