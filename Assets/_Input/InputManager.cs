using Invector.vCharacterController;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public InputSystem_Actions inputSystem;
    public InputAction interaction;
    public InputAction attack;
    public InputAction zoom;
    public InputAction modeChange;
    public InputAction disableMouseInput;
    public InputAction nextCamera;
    public InputAction prevCamera;
    public InputAction toggleDebugPanel;
    public vThirdPersonInput vInput;

    void OnEnable() { }

    void Start()
    {
        inputSystem = new InputSystem_Actions();
        inputSystem.Enable();

        vInput = PLAYERSingleton.i.vInput;

        interaction = inputSystem.Player.Interact;
        attack = inputSystem.Player.Attack;
        zoom = inputSystem.Player.Zoom;
        modeChange = inputSystem.Player.ModeChange;
        disableMouseInput = inputSystem.Player.DisableMouseInput;
        nextCamera = inputSystem.Player.NextCamera;
        prevCamera = inputSystem.Player.PrevCamera;
        toggleDebugPanel = inputSystem.UI.ToggleDebugPanel;

        UISingleton.i.debug.pushMessage(
            "Move: "
                + InputActionRebindingExtensions.GetBindingDisplayString(vInput.inputAction_Move),
            "#ffaacc"
        );
        UISingleton.i.debug.pushMessage(
            "Sprint: "
                + InputActionRebindingExtensions.GetBindingDisplayString(vInput.inputAction_Sprint),
            "#ffaacc"
        );
        UISingleton.i.debug.pushMessage(
            "Interaction (Normal Mode): "
                + InputActionRebindingExtensions.GetBindingDisplayString(interaction),
            "#ffaacc"
        );
        UISingleton.i.debug.pushMessage(
            "Mode Change: " + InputActionRebindingExtensions.GetBindingDisplayString(modeChange),
            "#ffaacc"
        );
        UISingleton.i.debug.pushMessage(
            "Attack (Fight Mode): "
                + InputActionRebindingExtensions.GetBindingDisplayString(attack),
            "#ffaacc"
        );
        UISingleton.i.debug.pushMessage(
            "Zoom: " + InputActionRebindingExtensions.GetBindingDisplayString(zoom),
            "#ffaacc"
        );
        UISingleton.i.debug.pushMessage(
            "Disable Mouse Input: "
                + InputActionRebindingExtensions.GetBindingDisplayString(disableMouseInput),
            "#ffaacc"
        );
        UISingleton.i.debug.pushMessage(
            "Next Camera: " + InputActionRebindingExtensions.GetBindingDisplayString(nextCamera),
            "#ffaacc"
        );
        UISingleton.i.debug.pushMessage(
            "Prev Camera: " + InputActionRebindingExtensions.GetBindingDisplayString(prevCamera),
            "#ffaacc"
        );
    }

    private bool mouseInputDisabled = false;

    void Update()
    {
        ListenForDisableMouse();
        ListenForToggleDebugPanel();
    }

    private void ListenForDisableMouse()
    {
        if (disableMouseInput.WasPressedThisFrame())
        {
            mouseInputDisabled = !mouseInputDisabled;
        }
        if (mouseInputDisabled)
        {
            InputSystem.DisableDevice(Mouse.current);
        }
        else
        {
            InputSystem.EnableDevice(Mouse.current);
        }
    }

    private void ListenForToggleDebugPanel()
    {
        if (toggleDebugPanel.WasPressedThisFrame() && UISingleton.i.debug.isActive)
        {
            UISingleton.i.debug.gameObject.SetActive(false);
            UISingleton.i.debug.isActive = false;
            EventSystem.current.SetSelectedGameObject(null);
        }
        else if (toggleDebugPanel.WasPressedThisFrame() && !UISingleton.i.debug.isActive)
        {
            UISingleton.i.debug.gameObject.SetActive(true);
            UISingleton.i.debug.isActive = true;
            UISingleton.i.eventSystem.SetSelectedGameObject(
                UISingleton.i.debug.scrollbar.gameObject
            );
        }
    }

    void OnDestroy()
    {
        if (inputSystem != null)
        {
            inputSystem.Dispose();
            inputSystem = null;
        }
    }
}
