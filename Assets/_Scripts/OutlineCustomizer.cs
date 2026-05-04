using UnityEngine;

public class OutlineCustomizer : MonoBehaviour
{
  public Color customColor = Color.blue;
  private Renderer _renderer;
  private MaterialPropertyBlock _propBlock;

  void Awake()
  {
    _renderer = GetComponent<Renderer>();
    _propBlock = new MaterialPropertyBlock();
  }

  void Update()
  {
    // 1. Get current properties from the renderer to avoid overwriting other blocks
    _renderer.GetPropertyBlock(_propBlock);

    // 2. Set the property used by your Renderer Feature shader
    _propBlock.SetColor("_BaseColor", customColor);

    // 3. Apply the updated block back to the renderer
    _renderer.SetPropertyBlock(_propBlock);
  }
}
