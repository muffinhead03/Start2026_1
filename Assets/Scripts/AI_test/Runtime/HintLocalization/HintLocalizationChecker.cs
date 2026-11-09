using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

/// <summary>
/// HintLanguageData 번역 검사 — 빠진 번역 / 자리표시자 불일치 / 후보 개수 차이 / 행동 라벨 안 쉼표.
/// LLM이나 사람이 번역을 넣은 뒤 이 검사를 통과해야 게임에서 문장이 깨지지 않음
/// (예: 한국어 "{0}에 있구나"를 영어로 옮기면서 {0}을 빼먹으면 구역 이름이 사라짐).
///
/// 검사 로직만 담당 (Unity 에디터 의존 없음) — 메뉴 실행은 Editor/HintLocalizationValidatorMenu.cs.
/// </summary>
public static class HintLocalizationChecker
{
    // 검사할 언어 (기준 언어인 한국어 제외)
    static readonly LanguageType[] TargetLanguages = { LanguageType.English, LanguageType.Japanese };

    static readonly Regex Placeholder = new Regex(@"\{\d+\}");

    public sealed class Report
    {
        public readonly List<string> Errors   = new List<string>(); // 게임에서 문장이 깨지는 문제
        public readonly List<string> Warnings = new List<string>(); // 동작은 하지만 확인이 필요한 것
        public int KeyCount;
    }

    public static Report Run()
    {
        var report = new Report();

        foreach (string key in HintLanguageData.AllKeys)
        {
            report.KeyCount++;

            string[] korean = HintLanguageData.GetRaw(key, LanguageType.Korean);
            if (IsEmpty(korean))
            {
                report.Errors.Add($"{key}: 한국어(기준 문장)가 없음");
                continue;
            }

            string koreanPlaceholders = PlaceholderSignature(korean[0]);

            foreach (var language in TargetLanguages)
            {
                string[] translated = HintLanguageData.GetRaw(key, language);

                if (IsEmpty(translated))
                {
                    report.Errors.Add($"{key}: {language} 번역 없음 (게임에선 한국어로 대체됨)");
                    continue;
                }

                if (translated.Length != korean.Length)
                    report.Warnings.Add($"{key}: {language} 후보 {translated.Length}개 / 한국어 {korean.Length}개");

                foreach (string candidate in translated)
                {
                    if (candidate == null)
                    {
                        report.Errors.Add($"{key}: {language} 후보 중 null이 있음");
                        continue;
                    }

                    if (PlaceholderSignature(candidate) != koreanPlaceholders)
                        report.Errors.Add($"{key}: {language} 자리표시자 불일치 — \"{candidate}\" (한국어: {koreanPlaceholders})");
                }
            }

            // 행동 라벨은 관찰 문장 틀("{행동}, 지금은 {위치}...") 안에 들어가므로 쉼표가 있으면 문장이 어색해짐
            if (key.StartsWith("HintAction_", StringComparison.Ordinal))
            {
                foreach (var language in new[] { LanguageType.Korean, LanguageType.English, LanguageType.Japanese })
                {
                    string[] labels = HintLanguageData.GetRaw(key, language);
                    if (labels == null) continue;
                    foreach (string label in labels)
                        if (label != null && (label.Contains(",") || label.Contains("、")))
                            report.Warnings.Add($"{key}: {language} 행동 라벨에 쉼표 — \"{label}\"");
                }
            }
        }

        return report;
    }

    static bool IsEmpty(string[] values) => values == null || values.Length == 0;

    // "{1} ... {0}" → "{0}{1}" (순서 무관, 종류만 비교 — 언어마다 어순이 달라서)
    static string PlaceholderSignature(string text)
    {
        var found = new SortedSet<string>(StringComparer.Ordinal);
        foreach (Match m in Placeholder.Matches(text))
            found.Add(m.Value);
        return string.Concat(found);
    }
}