using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class SCENEManagement : MonoBehaviour
{
  // private static SCENEManagement _sceneManagement;
  // public static SCENEManagement i { get { return _sceneManagement; } }

  [SerializeField] public Scene[] Scenes;
  [HideInInspector] public Transform spawnTransform;


  void Awake()
  {
  //   if (_sceneManagement != null && _sceneManagement != this)
  //   {
  //     Destroy(this.gameObject);
  //   }
  //   else
  //   {
  //     _sceneManagement = this;
  //     DontDestroyOnLoad(this.gameObject);
  //   }
  }

}