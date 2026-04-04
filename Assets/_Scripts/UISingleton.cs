using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UISingleton : MonoBehaviour
{
    private static UISingleton _uiSingleton;
    public static UISingleton i
    {
        get { return _uiSingleton; }
    }

    public EventSystem eventSystem;
    public uiDebugPanel debug;
    public TextMeshProUGUI hpText;
    public Selectable DialoguePrev;
    public Selectable DialogueNext;

    void Awake()
    {
        if (_uiSingleton != null && _uiSingleton != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            _uiSingleton = this;
            DontDestroyOnLoad(this.gameObject);
        }

        RefreshUI();
        eventSystem.SetSelectedGameObject(debug.scrollbar.gameObject);
    }

    void OnDestroy()
    {
        if (_uiSingleton == this)
        {
            _uiSingleton = null;
        }
    }

    public void RefreshUI()
    {
        hpText.text = PLAYERSingleton.i.playerHealth.hp.ToString();
    }
}
