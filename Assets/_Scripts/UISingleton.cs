using TMPro;
using UnityEngine;

public class UISingleton : MonoBehaviour
{
    private static UISingleton _uiSingleton;
    public static UISingleton i
    {
        get { return _uiSingleton; }
    }

    public uiDebugPanel debug;
    public TextMeshProUGUI hpText;

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

        RefreshUI();
    }

    public void RefreshUI()
    {
        hpText.text = PLAYERSingleton.i.playerHealth.hp.ToString();
    }
}
