using Unity.VisualScripting;
using UnityEngine;

// [System.Serializable]
public class PlayerWeapon_Sword : PlayerWeapon_BASE
{
  // * weird trick to access scriptable object data
  public ScriptableObject _data;
  public override ScriptableObject data => _data;

  public Transform rayStart;
  public Transform rayEnd;

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

  public override void StartAnimation()
  {
    PLAYERSingleton.i.animations.animator.SetBool(
    PLAYERSingleton.i.animations.FightStance,
    true);
  }
  public override void StopAnimation()
  {
    PLAYERSingleton.i.animations.animator.SetBool(
        PLAYERSingleton.i.animations.FightStance,
        false);
  }
}
