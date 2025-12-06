using UnityEngine;
using System.Collections;
using System;

[SelectionBase]
public class Animal_IronGolem : MonoBehaviour, I_Animal
{

  public int _hp;
  public int hp
  {
    get { return _hp; }
    set { _hp = value; }
  }

  public int _ap;
  public int ap
  {
    get { return _ap; }
    set { _ap = value; }
  }

  public int _mp;
  public int mp
  {
    get { return _mp; }
    set { _mp = value; }
  }

  public float speed = 1f;
  public bool inTransit;
  public EnemyMode mode;
  
  public bool running;
  public Vector3 direction;

  WaypointSystem waypointSystem;

  Coroutine movementMotorCoroutine;

  Animator animator;
  Rigidbody rB;

  void Awake()
  {
    waypointSystem = GetComponentInChildren<WaypointSystem>();
    // mode = new EnemyMode();
    mode = EnemyMode.Patrol;
    rB = GetComponentInChildren<Rigidbody>();
    animator = GetComponent<Animator>();
    playerLayerMask = LayerMask.GetMask("Player");
  }
  
  public void Patrol()
  {

    if (!inTransit)
    {
      inTransit = true;
      waypointSystem.speed = speed;
      movementMotorCoroutine = StartCoroutine(MovementMotor());
    }

    if (direction.sqrMagnitude > 0)
    transform.rotation = Quaternion.LookRotation(-direction);

    if (PlayerDetected()) {
      Debug.Log(transform.name + " detected the player!");
      StopCoroutine(movementMotorCoroutine);
      StartCoroutine(InitPlayerDetected());
    };

  }

  IEnumerator InitPlayerDetected () {
    transform.LookAt(PLAYERSingleton.i.transform.position);
    mode = EnemyMode.Alert;
    yield return null;
  }

  public EnemyMode _prevMode;

  public void Alert () {
    if (_prevMode == EnemyMode.Alert) {

    }
    transform.LookAt(PLAYERSingleton.i.transform.position);

    if (Vector3.Distance(transform.position, PLAYERSingleton.i.transform.position) > forgetDistance) {
      mode = EnemyMode.Patrol;
    }
  }

  public void Die()
  {
    Destroy(gameObject);
    animator.SetTrigger("Die");
  }

  public void TakeDamage(int amount)
  {
    hp = hp - amount;
  }

  float sightHeight = 1f;
  float sightDistance = 15f;
  float forgetDistance = 20f;
  LayerMask playerLayerMask;
  RaycastHit playerRaycastHit;

  bool PlayerDetected ()
  {
    bool playerSighted = Physics.Raycast(
      transform.position + new Vector3(0, sightHeight, 0),
      transform.TransformDirection(Vector3.forward),
      out playerRaycastHit,
      sightDistance,
      playerLayerMask
      );
    return playerSighted;
  }
  
  IEnumerator MovementMotor()
  {

    // todo: 
    // Add parameters for target waypoint system and starting waypoint
    // Add Pause functionality that returns from a defined location and continues from the last active target waypoint

    for (int i = 0; i < waypointSystem.waypoints.Count; i++)
    {
      waypointSystem.activeWaypointTarget = waypointSystem.waypoints[i];
      running = true;
      direction = (waypointSystem.waypoints[i].location - waypointSystem.waypoints[i].neighborNext.location).normalized;
      float distance = Vector3.Distance(waypointSystem.waypoints[i].location, waypointSystem.waypoints[i].neighborNext.location);
      float calculatedSpeed = distance / speed;

      // Debug.Log("Next Waypoint: " + waypointSystem.waypoints[i].neighborNext.name);
      // Debug.Log("Next Waypoint position: " + waypointSystem.waypoints[i].neighborNext.location);

      for (float j = 0; j < 1; j += Time.deltaTime / calculatedSpeed)
      {
        rB.MovePosition(Vector3.Lerp(waypointSystem.waypoints[i].location, waypointSystem.waypoints[i].neighborNext.location, j));
        yield return null;
      }
      running = false;
      yield return new WaitForSeconds(1);
    }
    inTransit = false;
  }
  
  void OnTriggerEnter(Collider other)
  {
    if (other.CompareTag("PlayerDamage") && PLAYERSingleton.i.playerIsAttacking)
    {
      UISingleton.i.debug.pushMessage(other.transform.root + " hit " + transform.name, "#ff3355");
      UISingleton.i.debug.pushMessage(transform.name + " took", "#ff3355", false);
      UISingleton.i.debug.pushMessage(" " + 10, "#ff3355", false);
      UISingleton.i.debug.pushMessage(" damage!", "#ff3355");
      Debug.Log(other.name + " hit " + transform.name);
      PLAYERSingleton.i.playerIsAttacking = false;

      TakeDamage(10);
    }
  }

  void FixedUpdate()
  {
  }

  void Update()
  {
    UpdateMode();
    UpdateAnimation();
  }

  private void UpdateMode()
  {
    if (mode == EnemyMode.Idle)
    {

    }
    if (mode == EnemyMode.Patrol)
    {
      Patrol();
    }
    if (mode == EnemyMode.Alert)
    {
      Debug.Log(playerRaycastHit.transform.name + " alerted " + transform.name);
    }
    if (mode == EnemyMode.Attack)
    {

    }
    if (mode == EnemyMode.Retreat)
    {

    }
  }

  void UpdateAnimation()
  {
    if (running) animator.SetBool("isRunning", true);
    else animator.SetBool("isRunning", false);
  }

}
