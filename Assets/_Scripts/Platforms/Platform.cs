using System.Collections;
using UnityEngine;

public class Platform : MonoBehaviour, I_Platform
{
    [SerializeField]
    public float speed;

    [SerializeField]
    public bool platformEnabled = true;

    [SerializeField]
    float pauseTime = 1;

    public bool activated { get; set; }
    public WaypointSystem waypointSystem { get; set; }
    public Transform yFloor;

    private Vector3 _previousPosition;
    private Vector3 _velocity;

    private Rigidbody rB;

    [HideInInspector]
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
            float distance = Vector3.Distance(
                waypointSystem.waypoints[i].location,
                waypointSystem.waypoints[i].neighborNext.location
            );
            float calculatedSpeed = distance / speed;
            for (float j = 0; j < 1; j += Time.deltaTime / calculatedSpeed)
            {
                rB.MovePosition(
                    Vector3.Lerp(
                        waypointSystem.waypoints[i].location,
                        waypointSystem.waypoints[i].neighborNext.location,
                        j
                    )
                );
                yield return null;
            }
            yield return new WaitForSeconds(pauseTime);
        }
        inTransit = false;
    }

    public Vector3 CalculateVelocity()
    {
        return _velocity;
    }

    void Awake()
    {
        waypointSystem = GetComponentInChildren<WaypointSystem>();
        rB = GetComponent<Rigidbody>();
        activated = true;
    }

    void FixedUpdate()
    {
        if (platformEnabled && activated)
        {
            Vector3 displacement = rB.position - _previousPosition;
            _velocity = displacement / Time.deltaTime;
            _previousPosition = rB.position;
        }

        Movement();
    }

    void Update() { }
}
