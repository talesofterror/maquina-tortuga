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
  Coroutine initPlayerDetected;

  Animator animator;
  string activeAnimationString;
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

  private bool returningFromAlert;

  public void Patrol()
  {
    if (_prevMode != EnemyMode.Patrol)
    {
      _prevMode = EnemyMode.Patrol;
    }

    if (!inTransit)
    {
      inTransit = true;
      waypointSystem.speed = speed;
      if (movementMotorCoroutine == null)
      {
        if (returningFromAlert)
        {
          movementMotorCoroutine = StartCoroutine(MovementMotor(transform.position));
          returningFromAlert = false;
        }
        else movementMotorCoroutine = StartCoroutine(MovementMotor());
        initPlayerDetected = null;
      }
    }

    if (direction.sqrMagnitude > 0)
      transform.rotation = Quaternion.LookRotation(-direction);

    if (PlayerDetected())
    {
      Debug.Log(transform.name + " detected the player!");
      mode = EnemyMode.Alert;
      StopCoroutine(movementMotorCoroutine);
      inTransit = false;
      running = false;
      if (initPlayerDetected == null)
      {
        movementMotorCoroutine = null;
        initPlayerDetected = StartCoroutine(InitPlayerDetected());
      }
    }
  }

  IEnumerator InitPlayerDetected()
  {
    transform.LookAt(PLAYERSingleton.i.transform.position);
    yield return null;
  }

  public EnemyMode _prevMode;

  public void Alert()
  {
    if (_prevMode != EnemyMode.Alert)
    {
      Debug.Log(transform.name + " was alerted by " + PLAYERSingleton.i.name);
      _prevMode = EnemyMode.Alert;
    }

    transform.LookAt(PLAYERSingleton.i.transform.position);

    // * forgetting behavior
    if (Vector3.Distance(transform.position, PLAYERSingleton.i.transform.position) > forgetDistance)
    {
      mode = EnemyMode.Patrol;
      returningFromAlert = true;
    }
  }

  IEnumerator initReturnToPatrol()
  {

    yield return null;
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

  bool PlayerDetected()
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

  IEnumerator MovementMotor(Vector3? interruptVector = null) // nullable value type
  {

    // todo: 
    // Add parameters for target waypoint system and starting waypoint
    // Add Pause functionality that returns from a defined location and continues from the last active target waypoint

    int activeIndex;

    if (waypointSystem.activeWaypointTarget == null)
    {
      activeIndex = 0;
    }
    else
    {
      activeIndex = waypointSystem.activeWaypointTarget.index;
    }

    for (int i = activeIndex; i < waypointSystem.waypoints.Count; i++)
    {
      Vector3 originVector;
      if (interruptVector.HasValue)
      {
        originVector = interruptVector.GetValueOrDefault(); // nullable value type
        interruptVector = null;
      } 
      else originVector = waypointSystem.waypoints[i].location;
      waypointSystem.activeWaypointTarget = waypointSystem.waypoints[i];
      running = true;
      direction = (originVector - waypointSystem.waypoints[i].neighborNext.location).normalized;
      float distance = Vector3.Distance(originVector, waypointSystem.waypoints[i].neighborNext.location);
      float calculatedSpeed = distance / speed;

      // Debug.Log("Next Waypoint: " + waypointSystem.waypoints[i].neighborNext.name);
      // Debug.Log("Next Waypoint position: " + waypointSystem.waypoints[i].neighborNext.location);

      for (float j = 0; j < 1; j += Time.deltaTime / calculatedSpeed)
      {
        rB.MovePosition(Vector3.Lerp(originVector, waypointSystem.waypoints[i].neighborNext.location, j));
        yield return null;
      }
      running = false;
      yield return new WaitForSeconds(1);
    }
    inTransit = false;
    movementMotorCoroutine = null;
    waypointSystem.activeWaypointTarget = null;
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
      Alert();
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

  // void setAnimationBool(string prev, string next) {
  //   animator.SetBool(prev, false);
  //   animator.SetBool(next, true);
  // }

}
