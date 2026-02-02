using Unity.Cinemachine;
using UnityEngine;
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
    public Camera uiCamera;

    public CinemachineCamera[] cameraArray;

    [SerializeField]
    private int initialCameraIndex = 1;

    [HideInInspector]
    public int currentCameraIndex = 0;

    [HideInInspector]
    public int previousCameraIndex = 0;

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

        setCurrentCamera(initialCameraIndex);
    }

    void setCurrentCamera(int index)
    {
        for (int i = 0; i < cameraArray.Length; i++)
        {
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
        if (GMSingleton.i.inputManager.zoom.IsPressed())
        {
            cameraArray[currentCameraIndex].enabled = false;
            primaryZoomFreelook.GetComponent<CinemachineOrbitalFollow>().HorizontalAxis =
                primaryFreelook.GetComponent<CinemachineOrbitalFollow>().HorizontalAxis;
        }
        else
        {
            cameraArray[currentCameraIndex].enabled = true;
        }

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

    void FixedUpdate() { }

    void OnDrawGizmosSelected() { }

    void Start() { }
}
