using Invector.vCharacterController;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public InputSystem_Actions inputSystem;
    public InputAction interaction;
    public InputAction attack;
    public InputAction zoom;
    public InputAction modeChange;
    public InputAction disableMouseInput;
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
    }

    private bool mouseInputDisabled = false;

    void Update()
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

    void OnDisable()
    {
        inputSystem.Disable();
    }
}
