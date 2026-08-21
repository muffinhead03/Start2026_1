using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public static class Setting
{
    public const int maxLevel = 2;
    public const int maxLanguage = 1;
    public const float minSensitivity = 0.01f;
    public const float maxSensitivity = 0.5f;
    public static int hintLevel;
    public static float mouseSensitivity;
    public static float soundEffect;
    public static int language;
}

public class GameSetting : MonoBehaviour
{
    [Header("Text")]
    string[] hintLevel = { "Easy", "Medium", "Hard" };
    string[] language = { "English", "ÇÑ±¹¾î" };

    [Header("Player")]
    public Player_Move player;

    [Header("UI")]
    public Scene_UI_Manager SceneUI;

    private InputActionMap ui_input;
    private InputAction settingsAction;
    private bool isPanelOpen;


    private void OnEnable()
    {
        SetValuesSetting();
        RegisterInputAction();
        isPanelOpen = false;
    }

    private void OnDisable()
    {
        UnregisterInputAction();
    }

    private void RegisterInputAction()
    {
        ui_input = InputSystem.actions.FindActionMap("PC_UI");
        ui_input.Enable();
        settingsAction = InputSystem.actions.FindAction("Settings");
        settingsAction.performed += OpenCloseSettings;
    }

    private void UnregisterInputAction()
    {
        ui_input.Disable();
        settingsAction.performed -= OpenCloseSettings;
    }

    private void SetValuesSetting()
    {
        Setting.hintLevel = 1;
        Setting.mouseSensitivity = 0.1f;
        Setting.soundEffect = 1f;
        Setting.language = 0;
    }

    private void OpenCloseSettings(InputAction.CallbackContext context)
    {
        isPanelOpen = !isPanelOpen;
        SceneUI.SetActivePanel(0, isPanelOpen);

        if (isPanelOpen)
        {
            ShowSettingPanel();
            SceneUI.UnlockPointer();
            SceneUI.SetActiveCursor(false);
            player?.SetMoveLock(true);
        }
        else
        {
            SceneUI.LockPointer();
            SceneUI.SetActiveCursor(true);
            player?.SetMoveLock(false);
        }
    }

    public void ClickLeftBtn_Hint()
    {
        if (Setting.hintLevel > 0) Setting.hintLevel--;

        ShowSettingPanel() ;
    }

    public void ClickRightBtn_Hint()
    {
        if(Setting.hintLevel < Setting.maxLevel) Setting.hintLevel++;

        ShowSettingPanel() ;
    }

    public void ClickLeftBtn_Language()
    {
        if (Setting.language > 0) Setting.language--;

        ShowSettingPanel() ;
    }

    public void ClickRightBtn_Language()
    {
        if (Setting.language < Setting.maxLanguage) Setting.language++;

        ShowSettingPanel() ;
    }

    public void SetValueSlider_Mouse(float value)
    {
        Setting.mouseSensitivity = Setting.minSensitivity + (Setting.maxSensitivity - Setting.minSensitivity) * value;
    }
    public void SetValueSlider_Sound(float value)
    {
        Setting.soundEffect = value;
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    void ShowSettingPanel()
    {
        SceneUI.ChangeText(1, hintLevel[Setting.hintLevel]);
        SceneUI.ChangeText(2, language[Setting.language]);
        SceneUI.SetSlider(0, (Setting.mouseSensitivity - Setting.minSensitivity) / (Setting.maxSensitivity - Setting.minSensitivity));
        SceneUI.SetSlider(1, Setting.soundEffect);

        SceneUI.SetActiveButton(0, true);
        SceneUI.SetActiveButton(1, true);
        SceneUI.SetActiveButton(2, true);
        SceneUI.SetActiveButton(3, true);

        if (Setting.hintLevel == 0) SceneUI.SetActiveButton(0, false);
        else if (Setting.hintLevel == Setting.maxLevel) SceneUI.SetActiveButton(1, false);

        if (Setting.language == 0) SceneUI.SetActiveButton(2, false);
        else if (Setting.language == Setting.maxLanguage) SceneUI.SetActiveButton(3, false);
    }
}
