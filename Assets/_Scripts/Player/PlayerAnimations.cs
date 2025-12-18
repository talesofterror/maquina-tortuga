using System;
using System.Collections;
using UnityEngine;

public class PlayerAnimations : MonoBehaviour
{
    [HideInInspector]
    public Animator animator;
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

    public float CurrentAnimationLength()
    {
        return stateInfo.length;
    }

    public IEnumerator WaitAndFreeze(float time, string animation)
    {
        PLAYERSingleton.i.animations.animator.SetTrigger(animation);
        PLAYERSingleton.i.movementDisabled = true;
        yield return new WaitForSeconds(time);

        PLAYERSingleton.i.movementDisabled = false;
    }
}
