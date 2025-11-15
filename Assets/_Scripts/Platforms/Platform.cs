using UnityEngine;
using System.Collections;

public class Platform : MonoBehaviour, I_Platform
{
  public float speed;
  public bool platformEnabled = true;
  [SerializeField] public bool activated { get; set; }
  public WaypointSystem waypointSystem { get; set; }
  public Transform yFloor;

  private Vector3 _previousPosition;
  private Vector3 _velocity;

  private Rigidbody rB;


  public bool inTransit;

  public void Movement()
  {
    if (!inTransit)
    {
      inTransit = true;
      waypointSystem.speed = speed;
      StartCoroutine(MovementMotor());
    }
  }

  IEnumerator MovementMotor()
  {

    for (int i = 0; i < waypointSystem.waypoints.Count; i++)
    {
      float distance = Vector3.Distance(waypointSystem.waypoints[i].location, waypointSystem.waypoints[i].neighborNext.location);
      float calculatedSpeed = distance / speed;
      for (float j = 0; j < 1; j += Time.deltaTime / calculatedSpeed)
      {
        rB.MovePosition(Vector3.Lerp(waypointSystem.waypoints[i].location, waypointSystem.waypoints[i].neighborNext.location, j));
        yield return null;
      }
      yield return new WaitForSeconds(1);
    }
    inTransit = false;
  }

  public Vector3 CalculateVelocity()
  {
    return _velocity;
  }

  // Start is called once before the first execution of Update after the MonoBehaviour is created
  void Awake()
  {
    waypointSystem = GetComponentInChildren<WaypointSystem>();
    rB = GetComponent<Rigidbody>();
    activated = true;
  }

  // Update is called once per frame
  void FixedUpdate()
  {
    if (platformEnabled)
    {
      Vector3 displacement = rB.position - _previousPosition;
      _velocity = displacement / Time.deltaTime;
      _previousPosition = rB.position;
    }

    Movement();
  }

  void Update()
  {

  }
}
