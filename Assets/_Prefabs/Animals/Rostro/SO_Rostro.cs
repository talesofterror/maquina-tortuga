using UnityEngine;

[CreateAssetMenu(fileName = "SO_Rostro", menuName = "Scriptable Objects/Animals/SO_Rostro")]
public class SO_Rostro : ScriptableObject
{
    [Header("Health Settings")]
    public int maxHP = 100;

    public float rotationSpeed = 0.4f;
    private bool invulnerable = false;

    [Header("Laser Settings")]
    public int maxAP = 10;
    public float laserScale = 0.1f;
    public float laserLength = 10f;
    public float laserExtendSpeed = 3f;
    public float laserRetractSpeed = 3f;

    [Header("Projectile Settings")]
    public float projectileInterval = 1.5f;
    public int projectilePoolSize = 10;
    public float projectileSpeed = 10;
    public float projectileDistance = 15;
    public int projectileDamage = 10;
    public float projectileForce = 25f;

    [Header("Player Detection")]
    public float detectionRadius = 10f;
    
}
