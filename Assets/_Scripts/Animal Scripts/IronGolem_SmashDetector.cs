using UnityEngine;

public class IronGolem_SmashDetector : MonoBehaviour
{
    [HideInInspector]
    public Animal_IronGolem golem;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !PLAYERSingleton.i.isTakingDamage)
        {
            Debug.Log("Player hit by golem smash");
            PLAYERSingleton.i.playerHealth.TakeDamage(10);
            PLAYERSingleton.i.playerHealth.DamageKnockback(
                golem.rB.position,
                golem.smashKnockbackForce
            );
        }
    }
}
