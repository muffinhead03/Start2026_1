using System.Collections.Generic;

/// <summary>
/// 힌트 패널 고정 UI 문구를 HintLanguageData(힌트 전용 번역 데이터)에서 가져오는 구현체.
/// 문장 자체는 HintLanguageData.UI.cs에 한·영·일로 있음 — 여기는 "어떤 문구 → 어떤 키"만 연결.
/// HintLanguageData가 매번 현재 언어(LanguageCurrentStatus)를 읽으므로 게임 중에 언어를 바꿔도 다음 표시부터 바로 반영됨.
/// </summary>
public class LocalizedHintUIText : IHintUITextProvider
{
    static readonly Dictionary<HintUITextId, string> Keys = new Dictionary<HintUITextId, string>
    {
        { HintUITextId.RadioNoise,    HintLanguageDataKey.HintUI_RadioNoise },
        { HintUITextId.Greeting,      HintLanguageDataKey.HintUI_Greeting },
        { HintUITextId.Connecting,    HintLanguageDataKey.HintUI_Connecting },
        { HintUITextId.NoHintsLeft,   HintLanguageDataKey.HintUI_NoHintsLeft },
        { HintUITextId.AllCluesFound, HintLanguageDataKey.HintUI_AllCluesFound },
        { HintUITextId.NoResponse,    HintLanguageDataKey.HintUI_NoResponse },
        { HintUITextId.FallbackTag,   HintLanguageDataKey.HintUI_FallbackTag },
    };

    public string Get(HintUITextId id)
    {
        return Keys.TryGetValue(id, out string key)
            ? HintLanguageData.Get(key)
            : id.ToString(); // 키 연결을 빠뜨렸으면 이름이 그대로 보여서 바로 눈에 띔
    }
}