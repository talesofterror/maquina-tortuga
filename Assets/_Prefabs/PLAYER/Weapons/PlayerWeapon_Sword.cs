using Unity.VisualScripting;
using UnityEngine;

// [System.Serializable]
public class PlayerWeapon_Sword : PlayerWeapon_BASE
{
  // * weird trick to access scriptable object data
  public ScriptableObject _data;
  public override ScriptableObject data => _data;

  public override void Draw()
  {
    Debug.Log("Sword has been drawn!");
    this.gameObject.SetActive(true);
  }
  public override void Withdraw()
  {
    Debug.Log("Sword has been drawn!");
    this.gameObject.SetActive(false);
  }

  public override void Attack()
  {
    Debug.Log("Sword is attacking!!");
  }

  public override void Animate()
  {

  }
}
