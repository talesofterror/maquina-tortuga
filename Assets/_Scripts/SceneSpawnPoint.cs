using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSpawnPoint : MonoBehaviour
{
  string _name;
  Scene thisScene;
  public bool isActiveSpawnPoint = true;
  void Awake()
  {
    // PlayerPrefs.SetFloat("SpawnX", transform.position.x);
    // PlayerPrefs.SetFloat("SpawnY", transform.position.y);
    // PlayerPrefs.SetFloat("SpawnZ", transform.position.z);
    // PlayerPrefs.Save();

    SceneManager.sceneLoaded += OnSceneLoaded;
    SceneManager.sceneUnloaded += OnSceneUnloaded;

    Debug.Log("SceneSpawnPoint called Awake()");
  }

  void Start()
  {
    Debug.Log("SceneSpawnPoint called Start()");
    GMSingleton.i.spawnPoint = this;
    _name = transform.name;
    GameObject.FindGameObjectWithTag("Player").transform.position = GameObject.FindGameObjectWithTag("Respawn").transform.position;
    Debug.Log("SP: Start - Spawn object transform position: " + GameObject.FindGameObjectWithTag("Respawn").transform.position);
    Debug.Log("SP: Start - Player singleton transform position: " + PLAYERSingleton.i.transform.position);
    Debug.Log("SP: Start - Player tag transform position: " + GameObject.FindGameObjectWithTag("Player").transform.position);
  }

  void OnEnable()
  {
    Debug.Log("SceneSpawnPoint called OnEnable()");
    Debug.Log("SP: OnEnable - Spawn object transform position: " + GameObject.FindGameObjectWithTag("Respawn").transform.position);
    Debug.Log("SP: OnEnable - Player singleton transform position: " + PLAYERSingleton.i.transform.position);
    Debug.Log("SP: OnEnable - Player tag transform position: " + GameObject.FindGameObjectWithTag("Player").transform.position);
  }

  void Update()
  {

  } 

  void OnDestroy()
  {
    Debug.Log("SceneSpawnPoint called OnDestroy()");
    SceneManager.sceneLoaded -= OnSceneLoaded;
    SceneManager.sceneUnloaded -= OnSceneUnloaded;
    GMSingleton.i.lastScene = thisScene;
  }

  void OnSceneLoaded(Scene scene, LoadSceneMode mode)
  {
    thisScene = scene;
    SceneManager.SetActiveScene(scene);
    GMSingleton.i.activeScene = scene;
    // StartCoroutine(DeferredOnSceneLoaded());
    Debug.Log("SceneSpawnPoint(" + _name + ") called OnSceneLoaded");
    if (!GMSingleton.i.gameStarted)
    {
      GMSingleton.i.gameStarted = true;
    }
    else
    {
      SceneManager.UnloadSceneAsync(GMSingleton.i.lastScene);
    }
  }

  IEnumerator DeferredOnSceneLoaded()
  {
    yield return null;
  }

  void OnSceneUnloaded(Scene currentScene)
  {
    // PLAYERSingleton.i.transform.position = transform.position;
    Debug.Log("SceneSpawnPoint called OnSceneUnloaded(" + currentScene.name + ")");
  }

}
