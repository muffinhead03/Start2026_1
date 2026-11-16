using UnityEditor;
using UnityEngine;

/// <summary>
/// 메뉴: HintSystem/Validate Translations — 힌트 번역 데이터(HintLanguageData) 검사 결과를 콘솔에 출력.
/// 검사 내용은 HintLocalizationChecker 참고. 번역을 추가·수정한 뒤(특히 LLM으로 초안을 만든 뒤) 한 번씩 돌리기.
/// </summary>
public static class HintLocalizationValidatorMenu
{
    [MenuItem("HintSystem/Validate Translations")]
    static void Validate()
    {
        var report = HintLocalizationChecker.Run();

        foreach (string error in report.Errors)
            Debug.LogError($"[HintLocalization] {error}");
        foreach (string warning in report.Warnings)
            Debug.LogWarning($"[HintLocalization] {warning}");

        string summary = $"[HintLocalization] 키 {report.KeyCount}개 검사 — 오류 {report.Errors.Count}개, 경고 {report.Warnings.Count}개";
        if (report.Errors.Count > 0) Debug.LogError(summary);
        else Debug.Log(summary);
    }
}