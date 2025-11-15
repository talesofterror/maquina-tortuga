using UnityEngine;

public interface I_Platform
{
  public bool activated { get; set; }
  public WaypointSystem waypointSystem { get; set; }
  public void Movement();
}
