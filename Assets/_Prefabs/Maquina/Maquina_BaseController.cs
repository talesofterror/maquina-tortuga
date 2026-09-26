using UnityEngine;

public class Maquina_BaseController : FSM_BaseController
{
  public Maquina_DetectInteractions interactionDetector;

  [HideInInspector] public Vector3 direction;
  [HideInInspector] public Vector3 cachedDirection;
  [HideInInspector] public FSM_Base cacheState;

  [HideInInspector] public GameObject attentionTarget;

  public void SetFacingDirection(Vector3 targetPosition)
  {
    Vector3 lookDirection = targetPosition - transform.position;
    if (lookDirection.sqrMagnitude <= 0.01f)
    {
      return;
    }

    direction = lookDirection.normalized;
    transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
  }

  public override void SwitchState(FSM_Base newState, long option = 0)
  {
    _currentState?.Exit();
    _currentState = newState;
    if (option != 0) _currentState.Option(option);
    _currentState?.Enter();
  }


}
