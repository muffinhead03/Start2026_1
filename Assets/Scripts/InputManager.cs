using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager instance {  get; private set; }
    InputActionMap player_input;

    InputActionMap ui_input;

    void Awake()
    {
        if(instance == null) { instance = this; }

        player_input = InputSystem.actions.FindActionMap("PC_Player");

        ui_input = InputSystem.actions.FindActionMap("PC_UI");
    }

    private void OnEnable()
    {
        EnablePlayerInput();
    }

    private void OnDisable()
    {
        player_input.Disable();
        ui_input.Disable();
    }

    public void EnablePlayerInput()
    {
        player_input.Enable();
        ui_input.Disable();
    }

    public void EnableUIInput()
    {
        player_input.Disable();
        ui_input.Enable();
    }
}
