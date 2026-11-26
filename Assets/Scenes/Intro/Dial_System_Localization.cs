using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class Dial_System_Localization: Dial_System_Event
{
    [SerializeField] Dial[] dials_EN;

    [SerializeField] Dial[] dials_JP;

    LanguageType current_language;

    private void OnDisable()
    {
        LanguageCurrentStatus.OnLanguageChanged -= Apply;
    }

    public override void InitDial()
    {
        current_dial = 0;

        LanguageCurrentStatus.OnLanguageChanged += Apply;

        current_language = LanguageCurrentStatus.CurrentLanguage;

        isInitialized = true;
    }

    protected override void GetDial()
    {
        if (current_dial >= dials.Length)
        {
            dial = null;
            return;
        }

        if (current_language == LanguageType.English)
        {
            dial = dials_EN[current_dial];
        }
        else if (current_language == LanguageType.Korean)
        {
            dial = dials[current_dial];
        }
        else if (current_language == LanguageType.Japanese)
        {
            dial = dials_JP[current_dial];
        }
    }

    public void Apply(LanguageType language)
    {
        current_language = language;

        GetDial();

        SceneUI.ChangeText(text_talker, dial.scripts[current_script].talker);
        SceneUI.ChangeText(text_talking, dial.scripts[current_script].text);
    }
}
