using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

public class WaypointSystem : MonoBehaviour
{

  public Rigidbody hostRB;
  public Transform hostTransform;
  public Waypoint[] waypointArray;
  public List<Waypoint> waypoints;
  public Waypoint self;
  [HideInInspector] public Waypoint activeWaypointTarget;

  public float speed = 0.5f;
  float gizmoSize = 0.5f;
  public WaypointSystemMode mode;
  public bool pauseOnWaypoint = true;
  public bool ignoreY;

  [HideInInspector] public bool inTransit = false;

  void OnValidate()
  {
    waypointArray = GetComponentsInChildren<Waypoint>();
    waypoints = waypointArray.ToList();
  }

  void Start()
  {
    // hostRB = GetComponentInParent<Rigidbody>();
    // hostTransform = GetComponentInParent<Transform>();
    mode = new WaypointSystemMode();
    mode = WaypointSystemMode.Loop;
    initWaypoints();
  }

  void OnDrawGizmos()
  {
    gizmoDrawLines(waypointArray);
  }

  private void initWaypoints()
  {
    self = GetComponent<Waypoint>();

    for (int i = 0; i < waypoints.Count; i++)
    {
      waypoints[i].SetLocation(this);
      waypoints[i].name = "Wayoint " + i;
    }

    for (int i = 0; i < waypoints.Count; i++)
    {
      waypoints[i].origin = transform.position;

      waypoints[i].gizmoSize = waypoints[i].origin == waypoints[i].location
        ? gizmoSize
        : waypoints[i].gizmoSize;

      waypoints[i].neighborNext = i == waypoints.Count - 1
        ? waypoints[0]
        : waypoints[i + 1];

      waypoints[i].neighborPrevious = i == 0
        ? waypoints[waypoints.Count - 1]
        : waypoints[i - 1];

      // Debug.Log($"Waypoint {i}: " + waypoints[i].name);
      // Debug.Log($"Waypoint {i} origin: " + waypoints[i].origin);
      // Debug.Log($"Waypoint {i} neighorFront: " + waypoints[i].neighborNext.location);
      // Debug.Log($"Waypoint {i} neighborBehind: " + waypoints[i].neighborPrevious.location);
    }
  }

  private void gizmoDrawLines(Waypoint[] waypointArray)
  {
    for (int i = 0; i < waypointArray.Length - 1; i++)
    {
      Gizmos.DrawLine(waypointArray[i].gameObject.transform.position, waypointArray[i + 1].gameObject.transform.position);
    }
  }

}
