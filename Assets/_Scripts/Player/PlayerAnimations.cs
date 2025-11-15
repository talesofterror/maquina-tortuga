using System;
using System.Collections;
using UnityEngine;

public class PlayerAnimations : MonoBehaviour
{

  [HideInInspector] public Animator animator;
  public AnimatorStateInfo stateInfo;
  public int FightStance;
  public int FightSlash1;

  void Awake()
  {
    animator = GetComponentInParent<Animator>();
    stateInfo = animator.GetCurrentAnimatorStateInfo(0);
    FightStance = Animator.StringToHash("FightStance");
    FightSlash1 = Animator.StringToHash("slash1");
  }

  public float currentAnimationLength()
  {
    return stateInfo.length;
  }

  public IEnumerator waitForLength(float time, string animation)
  {
    Debug.Log("waitForLength coroutine started");
    // PLAYERSingleton.i.movementEnabled = false;
    // PLAYERSingleton.i.vController.setStopMove(true);
    // Debug.Log("movement enabled?: " + PLAYERSingleton.i.movementEnabled);
    Debug.Log(stateInfo.length);
    PLAYERSingleton.i.animations.animator.SetTrigger(animation);
    PLAYERSingleton.i.freezeMovement = true;
    yield return new WaitForSeconds(time);
    // PLAYERSingleton.i.animations.animator.ResetTrigger(animation);
    PLAYERSingleton.i.freezeMovement = false;
    // PLAYERSingleton.i.movementEnabled = true;
    // PLAYERSingleton.i.vController.setStopMove(false);
    // Debug.Log("movement enabled?: " + PLAYERSingleton.i.movementEnabled);
  }

}
