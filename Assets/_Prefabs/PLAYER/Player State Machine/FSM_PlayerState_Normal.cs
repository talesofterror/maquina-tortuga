using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;

public class FSM_PlayerState_Normal : FSM_PlayerStateBase
{

  public FSM_PlayerState_Normal(FSM_PlayerStateController c) : base(c) { }


  FSM_PlayerStateBase cachedSubstate;

  public override void Enter()
  {
    // this.SetSubState(new FSM_PlayerState_Looking(controller));
    // PLAYERSingleton.i.movementDisabled = false;
    Debug.Log("Entering Normal State");
    Debug.Log("Substate: " + this._currentSubState);
    SetSubState(controller.state_Looking);
  }

  public override void Exit()
  {
    // currentInteractable = null;
  }

  public override void Loop()
  {
    // release code in casting state
    if (GMSingleton.i.inputManager.cast.WasPressedThisFrame())
    {
      SetSubState(controller.state_Casting);
    }
  }

}
