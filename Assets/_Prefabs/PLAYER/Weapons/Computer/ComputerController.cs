using System.Collections;
using UnityEngine;

public class ComputerController : MonoBehaviour
{
  [HideInInspector] public Animator animator;
  private Coroutine _activeRoutine;

  void Awake()
  {
    animator = GetComponent<Animator>();
  }

  public void Opening()
  {
    PLAYERSingleton.i.computer.SetActive(true);
    StopCurrentRoutine();
    _activeRoutine = StartCoroutine(OpeningCoroutine());
  }

  IEnumerator OpeningCoroutine()
  {
    animator.ResetTrigger("Close");
    animator.SetTrigger("Open");
    yield return null;

    var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
    var openAnimationLength = stateInfo.length;

    if (openAnimationLength > 0f)
    {
      yield return new WaitForSeconds(openAnimationLength);
    }

    _activeRoutine = null;
    PLAYERSingleton.i.animations.animator.SetBool("CastStance", true);
  }

  public void Closing()
  {
    StopCurrentRoutine();
    _activeRoutine = StartCoroutine(ClosingCoroutine());
  }

  IEnumerator ClosingCoroutine()
  {
    animator.SetBool("Idle", false);
    animator.ResetTrigger("Open");
    animator.SetTrigger("Close");
    yield return null;

    var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
    var closeAnimationLength = stateInfo.length;

    if (closeAnimationLength > 0f)
    {
      yield return new WaitForSeconds(closeAnimationLength);
    }

    _activeRoutine = null;
    PLAYERSingleton.i.computer.SetActive(false);
  }

  private void StopCurrentRoutine()
  {
    if (_activeRoutine != null)
    {
      StopCoroutine(_activeRoutine);
      _activeRoutine = null;
    }
  }
}
