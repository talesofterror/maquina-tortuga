using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
  public int level;
  public string sceneToLoad;
  public bool isInitialSpawnPoint = true;

  public Transform spawnPoint;

  void Awake()
  {
    SceneManager.sceneLoaded += OnSceneLoaded;
    SceneManager.sceneUnloaded += OnSceneUnloaded;
  }

  void OnSceneLoaded (Scene scene, LoadSceneMode mode)
  {
    UISingleton.i.debug.pushMessage("SceneLoader called <b>OnSceneLoaded(" + scene.name + ")</b>", "#44cc22");
    
    if (isInitialSpawnPoint) PLAYERSingleton.i.transform.position = spawnPoint.position;
    PLAYERSingleton.i.rB.isKinematic = false;
  }

  void OnSceneUnloaded(Scene scene)
  {
    UISingleton.i.debug.pushMessage("SceneLoader called <b>OnSceneUnloaded(" + scene.name + ")</b>","#44cc99");
    PLAYERSingleton.i.rB.isKinematic = true;
  }

  void OnDestroy()
  {
    SceneManager.sceneLoaded -= OnSceneLoaded;
    SceneManager.sceneUnloaded -= OnSceneUnloaded;
  }

  public void loadLevel()
  {
    SceneManager.LoadScene(sceneToLoad, LoadSceneMode.Single);
  }

}
