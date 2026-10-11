using UnityEngine;

public class PlaceGizmo : MonoBehaviour
{
    public enum GizmoShape
    {
        Sphere,
        WireSphere,
        Cube,
        WireCube
    }

    [SerializeField] private GizmoShape shape = GizmoShape.Sphere;
    [SerializeField, Min(0f)] private float size = 1f;
    [SerializeField] private Color color = Color.yellow;
    [SerializeField] private bool selectedOnly;

    private void OnDrawGizmos()
    {
        if (!selectedOnly)
        {
            DrawGizmo();
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (selectedOnly)
        {
            DrawGizmo();
        }
    }

    private void DrawGizmo()
    {
        Color previousColor = Gizmos.color;
        Gizmos.color = color;

        switch (shape)
        {
            case GizmoShape.Sphere:
                Gizmos.DrawSphere(transform.position, size);
                break;
            case GizmoShape.WireSphere:
                Gizmos.DrawWireSphere(transform.position, size);
                break;
            case GizmoShape.Cube:
                Gizmos.DrawCube(transform.position, Vector3.one * size);
                break;
            case GizmoShape.WireCube:
                Gizmos.DrawWireCube(transform.position, Vector3.one * size);
                break;
        }
 
        Gizmos.color = previousColor;
    }
}
