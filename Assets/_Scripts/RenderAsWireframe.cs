using UnityEngine;

public class RenderAsWireframe : MonoBehaviour
{
  // Start is called once before the first execution of Update after the MonoBehaviour is created
  void Start()
  {
    MeshFilter meshFilter = GetComponent<MeshFilter>();
    Mesh mesh = meshFilter.mesh;

    // Get original triangle indices
    int[] oldIndices = mesh.GetIndices(0);

    // Convert to lines by converting every 3 triangle indices to lines
    // A simple wireframe usually needs custom line indices.
    // For a quick fix, this reorders triangle indices to lines:
    int[] lineIndices = new int[oldIndices.Length * 2];
    for (int i = 0; i < oldIndices.Length; i += 3)
    {
      lineIndices[i * 2] = oldIndices[i];
      lineIndices[i * 2 + 1] = oldIndices[i + 1];
      lineIndices[i * 2 + 2] = oldIndices[i + 1];
      lineIndices[i * 2 + 3] = oldIndices[i + 2];
      lineIndices[i * 2 + 4] = oldIndices[i + 2];
      lineIndices[i * 2 + 5] = oldIndices[i];
    }

    // Set the new topology and indices
    mesh.SetIndices(lineIndices, MeshTopology.Lines, 0);
  }

}
