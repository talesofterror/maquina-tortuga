using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class Waypoint : MonoBehaviour
{
  public float gizmoSize = 0.5f;
  [HideInInspector] public Vector3 location;
  [HideInInspector] public Vector3 origin;
  [HideInInspector] public Waypoint neighborNext;
  [HideInInspector] public Waypoint neighborPrevious;

  public void SetLocation(WaypointSystem hostSystem)
  {
      location = new Vector3(transform.position.x, transform.position.y, transform.position.z);
  }

  void OnDrawGizmos()
  {
    Gizmos.color = Color.blue;
    Handles.Label(transform.position, "Waypoint");
    Gizmos.DrawSphere(transform.position, gizmoSize);
  }

}
