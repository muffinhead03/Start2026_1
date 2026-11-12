using System.Collections.Generic;

/// <summary>
/// 힌트 패널 고정 문구 (LocalizedHintUIText가 사용).
/// </summary>
public static partial class HintLanguageData
{
    private static readonly Dictionary<string, Dictionary<LanguageType, string[]>> UIData = new()
    {
        {
            HintLanguageDataKey.HintUI_RadioNoise,
            new()
            {
                { LanguageType.Korean,   new[] { "치지직..." } },
                { LanguageType.English,  new[] { "Kzzt..." } },
                { LanguageType.Japanese, new[] { "ザザッ…" } }
            }
        },

        {
            HintLanguageDataKey.HintUI_Greeting,
            new()
            {
                { LanguageType.Korean,   new[] { "도움이 필요해?" } },
                { LanguageType.English,  new[] { "Need some help?" } },
                { LanguageType.Japanese, new[] { "助けが必要？" } }
            }
        },

        {
            HintLanguageDataKey.HintUI_Connecting,
            new()
            {
                { LanguageType.Korean,   new[] { "치지직.. 교신 중" } },
                { LanguageType.English,  new[] { "Kzzt.. connecting" } },
                { LanguageType.Japanese, new[] { "ザザッ…交信中" } }
            }
        },

        {
            HintLanguageDataKey.HintUI_NoHintsLeft,
            new()
            {
                { LanguageType.Korean,   new[] { "더 이상 도움을 받을 수 없어." } },
                { LanguageType.English,  new[] { "I can't help you anymore." } },
                { LanguageType.Japanese, new[] { "これ以上は手伝えない。" } }
            }
        },

        {
            HintLanguageDataKey.HintUI_AllCluesFound,
            new()
            {
                { LanguageType.Korean,   new[] { "이미 모든 단서를 찾았어." } },
                { LanguageType.English,  new[] { "You've already found every clue." } },
                { LanguageType.Japanese, new[] { "もう手がかりは全部見つけたよ。" } }
            }
        },

        {
            HintLanguageDataKey.HintUI_NoResponse,
            new()
            {
                { LanguageType.Korean,   new[] { "지금은 응답할 수 없어." } },
                { LanguageType.English,  new[] { "I can't answer right now." } },
                { LanguageType.Japanese, new[] { "今は応答できない。" } }
            }
        },

        {
            HintLanguageDataKey.HintUI_FallbackTag,
            new()
            {
                { LanguageType.Korean,   new[] { "[폴백]" } },
                { LanguageType.English,  new[] { "[Fallback]" } },
                { LanguageType.Japanese, new[] { "[フォールバック]" } }
            }
        },

        /*
         * 패널에 붙어 있는 고정 라벨 (HintUILocalizedLabel) — 무전기 친구 말투가 아니라 일반 UI 문구
         */

        {
            HintLanguageDataKey.HintUI_HintButton,
            new()
            {
                { LanguageType.Korean,   new[] { "퍼즐 힌트" } },
                { LanguageType.English,  new[] { "Puzzle Hint" } },
                { LanguageType.Japanese, new[] { "パズルのヒント" } }
            }
        },

        {
            HintLanguageDataKey.HintUI_EscClose,
            new()
            {
                { LanguageType.Korean,   new[] { "ESC: 닫기" } },
                { LanguageType.English,  new[] { "ESC: Close" } },
                { LanguageType.Japanese, new[] { "ESC: 閉じる" } }
            }
        },

        {
            HintLanguageDataKey.HintUI_FallbackBadgeTitle,
            new()
            {
                { LanguageType.Korean,   new[] { "Fall Back" } },
                { LanguageType.English,  new[] { "Fall Back" } },
                { LanguageType.Japanese, new[] { "フォールバック" } }
            }
        },

        {
            HintLanguageDataKey.HintUI_FallbackBadgeSub,
            new()
            {
                { LanguageType.Korean,   new[] { "(에러 아닙니다)" } },
                { LanguageType.English,  new[] { "(Not an error)" } },
                { LanguageType.Japanese, new[] { "(エラーではありません)" } }
            }
        }
    };
}