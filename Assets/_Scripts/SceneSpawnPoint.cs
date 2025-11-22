using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSpawnPoint : MonoBehaviour
{
  string _name;
  Scene thisScene;
  public bool isActiveSpawnPoint = true;
  Transform respawnPoint;

  void Awake()
  {
    SceneManager.sceneLoaded += OnSceneLoaded;
    SceneManager.sceneUnloaded += OnSceneUnloaded;

    GameObject respawnObj = GameObject.FindGameObjectWithTag("Respawn");
    if (respawnObj != null) respawnPoint = respawnObj.transform;

    Debug.Log("SceneSpawnPoint called Awake()");
  }

  void Start()
  {
    GMSingleton.i.spawnPoint = this;
    _name = transform.name;
    
    if (respawnPoint != null && PLAYERSingleton.i != null)
    {
        PLAYERSingleton.i.transform.position = respawnPoint.position;
    }
  }

  void OnDestroy()
  {
    SceneManager.sceneLoaded -= OnSceneLoaded;
    SceneManager.sceneUnloaded -= OnSceneUnloaded;
    GMSingleton.i.lastScene = thisScene;
  }

  void OnSceneLoaded(Scene scene, LoadSceneMode mode)
  {
    thisScene = scene;
    SceneManager.SetActiveScene(scene);
    GMSingleton.i.activeScene = scene;
    if (!GMSingleton.i.gameStarted)
    {
      GMSingleton.i.gameStarted = true;
    }
    else
    {
      SceneManager.UnloadSceneAsync(GMSingleton.i.lastScene);
    }
  }

  void OnSceneUnloaded(Scene currentScene)
  {
  }

}
