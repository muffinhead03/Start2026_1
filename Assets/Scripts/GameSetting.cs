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
    public static int hintLevel = 1;
    public static float mouseSensitivity = 0.1f;
    public static float soundEffect = 1;
    public static int language = 0;
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

    [Header("LockPointer")]
    public bool lockPointer;

    private InputAction settings_open;
    private InputAction settings_close;

    private bool isPanelOpen;


    private void OnEnable()
    {
        InputManager.EnablePlayerInput();

        UnregisterInputAction();
        RegisterInputAction();
        isPanelOpen = false;
    }

    private void OnDisable()
    {
        UnregisterInputAction();
    }

    private void RegisterInputAction()
    {
        settings_open = InputSystem.actions.FindAction("SettingOpen");
        settings_open.performed += OnSettingsOpenPerformed;
        settings_close = InputSystem.actions.FindAction("SettingClose");
        settings_close.performed += OnSettingsClosePerformed;
    }

    private void UnregisterInputAction()
    {
        if (settings_open != null)  settings_open.performed -= OnSettingsOpenPerformed;
        if (settings_close!=null) settings_close.performed -= OnSettingsClosePerformed;
    }

    private void OnSettingsOpenPerformed(InputAction.CallbackContext ctx)
    {
        OpenSetting();
    }

    private void OnSettingsClosePerformed(InputAction.CallbackContext ctx)
    {
        CloseSetting();
    }

    public void OpenSetting()
    {
        if (isPanelOpen) return;

        isPanelOpen = true;
        SceneUI.SetActivePanel(0, isPanelOpen);

        ShowSettingPanel();
        if(lockPointer) SceneUI.UnlockPointer();
        SceneUI.SetActiveCursor(false);
        player?.SetMoveLock(true);

        InputManager.EnableUIInput();
    }

    public void CloseSetting()
    {
        if (!isPanelOpen) return;

        isPanelOpen = false;
        SceneUI.SetActivePanel(0, isPanelOpen);

        if(lockPointer) SceneUI.LockPointer();
        SceneUI.SetActiveCursor(true);
        player?.SetMoveLock(false);

        InputManager.EnablePlayerInput();
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
