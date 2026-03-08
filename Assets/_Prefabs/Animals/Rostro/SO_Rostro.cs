using UnityEngine;

[CreateAssetMenu(fileName = "SO_Rostro", menuName = "Scriptable Objects/Animals/SO_Rostro")]
public class SO_Rostro : ScriptableObject
{
    [Header("Health Settings")]
    public int maxHP = 100;
    private bool invulnerable = false;

    [Header("Attack Settings")]
    public int maxAP = 10;
    public float laserScale = 0.1f;
    public float laserLength = 10f;
    public float laserExtendSpeed = 3f;
    public float laserRetractSpeed = 3f;
    
}
