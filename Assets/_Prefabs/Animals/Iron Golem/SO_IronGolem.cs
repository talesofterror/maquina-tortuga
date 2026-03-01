using UnityEngine;

[CreateAssetMenu(fileName = "SO_IronGolem", menuName = "Scriptable Objects/Animals/SO_IronGolem")]
public class SO_IronGolem : ScriptableObject
{
    public int maxHP = 100;
    public int maxAP = 100;

    [Header("Movement Settings")]
    public float speed = 5f;

    [Header("Detection Settings")]
    public float scanSpeed = 1f;
    public float scanAngle = 90f;
    public float sightDistance = 10f;
    public float forgetDistance = 20f;

    [Header("Attack Settings")]
    public float attackCooldown = 2f;
    public float smashRadius = 1f;
    public float smashDamageDelay = 1f;
    public float smashDamageDuration = 1f;
    public float smashThrustForce = 10f;
    public float alertDuration = 2f;
    public float pathUpdateFrequency = 1f;
    public float sightHeight = 1f;
}
