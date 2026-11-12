using System.Collections.Generic;

/// <summary>
/// 힌트 시스템 번역 데이터 (팀 LanguageData와 같은 방식 — 키 → LanguageType별 문장, 현재 언어는 LanguageCurrentStatus).
/// 팀 형식과 다른 점은 값이 문장 하나가 아니라 "후보 목록(string[])"이라는 것 하나뿐 —
/// 같은 상황에서 다시 물어도 매번 같은 말을 하지 않게 하려고 후보 중 하나를 랜덤으로 고름.
///
/// 데이터는 영역별 partial 파일에 나눠 둠 (HintLanguageData.UI.cs / .Observation.cs / .Actions.cs / .WineScene.cs ...).
/// 씬 담당자가 각자 자기 파일만 고치면 돼서 같은 파일 끝에 동시에 추가하다 머지 충돌 나는 일이 없음.
/// 새 영역 파일을 추가하면 아래 Parts 배열에 한 줄만 추가.
///
/// 조회 규칙 (팀 LanguageData.Get과 동일): 현재 언어 → 없으면 한국어 → 그것도 없으면
///  - Get: 키 이름 그대로 (화면에 키가 보여서 빠진 번역이 바로 눈에 띔)
///  - GetCandidates: null (호출하는 쪽이 기본값을 쓰거나 원래 문자열을 그대로 씀)
/// </summary>
public static partial class HintLanguageData
{
    // 모든 영역 데이터를 하나로 합친 것. static 필드 초기화(각 partial 파일)가 끝난 뒤 정적 생성자에서 합침.
    private static readonly Dictionary<string, Dictionary<LanguageType, string[]>> Data =
        new Dictionary<string, Dictionary<LanguageType, string[]>>();

    static HintLanguageData()
    {
        var parts = new[]
        {
            UIData,
            ObservationData,
            ActionData,
            WineSceneData,
        };

        foreach (var part in parts)
        {
            foreach (var pair in part)
            {
#if UNITY_EDITOR
                if (Data.ContainsKey(pair.Key))
                    UnityEngine.Debug.LogWarning($"[HintLocalization] 중복 키: {pair.Key} (나중 값으로 덮어씀)");
#endif
                Data[pair.Key] = pair.Value;
            }
        }
    }

    /// <summary>현재 언어의 첫 번째 문장. 없으면 한국어, 그것도 없으면 키 그대로.</summary>
    public static string Get(string key)
    {
        string[] candidates = GetCandidates(key);
        if (candidates != null && candidates.Length > 0)
            return candidates[0];

#if UNITY_EDITOR
        UnityEngine.Debug.LogWarning($"[HintLocalization] 등록되지 않은 키: {key}");
#endif
        return key;
    }

    /// <summary>현재 언어의 문장 후보 전체. 없으면 한국어, 키 자체가 없으면 null.</summary>
    public static string[] GetCandidates(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            return null;

        if (!Data.TryGetValue(key, out Dictionary<LanguageType, string[]> languageData))
            return null;

        if (languageData.TryGetValue(LanguageCurrentStatus.CurrentLanguage, out string[] localized) &&
            localized != null && localized.Length > 0)
            return localized;

        if (languageData.TryGetValue(LanguageType.Korean, out string[] korean) &&
            korean != null && korean.Length > 0)
            return korean;

        return null;
    }

    /// <summary>등록된 전체 키 (번역 검증 도구용)</summary>
    public static IEnumerable<string> AllKeys => Data.Keys;

    /// <summary>특정 언어로 등록된 후보 (번역 검증 도구용 — 한국어로 대체하지 않음). 없으면 null.</summary>
    public static string[] GetRaw(string key, LanguageType language)
    {
        return Data.TryGetValue(key, out var languageData) && languageData.TryGetValue(language, out var value)
            ? value
            : null;
    }
}