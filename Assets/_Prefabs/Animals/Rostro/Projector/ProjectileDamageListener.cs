using UnityEngine;

public class ProjectileDamageListener : MonoBehaviour
{
  public int damage = 10;
  public float force = 25;

  void OnTriggerEnter(Collider other)
  {
    if (other.CompareTag("Player"))
    {
      PLAYERSingleton.i.playerHealth.TakeDamage(damage);
      PLAYERSingleton.i.playerHealth.DamageKnockback(transform.position, force);
    }
  }
}
