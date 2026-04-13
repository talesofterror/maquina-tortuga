using PixelCrushers.DialogueSystem;
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

  public float interactableSelectionHeight = 50;

  // [HideInInspector]
  // public UIDialogueSelectorPanel selectorElements;

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
      // DontDestroyOnLoad(this.gameObject);
      // * prob don't need this ^
    }

    RefreshUI();
    
    // selectorElements = GetComponentInChildren<UIDialogueSelectorPanel>();
    // if (selectorElements.gameObject.activeSelf) selectorElements.gameObject.SetActive(true);
  }

  void OnEnable()
  {
    // PLAYERSingleton.i.dialogueSelector.SelectedUsableObject += OnTargetSelected;
    // PLAYERSingleton.i.dialogueSelector.DeselectedUsableObject += OnTargetDeselected;

    // DialogueManager.instance.conversationStarted += OnConversationStarted;
    // DialogueManager.instance.conversationEnded += OnConversationEnded;
  }

  void OnTargetSelected(Usable usable)
  {
    // selectorElements.gameObject.SetActive(true);
    // selectorElements.nameText.text = usable.overrideName;
    // selectorElements.useMessageText.text = PLAYERSingleton.i.dialogueSelector.defaultUseMessage;
  }

  void OnTargetDeselected(Usable usable)
  {
    // selectorElements.gameObject.SetActive(false);
  }

  private void OnConversationStarted(Transform actor)
  {
    // Debug.Log($"Conversation started with: {actor.name}");
    // Tools.SetGameObjectActive(selectorElements, false);
  }

  private void OnConversationEnded(Transform actor)
  {
    // Debug.Log($"Conversation ended with: {actor.name}");
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
