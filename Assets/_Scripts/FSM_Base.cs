using UnityEngine;

public abstract class FSM_Base 
{
    protected FSM_BaseController controller;

    public FSM_Base (FSM_BaseController controller, Coroutine laserCoroutine = null)
    {
        this.controller = controller;
    }

    public abstract void Enter();   // Initialization logic
    public abstract void Update();  // Frame-by-frame logic
    public abstract void Exit();    // Cleanup logic
}
