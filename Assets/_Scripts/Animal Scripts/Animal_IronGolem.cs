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

  WaypointSystem waypointSystem;

  Animator animator;
  Rigidbody rB;

  void Awake()
  {
    waypointSystem = GetComponentInChildren<WaypointSystem>();
    mode = new EnemyMode();
    mode = EnemyMode.Patrol;
    rB = GetComponentInChildren<Rigidbody>();
    animator = GetComponent<Animator>();
    Debug.Log(animator);
  }


  void FixedUpdate()
  {
    if (mode == EnemyMode.Idle)
    {

    }
    if (mode == EnemyMode.Patrol)
    {
      Walk();
    }
    if (mode == EnemyMode.Attack)
    {

    }
    if (mode == EnemyMode.Retreat)
    {

    }

  }

  void Update()
  {
    AnimateRun();
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

  public void Walk()
  {
    if (!inTransit)
    {
      Debug.Log(rB);
      inTransit = true;
      waypointSystem.speed = speed;
      StartCoroutine(MovementMotor());
    }

    if (inTransit) transform.rotation = Quaternion.LookRotation(transform.position - waypointSystem.activeWaypointTarget.location);
  }

  public void TakeDamage(int amount)
  {
    hp = hp - amount;
  }

  public bool running;
  public Vector3 direction;
  void AnimateRun()
  {
    if (running) animator.SetBool("isRunning", true);
    else animator.SetBool("isRunning", false);
  }

  IEnumerator MovementMotor()
  {
    for (int i = 0; i < waypointSystem.waypoints.Count; i++)
    {
      waypointSystem.activeWaypointTarget = waypointSystem.waypoints[i];
      running = true;
      direction = (waypointSystem.waypoints[i].location - waypointSystem.waypoints[i].neighborNext.location).normalized;
      float distance = Vector3.Distance(waypointSystem.waypoints[i].location, waypointSystem.waypoints[i].neighborNext.location);
      float calculatedSpeed = distance / speed;

      Debug.Log("Next Waypoint: " + waypointSystem.waypoints[i].neighborNext.name);

      Vector3 lookAtTarget = new Vector3(waypointSystem.waypoints[i].neighborNext.transform.position.x, transform.position.y, waypointSystem.waypoints[i].neighborNext.transform.position.z);
      

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
}
