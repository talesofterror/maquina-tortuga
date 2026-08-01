using UnityEngine;

public class FSM_RostroController : FSM_BaseController
{
  // private FSM_Base _currentState;

  // References to concrete states
  public FSM_RostroIdle state_Idle;
  public FSM_RostroAttack state_Attack;
  public FSM_RostroDeactivated state_Deactivated;

  public Animal_Rostro animalRostro;
  public LaserGenerator laserGenerator;

  // Cached Components (States will use these via the manager)
  [HideInInspector] public Animation anim;

  void Start()
  {
    // Initialize the states
    state_Idle = new FSM_RostroIdle(this);
    state_Attack = new FSM_RostroAttack(this);
    state_Deactivated = new FSM_RostroDeactivated(this);

    anim = GetComponent<Animation>();
    laserGenerator = GetComponent<LaserGenerator>();

    // Set the starting state
    _currentState = state_Idle;
  }

  bool attacking = false;

  void OnTriggerEnter(Collider other)
  {
    if (other.CompareTag("Player"))
    {
      if (!attacking)
      {
        SwitchState(state_Attack);
        attacking = true;
      }
    }
  }
  void OnTriggerExit(Collider other)
  {
    if (other.CompareTag("Player"))
    {
      if (attacking)
      {
        SwitchState(state_Idle);
        attacking = false;
      }
    }
  }

  void Update()
  {
    // Execute the current state's update logic every frame
    _currentState?.Update();

  }
  public override void SwitchState(FSM_Base newState, int option = 0)
  {
    _currentState?.Exit();  // Clean up old state
    _currentState = newState;
    _currentState.Enter(); // Start new state
  }
}
