using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class uiDebugPanel : MonoBehaviour
{
  // // public Text line1;
  public TextMeshProUGUI line1;
  public TextMeshProUGUI line2;
  public TextMeshProUGUI line3;
  public TextMeshProUGUI line4;
  public TextMeshProUGUI line5;
  public TextMeshProUGUI line6;
  public TextMeshProUGUI debugArea;
  int testNumber = 0;

  public void pushMessage(string message, string color = "white", bool newline = true)
  {
    string ending = newline ? "\n" : " ";
    debugArea.text += "<color=" + color + ">" + message + "</color>" + ending;
  }

}
