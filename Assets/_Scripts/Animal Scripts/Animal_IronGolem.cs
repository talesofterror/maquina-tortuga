using UnityEngine;
using System.Collections;

[SelectionBase]
public class Animal_IronGolem : MonoBehaviour, I_Animal
{

  public int hp { 
    get { return _hp; }
    set { _hp = value; }
  }
  public int _hp;

  public int _ap;
  public int ap {
    get { return _ap; }
    set { _ap = value; }
  }

  public int _mp;
  public int mp { 
    get { return _mp; }
    set { _mp = value; }
  }

  public float speed = 1f;
  public bool inTransit;

  WaypointSystem waypointSystem;

  Animator animator;
  Rigidbody rB;

  void Awake()
  {
    rB = GetComponentInChildren<Rigidbody>();
    animator = GetComponent<Animator>();
    waypointSystem = GetComponentInChildren<WaypointSystem>();
    Debug.Log(animator);
    hp = 10;
  }

  void FixedUpdate()
  {
    Walk();
  }
  void Update()
  {
    AnimateRun();
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
  }

  public void TakeDamage()
  {

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
      running = true;
      direction = (waypointSystem.waypoints[i].location - waypointSystem.waypoints[i].neighborNext.location).normalized;
      float distance = Vector3.Distance(waypointSystem.waypoints[i].location, waypointSystem.waypoints[i].neighborNext.location);
      float calculatedSpeed = distance / speed;
      
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
