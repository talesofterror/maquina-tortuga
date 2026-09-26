using System.Collections;
using UnityEngine;

public class Clone_State_Walk : FSM_Base
{

  Clone_StateController controller;
  public Vector3 targetPosition;
  Coroutine transitCoroutine;
  float speed;

  public Clone_State_Walk(Clone_StateController c) : base(c)
  {
    this.controller = c;
  }

  public override void Option(long option)
  {
    targetPosition = UTILITY.VectorPacker.UnpackVector3(option);
  }

  public override void Enter()
  {
    
  }
  public override void Loop()
  {

  }

  public override void Exit()
  {

  }

  IEnumerator IETransitCoroutine (Vector3 startingPos, Vector3 targetPos)
  {
    
    yield return null;
  }

}
