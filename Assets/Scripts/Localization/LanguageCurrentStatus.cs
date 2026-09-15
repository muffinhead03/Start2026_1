using System;

public enum LanguageType
{
    Korean,
    English
}

public static class LanguageCurrentStatus
{
    public static LanguageType CurrentLanguage
    {
        get;
        private set;
    } = LanguageType.Korean;

    public static event Action<LanguageType> OnLanguageChanged;

    public static void SetLanguage(
        LanguageType language)
    {
        if (CurrentLanguage == language)
        {
            return;
        }

        CurrentLanguage = language;

        OnLanguageChanged?.Invoke(
            CurrentLanguage
        );
    }
}