using UnityEngine;

public abstract class FSM_BaseController : MonoBehaviour
{
  public FSM_Base _currentState;

  public FSM_BaseController ()
  {
    
  }

  public abstract void SwitchState(FSM_Base newState, int option = 0);
}
