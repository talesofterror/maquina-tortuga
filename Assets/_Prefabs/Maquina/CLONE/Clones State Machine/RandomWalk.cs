using UnityEngine;

public class RandomWalk : MonoBehaviour
{
	[SerializeField] private Transform[] cornerTransforms = new Transform[0];

	[SerializeField, Min(1)] private int maxSamplingAttempts = 4096;

	public Transform PreviousWaypoint { get; private set; }
	public Transform CurrentWaypoint { get; private set; }
	public Transform FutureWaypoint { get; private set; }
	public float PreviousToCurrentDistance { get; private set; }
	public float CurrentToFutureDistance { get; private set; }

	private int waypointNumber;

	public Transform GetCurrentWaypoint()
	{
		if (CurrentWaypoint == null)
		{
			CurrentWaypoint = GenerateWaypoint();
			if (CurrentWaypoint == null)
			{
				return null;
			}

			FutureWaypoint = GenerateWaypoint();
			UpdateDistances();
			return CurrentWaypoint;
		}

		PreviousWaypoint = CurrentWaypoint;
		CurrentWaypoint = FutureWaypoint != null ? FutureWaypoint : GenerateWaypoint();
		if (CurrentWaypoint == null)
		{
			return null;
		}

		FutureWaypoint = GenerateWaypoint();
		UpdateDistances();
		return CurrentWaypoint;
	}

	private void OnDrawGizmos()
	{
		DrawBoundary();
		DrawWaypoint(PreviousWaypoint, Color.gray);
		DrawWaypoint(CurrentWaypoint, Color.green);
		DrawWaypoint(FutureWaypoint, Color.cyan);
	}

	private void DrawBoundary()
	{
		if (cornerTransforms == null || cornerTransforms.Length < 2)
		{
			return;
		}

		Color previousColor = Gizmos.color;
		Gizmos.color = Color.yellow;

		Transform previousCorner = null;
		Transform firstCorner = null;
		foreach (Transform corner in cornerTransforms)
		{
			if (corner == null)
			{
				continue;
			}

			if (firstCorner == null)
			{
				firstCorner = corner;
			}

			if (previousCorner != null)
			{
				Gizmos.DrawLine(previousCorner.position, corner.position);
			}

			previousCorner = corner;
		}

		if (previousCorner != null && firstCorner != null && previousCorner != firstCorner)
		{
			Gizmos.DrawLine(previousCorner.position, firstCorner.position);
		}

		Gizmos.color = previousColor;
	}

	private void DrawWaypoint(Transform waypoint, Color color)
	{
		if (waypoint == null)
		{
			return;
		}

		Color previousColor = Gizmos.color;
		Gizmos.color = color;
		Gizmos.DrawSphere(waypoint.position, 0.2f);
		Gizmos.color = previousColor;
	}

	private Transform GenerateWaypoint()
	{
		if (!TryGetBounds(out Vector2 minimum, out Vector2 maximum))
		{
			return null;
		}

		int attempts = Mathf.Max(1, maxSamplingAttempts);
		for (int attempt = 0; attempt < attempts; attempt++)
		{
			Vector2 point = new Vector2(
				Random.Range(minimum.x, maximum.x),
				Random.Range(minimum.y, maximum.y)
			);

			if (IsInsideBoundary(point))
			{
				GameObject waypoint = new GameObject($"Waypoint {++waypointNumber}");
				waypoint.transform.SetParent(transform, true);
				waypoint.transform.position = new Vector3(point.x, point.y, transform.position.z);
				return waypoint.transform;
			}
		}

		return null;
	}

	private bool TryGetBounds(out Vector2 minimum, out Vector2 maximum)
	{
		minimum = Vector2.zero;
		maximum = Vector2.zero;
		bool foundCorner = false;
		int cornerCount = 0;

		if (cornerTransforms == null)
		{
			return false;
		}

		foreach (Transform corner in cornerTransforms)
		{
			if (corner == null)
			{
				continue;
			}

			Vector2 point = corner.position;
			if (!foundCorner)
			{
				minimum = point;
				maximum = point;
				foundCorner = true;
			}
			else
			{
				minimum = Vector2.Min(minimum, point);
				maximum = Vector2.Max(maximum, point);
			}

			cornerCount++;
		}

		return foundCorner && cornerCount >= 3;
	}

	private bool IsInsideBoundary(Vector2 point)
	{
		bool inside = false;
		int previousIndex = cornerTransforms.Length - 1;

		for (int currentIndex = 0; currentIndex < cornerTransforms.Length; currentIndex++)
		{
			Transform currentCorner = cornerTransforms[currentIndex];
			Transform previousCorner = cornerTransforms[previousIndex];
			if (currentCorner != null && previousCorner != null)
			{
				Vector2 current = currentCorner.position;
				Vector2 previous = previousCorner.position;
				bool crossesRay = (current.y > point.y) != (previous.y > point.y);

				if (crossesRay && point.x < (previous.x - current.x) *
					(point.y - current.y) / (previous.y - current.y) + current.x)
				{
					inside = !inside;
				}
			}

			previousIndex = currentIndex;
		}

		return inside;
	}

	private void UpdateDistances()
	{
		PreviousToCurrentDistance = PreviousWaypoint != null && CurrentWaypoint != null
			? Vector3.Distance(PreviousWaypoint.position, CurrentWaypoint.position)
			: 0f;
		CurrentToFutureDistance = CurrentWaypoint != null && FutureWaypoint != null
			? Vector3.Distance(CurrentWaypoint.position, FutureWaypoint.position)
			: 0f;
	}
}
