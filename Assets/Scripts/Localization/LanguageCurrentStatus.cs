using System;

public enum LanguageType
{
    Korean,
    English,
    Japanese // 준비 중: 아직 설정 UI(GameData.LanguageType)에는 연결하지 않음
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