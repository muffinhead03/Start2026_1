using System.Collections.Generic;
using UnityEngine;

public static class LanguageData
{
    private static readonly Dictionary<
        string,
        Dictionary<LanguageType, string>
    > Data = new()
    {
        {
            LanguageDataKey.Tutorial_Doll,
            new()
            {
                {
                    LanguageType.Korean,
                    "인형"
                },
                {
                    LanguageType.English,
                    "Doll"
                }
            }
        },

        {
            LanguageDataKey.Tutorial_Coin,
            new()
            {
                {
                    LanguageType.Korean,
                    "동전"
                },
                {
                    LanguageType.English,
                    "Coin"
                }
            }
        },

        {
            LanguageDataKey.Tutorial_Key,
            new()
            {
                {
                    LanguageType.Korean,
                    "열쇠"
                },
                {
                    LanguageType.English,
                    "Key"
                }
            }
        }
    };


    private static readonly Dictionary<
        string,
        string
    > LegacyKey = new()
    {
        {
            "인형",
            LanguageDataKey.Tutorial_Doll
        },
        {
            "동전",
            LanguageDataKey.Tutorial_Coin
        },
        {
            "열쇠",
            LanguageDataKey.Tutorial_Key
        }
    };


    public static string Get(
        string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return value;
        }

        string key = value;

        // 기존 Korean Hard Coding 처리
        if (LegacyKey.TryGetValue(
                value,
                out string legacyKey))
        {
            key = legacyKey;
        }

        // Localization Key 자체가 없는 경우
        if (!Data.TryGetValue(
                key,
                out Dictionary<
                    LanguageType,
                    string
                > languageData))
        {
            return value;
        }

        // 현재 언어 검색
        if (languageData.TryGetValue(
                LanguageCurrentStatus.CurrentLanguage,
                out string localizedText))
        {
            return localizedText;
        }

        // 현재 언어 번역이 없다면 Korean fallback
        if (languageData.TryGetValue(
                LanguageType.Korean,
                out string koreanText))
        {
            return koreanText;
        }

        // 그것조차 없다면 원본 반환
        return value;
    }
}