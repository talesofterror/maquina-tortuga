using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CAMERASingleton : MonoBehaviour
{

  private static CAMERASingleton _cameraSingleton;
  public static CAMERASingleton i { get { return _cameraSingleton; } }

  public Camera mainCamera;
  public CinemachineCamera primaryFreelook;
  public CinemachineCamera primaryZoomFreelook;
  public Camera uiCamera;

  void Awake()
  {
    if (_cameraSingleton != null && _cameraSingleton != this)
    {
      Destroy(this.gameObject);
    }
    else
    {
      _cameraSingleton = this;
      DontDestroyOnLoad(this.gameObject);
    }
  }

  void FixedUpdate()
  {
    if (GMSingleton.i.inputManager.zoom.IsPressed())
    {
      primaryFreelook.enabled = false;
      primaryZoomFreelook.GetComponent<CinemachineOrbitalFollow>().HorizontalAxis = primaryFreelook.GetComponent<CinemachineOrbitalFollow>().HorizontalAxis;
    }
    else
    {
      primaryFreelook.enabled = true;
    }
  }

  void OnDrawGizmosSelected()
  {
  }

  void Start()
  {

  }

  void Update()
  {

  }

}
