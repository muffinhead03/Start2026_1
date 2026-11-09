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
        InputActionMap dial_input = InputSystem.actions.FindActionMap("PC_Dial");

        player_input.Enable();
        ui_input.Disable();
        dial_input.Disable();

        Debug.Log("PlayerInput");
    }

    public static void DisableInput()
    {
        InputActionMap player_input = InputSystem.actions.FindActionMap("PC_Player");
        InputActionMap ui_input = InputSystem.actions.FindActionMap("PC_UI");
        InputActionMap dial_input = InputSystem.actions.FindActionMap("PC_Dial");

        player_input.Disable();
        ui_input.Disable();
        dial_input.Disable();

        Debug.Log("DisableInput");
    }

    public static void EnableUIInput()
    {
        InputActionMap player_input = InputSystem.actions.FindActionMap("PC_Player");
        InputActionMap ui_input = InputSystem.actions.FindActionMap("PC_UI");
        InputActionMap dial_input = InputSystem.actions.FindActionMap("PC_Dial");

        player_input.Disable();
        ui_input.Enable();
        dial_input.Disable();

        Debug.Log("UIInput");
    }

    public static void EnableDialInput()
    {
        InputActionMap player_input = InputSystem.actions.FindActionMap("PC_Player");
        InputActionMap ui_input = InputSystem.actions.FindActionMap("PC_UI");
        InputActionMap dial_input = InputSystem.actions.FindActionMap("PC_Dial");

        player_input.Disable();
        ui_input.Disable();
        dial_input.Enable();

        Debug.Log("Dial Input");
    }
}
