using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public static class InputManager
{
    static InputManager()
    {
        EnablePlayerInput();
    }

    public static void EnablePlayerInput()
    {
        InputActionMap player_input = InputSystem.actions.FindActionMap("PC_Player");
        InputActionMap ui_input = InputSystem.actions.FindActionMap("PC_UI");

        player_input.Enable();
        ui_input.Disable();
    }

    public static void EnableUIInput()
    {
        InputActionMap player_input = InputSystem.actions.FindActionMap("PC_Player");
        InputActionMap ui_input = InputSystem.actions.FindActionMap("PC_UI");

        player_input.Disable();
        ui_input.Enable();
    }
}
