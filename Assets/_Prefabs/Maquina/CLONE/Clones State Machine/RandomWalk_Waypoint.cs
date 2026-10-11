using UnityEngine;

public class RandomWalk_Waypoint
{
	public Transform transform { get; set; }
	public FSM_Base state { get; set; }
	public FSM_Base subState { get; set; }

	public RandomWalk_Waypoint(Transform transform, FSM_Base substate = null, FSM_Base state = null)
	{
		this.transform = transform;
		this.subState = substate;
		this.state = state;
	}
}