using UnityEngine;

public class UISingleton : MonoBehaviour
{
  private static UISingleton _uiSingleton;
  public static UISingleton i { get { return _uiSingleton; } }


  public uiDebugPanel debug;

  void Awake()
  {
    if (_uiSingleton != null && _uiSingleton != this)
    {
      Destroy(this.gameObject);
    }
    else
    {
      _uiSingleton = this;
      DontDestroyOnLoad(this.gameObject);
    }
  }

}
