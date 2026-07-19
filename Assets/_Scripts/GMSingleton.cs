using System.Numerics;
using PixelCrushers.DialogueSystem;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GMSingleton : MonoBehaviour
{
  private static GMSingleton _gmSingleton;
  public static GMSingleton i
  {
    get { return _gmSingleton; }
  }

  [HideInInspector]
  public bool gameStarted;

  // [Header("Controls")]
  // public PlayerInput primaryPlayerInput;

  [Header("Singletons")]
  public UISingleton userInterface;
  public PLAYERSingleton player;
  public CAMERASingleton cameraSingleton;
  public SCENEManagement sceneManagement;
  public InputManager inputManager;
  public PrefabManager prefabManager;

  [Header("Interaction")]
  public Interaction currentInteraction;
  public float defaultInteractionDistance = 1;

  [Header("Scene Management")]
  public SceneSpawnPoint spawnPoint;
  public Scene activeScene;
  public Scene lastScene;

  void Awake()
  {
    if (_gmSingleton != null && _gmSingleton != this)
    {
      Destroy(this.gameObject);
      return;
    }
    else
    {
      _gmSingleton = this;
      DontDestroyOnLoad(this.gameObject);
    }

    userInterface.gameObject.SetActive(true);
    gameStarted = true;

    PixelCrushers.CursorControl.ShowCursor(false);
    PixelCrushers.CursorControl.LockCursor(true);
    // Cursor.visible = false;
    // Cursor.lockState = CursorLockMode.Confined;


  }

  void OnEnable()
  {
    DialogueManager.instance.conversationEnded += OnDialogueEnded;
    SceneManager.sceneLoaded += OnSceneLoaded;

  }

  private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
  {
    Lua.RegisterFunction("LoadScene", this, SymbolExtensions.GetMethodInfo(() => DialogueFunction_LoadScene()));
    Lua.RegisterFunction("EndConvo", this, SymbolExtensions.GetMethodInfo(() => DialogueFunction_EndConvo()));
  }

  void OnDisable()
  { 
    SceneManager.sceneLoaded -= OnSceneLoaded;
    
    if (DialogueManager.instance != null)
      DialogueManager.instance.conversationEnded -= OnDialogueEnded;

    Lua.UnregisterFunction("LoadScene");
    Lua.UnregisterFunction("EndConvo");
  }

  private void OnDialogueEnded(Transform actor)
  {
    Debug.Log("Global Hook: Dialogue finished.");
    PLAYERSingleton.i.stateController.BeNormal();
  }

  public void DialogueFunction_LoadScene()
  {
    currentInteraction.i.gameObject.GetComponent<SceneLoader>().loadLevel();
  }

  public void DialogueFunction_EndConvo()
  {
    DialogueManager.StopAllConversations();
    PLAYERSingleton.i.stateController.BeNormal();
  }


  void Start()
  {
  }

  public void LoadScene()
  {
    GMSingleton.i.currentInteraction.i.gameObject.GetComponent<SceneLoader>().loadLevel();
  }

  void OnDestroy()
  {
    if (_gmSingleton == this)
    {
      _gmSingleton = null;
    }
  }

}
