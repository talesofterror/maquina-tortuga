using UnityEngine;

[CreateAssetMenu(fileName = "SO_CameraSettings", menuName = "Scriptable Objects/SO_CameraSettings")]
public class SO_CameraSettings : ScriptableObject
{
  [Header("Primary Freelook Camera")]
  public float primaryFreelookRadius;
  public float primaryFreelookLens;

  [Header("Primary Close Freelook Camera")]
  public float primaryCloseFreelookRadius;
  public float primaryCloseFreelookLens;

  [Header("Primary Zoom Freelook Camera")]
  public float primaryZoomFreelookRadius;
  public float primaryZoomFreelookLens;

  [Header("Initial Camera Index")]
  public int initialCameraIndex;

}
