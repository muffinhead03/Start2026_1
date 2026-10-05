using System;
using System.IO;
using UnityEditor;
using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using static UnityEngine.Rendering.DebugUI;

public class GameSetting : MonoBehaviour
{
    const int maxLevel = 2;
    const int maxLanguage = 1;
    const float minSensitivity = 0.01f;
    const float maxSensitivity = 0.5f;

    public static PlayerData data;

    [Header("Text")]
    string[] hintLevel = { "Easy", "Medium", "Hard" };
    string[] language = { "English", "ÇÑ±¹¾î" };

    [Header("Player")]
    public Player_Move player;

    [Header("UI")]
    public Scene_UI_Manager SceneUI;

    [Header("LockPointer")]
    public bool lockPointer;

    [Header("Sound")]
    [SerializeField] private AudioMixer audioMixer;

    private InputAction settings_open;
    private InputAction settings_close;

    private bool isPanelOpen;

    private void Awake()
    {
        if (data == null) data = SaveSystem.LoadGame();

        if (SceneManager.GetActiveScene().name != "StartingScene")
        {
            data.current_stage = SceneManager.GetActiveScene().buildIndex;
        }
    }

    private void Start()
    {
        float dB = Mathf.Log10(data.soundEffect) * 20f;
        audioMixer.SetFloat("Master", dB);
    }

    private void OnEnable()
    {
        InputManager.EnablePlayerInput();

        UnregisterInputAction();
        RegisterInputAction();
        isPanelOpen = false;
    }

    private void OnDisable()
    {
        SaveSystem.SaveGame(data);

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
        SceneUI.UnlockPointer();
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
        if (data.hintLevel > 0) data.hintLevel--;

        ShowSettingPanel() ;
    }

    public void ClickRightBtn_Hint()
    {
        if(data.hintLevel < maxLevel) data.hintLevel++;

        ShowSettingPanel() ;
    }

    public void ClickLeftBtn_Language()
    {
        if (data.language > 0) data.language--;

        ShowSettingPanel() ;
    }

    public void ClickRightBtn_Language()
    {
        if (data.language < maxLanguage) data.language++;

        ShowSettingPanel() ;
    }

    public void SetValueSlider_Mouse(float value)
    {
        data.mouseSensitivity = minSensitivity + (maxSensitivity - minSensitivity) * value;
    }
    public void SetValueSlider_Sound(float value)
    {
        data.soundEffect = value;

        float dB = Mathf.Log10(value) * 20f;
        audioMixer.SetFloat("Master", dB);
    }

    public void ReturnTitle()
    {
        SceneManager.LoadScene("StartingScene");
    }

    public void ExitGame()
    {
        SaveSystem.SaveGame(data);

#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    void ShowSettingPanel()
    {
        SceneUI.ChangeText(1, hintLevel[data.hintLevel]);
        SceneUI.ChangeText(2, language[data.language]);
        SceneUI.SetSlider(0, (data.mouseSensitivity - minSensitivity) / (maxSensitivity - minSensitivity));
        SceneUI.SetSlider(1, data.soundEffect);

        SceneUI.SetActiveButton(0, true);
        SceneUI.SetActiveButton(1, true);
        SceneUI.SetActiveButton(2, true);
        SceneUI.SetActiveButton(3, true);

        if (data.hintLevel == 0) SceneUI.SetActiveButton(0, false);
        else if (data.hintLevel == maxLevel) SceneUI.SetActiveButton(1, false);

        if (data.language == 0) SceneUI.SetActiveButton(2, false);
        else if (data.language == maxLanguage) SceneUI.SetActiveButton(3, false);
    }
}
