using NUnit.Framework.Constraints;
using Palmmedia.ReportGenerator.Core;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class CAMERASingleton : MonoBehaviour
{
  private static CAMERASingleton _cameraSingleton;
  public static CAMERASingleton i
  {
    get { return _cameraSingleton; }
  }

  public Camera mainCamera;
  public CinemachineCamera primaryFreelook;
  public CinemachineCamera primaryCloseFreelook;
  public CinemachineCamera primaryZoomFreelook;
  CinemachineOrbitalFollow primaryFreeLookOrbital;
  CinemachineOrbitalFollow primaryCloseFreelookOrbital;
  CinemachineOrbitalFollow primaryZoomFreelookOrbital;
  public Camera uiCamera;

  public CinemachineCamera[] cameraArray;

  [SerializeField]
  private int initialCameraIndex = 1;

  [HideInInspector]
  public int currentCameraIndex = 0;

  [HideInInspector]
  public int previousCameraIndex = 0;

  public bool cameraSwitchDisabled = false;
  public bool zooming;

  public GameObject effectVolume;
  public Volume effectVolumeComponent;

  void Awake()
  {
    if (_cameraSingleton != null && _cameraSingleton != this)
    {
      Destroy(this.gameObject);
      return;
    }
    else
    {
      _cameraSingleton = this;
      DontDestroyOnLoad(this.gameObject);
    }

    primaryFreeLookOrbital = primaryFreelook.GetComponent<CinemachineOrbitalFollow>();
    primaryCloseFreelookOrbital = primaryCloseFreelook.GetComponent<CinemachineOrbitalFollow>();
    primaryZoomFreelookOrbital = primaryZoomFreelook.GetComponent<CinemachineOrbitalFollow>();

    effectVolume.SetActive(false);
    effectVolumeComponent = effectVolume.GetComponent<Volume>();

  }

  void OnEnable()
  {
    SceneManager.sceneLoaded += OnSceneLoaded;
  }

  void OnDisable()
  {
    SceneManager.sceneLoaded -= OnSceneLoaded;
  }

  void OnSceneLoaded(Scene scene, LoadSceneMode mode)
  {
    InitializeLevelSettings();
  }

  void setCurrentCamera(int index)
  {
    if (index < 0)
    {
      cameraArray[currentCameraIndex].enabled = false;
      return;
    }

    for (int i = 0; i < cameraArray.Length; i++)
    {
      cameraArray[i].Target.TrackingTarget = GMSingleton.i.player.cameraTargetGameobject.transform;
      if (i == index)
      {
        cameraArray[i].enabled = true;
      }
      else
      {
        cameraArray[i].enabled = false;
      }
    }
  }

  void OnDestroy()
  {
    if (_cameraSingleton == this)
    {
      _cameraSingleton = null;
    }
  }

  void Update()
  {
    // if (GMSingleton.i.inputManager.zoom.IsPressed())
    // {
    //   zooming = true;
    //   cachedActiveCameraIndex = currentCameraIndex;
    //   setCurrentCamera(-2); // Assuming the zoom camera is at index 2
    //   primaryZoomFreelook.GetComponent<CinemachineOrbitalFollow>().HorizontalAxis =
    //       cameraArray[cachedActiveCameraIndex].GetComponent<CinemachineOrbitalFollow>().HorizontalAxis;
    // }
    // else
    // {
    //   zooming = false;
    //   setCurrentCamera(currentCameraIndex);
    // }

    if (!cameraSwitchDisabled)
    {
      if (GMSingleton.i.inputManager.nextCamera.WasReleasedThisFrame())
      {
        previousCameraIndex = currentCameraIndex;
        currentCameraIndex = (currentCameraIndex + 1) % cameraArray.Length;
        setCurrentCamera(currentCameraIndex);
        cameraArray[currentCameraIndex]
            .GetComponent<CinemachineOrbitalFollow>()
            .HorizontalAxis = cameraArray[previousCameraIndex]
            .GetComponent<CinemachineOrbitalFollow>()
            .HorizontalAxis;
        UISingleton.i.debug.pushMessage(
            "Active Camera set to: " + cameraArray[currentCameraIndex].name,
            "#c9d039ff"
        );
      }

      if (GMSingleton.i.inputManager.prevCamera.WasReleasedThisFrame())
      {
        previousCameraIndex = currentCameraIndex;
        currentCameraIndex = (currentCameraIndex - 1 + cameraArray.Length) % cameraArray.Length;
        setCurrentCamera(currentCameraIndex);
        cameraArray[currentCameraIndex]
            .GetComponent<CinemachineOrbitalFollow>()
            .HorizontalAxis = cameraArray[previousCameraIndex]
            .GetComponent<CinemachineOrbitalFollow>()
            .HorizontalAxis;
        UISingleton.i.debug.pushMessage(
            "Active Camera set to: " + cameraArray[currentCameraIndex].name,
            "#c9d039ff"
        );
      }
    }


  }

  public void InitializeLevelSettings()
  {
    SO_CameraSettings settings = GameObject.FindWithTag("CameraSettings").GetComponent<CameraSettings>().settings;

    if (settings != null)
    {
      primaryFreelook.Lens.FieldOfView = settings.primaryFreelookLens;
      primaryCloseFreelookOrbital.Radius = settings.primaryFreelookRadius;

      primaryCloseFreelook.Lens.FieldOfView = settings.primaryCloseFreelookLens;
      primaryCloseFreelookOrbital.Radius = settings.primaryCloseFreelookRadius;

      primaryZoomFreelook.Lens.FieldOfView = settings.primaryZoomFreelookLens;
      primaryZoomFreelookOrbital.Radius = settings.primaryZoomFreelookRadius;

      setCurrentCamera(settings.initialCameraIndex);
    }
    else
    { Debug.Log("missing CameraSettings gameobject or scriptable object"); }
  }

  void FixedUpdate() { }

  void OnDrawGizmosSelected() { }

  void Start() { }
}
