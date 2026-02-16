using UnityEngine;

[ExecuteInEditMode]
public class InstancingCubes : MonoBehaviour
{
    // TODO: For grass: Use Terrain.GetHeights() to get the height of the terrain at each point.
    // TODO: For grass: Use Terrain.GetAlphamaps() to get the texture of the terrain at each point.

    [SerializeField]
    int size = 10;

    [SerializeField]
    Mesh mesh;

    [SerializeField]
    Material material;

    private Matrix4x4[] matrices;
    private Vector4[] colors;

    void Start() { }

    void Update()
    {
        matrices = new Matrix4x4[size * size * size];
        colors = new Vector4[size * size * size];

        int i = 0;

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                for (int z = 0; z < size; z++)
                {
                    matrices[i] = Matrix4x4.TRS(
                        new Vector3(
                            x * 2 + transform.position.x,
                            y * 2 + Mathf.Sin(Time.time + x + z) + transform.position.y,
                            z * 2 + transform.position.z
                        ),
                        Quaternion.identity,
                        Vector3.one
                    );
                    i++;
                }
            }
        }

        RenderParams renderParams = new RenderParams(material);
        renderParams.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
        renderParams.receiveShadows = true;

        Graphics.RenderMeshInstanced(renderParams, mesh, 0, matrices);
        // Graphics.DrawMeshInstanced(mesh, 0, material, matrices);
    }
}
