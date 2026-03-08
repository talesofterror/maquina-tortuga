using UnityEngine;

public class FSM_RostroController : MonoBehaviour
{
  private FSM_Base _currentState;

  // References to concrete states
  public FSM_RostroIdle Idle;
  public FSM_RostroAttack Attack;
  public FSM_RostroDeactivated Deactivated;

  public Animal_Rostro animalRostro;
  public LaserGenerator laserGenerator;

  // Cached Components (States will use these via the manager)
  [HideInInspector] public Animation anim;

  void Start()
  {
    // Initialize the states
    Idle = new FSM_RostroIdle(this);
    Attack = new FSM_RostroAttack(this);
    Deactivated = new FSM_RostroDeactivated(this);

    anim = GetComponent<Animation>();
    laserGenerator = GetComponent<LaserGenerator>();

    // Set the starting state
    SwitchState(Idle);
  }

  void Update()
  {
    // Execute the current state's update logic every frame
    _currentState?.Update();
  }

  public void SwitchState(FSM_Base newState)
  {
    _currentState?.Exit();  // Clean up old state
    _currentState = newState;
    _currentState.Enter(); // Start new state
  }
}
