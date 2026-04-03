using UnityEngine;

[CreateAssetMenu(fileName = "SO_PlayerWeapon_Sword", menuName = "Scriptable Objects/PlayerWeapon_Sword")]
public class SO_PlayerWeapon_Sword : ScriptableObject
{
  public string _name = "Sword";

  public float damage;
  public float speed;
    
}
