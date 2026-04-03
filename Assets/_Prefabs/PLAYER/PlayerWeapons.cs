using UnityEngine;

public class PlayerWeapons : MonoBehaviour
{
  public GameObject weapon;

  public PlayerWeapon_BASE currentWeapon;

  public PlayerWeapon_Sword sword;

  void Start()
  {
    currentWeapon = sword;
    currentWeapon.gameObject.SetActive(false);
  }

  void Update() { }
}
