using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class uiDebugPanel : MonoBehaviour
{
    public bool isActive;
    public TextMeshProUGUI debugArea;
    public Scrollbar scrollbar;

    void Awake()
    {
        isActive = gameObject.activeSelf;
    }

    public void pushMessage(string message, string color = "white", bool newline = true)
    {
        string ending = newline ? "\n" : " ";
        debugArea.text += "<color=" + color + ">" + message + "</color>" + ending;
        scrollbar.value = 0;
    }
}
