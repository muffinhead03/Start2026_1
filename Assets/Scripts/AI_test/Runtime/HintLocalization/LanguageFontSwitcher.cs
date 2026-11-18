using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// 현재 언어가 일본어일 때만 TMP 텍스트 폰트를 일본어 폰트로 바꾸고,
/// 다른 언어로 돌아오면 원래 폰트·머티리얼로 되돌리는 컴포넌트.
///
/// - 캔버스 루트(예: HintUiCanvas)에 붙이고 Japanese Font만 넣으면 됨
/// - Targets를 비워두면 자식의 모든 TMP 텍스트(비활성 포함)를 자동으로 대상으로 잡음
/// - LanguageCurrentStatus.OnLanguageChanged를 구독하므로 게임 중 언어 변경도 즉시 반영
///
/// Fallback 방식(JapaneseFontFallbackSetup)과 달리 폰트 자체를 교체하므로
/// 일본어 문장 안에서 원래 폰트(픽셀 폰트 등)와 일본어 폰트가 섞여 보이지 않음.
/// </summary>
[DisallowMultipleComponent]
public class LanguageFontSwitcher : MonoBehaviour
{
    [SerializeField] TMP_FontAsset japaneseFont;

    [Tooltip("비워두면 자식의 모든 TMP 텍스트(비활성 포함)를 대상으로 함")]
    [SerializeField] List<TMP_Text> targets = new List<TMP_Text>();

    // 원래 폰트·머티리얼 (아웃라인 등 커스텀 머티리얼 복원용)
    readonly Dictionary<TMP_Text, (TMP_FontAsset font, Material material)> originals =
        new Dictionary<TMP_Text, (TMP_FontAsset font, Material material)>();

    void Awake()
    {
        if (targets.Count == 0)
            GetComponentsInChildren(true, targets);

        foreach (TMP_Text text in targets)
        {
            if (text != null && !originals.ContainsKey(text))
                originals[text] = (text.font, text.fontSharedMaterial);
        }

        if (japaneseFont == null)
            Debug.LogWarning($"[LanguageFontSwitcher] {name}: Japanese Font가 비어 있음 — 일본어에서도 원래 폰트 사용", this);
    }

    void OnEnable()
    {
        LanguageCurrentStatus.OnLanguageChanged += Apply;
        Apply(LanguageCurrentStatus.CurrentLanguage); // 꺼져 있는 동안 언어가 바뀌었을 수도 있어서 켜질 때 한 번 맞춤
    }

    void OnDisable()
    {
        LanguageCurrentStatus.OnLanguageChanged -= Apply;
    }

    void Apply(LanguageType language)
    {
        bool useJapanese = language == LanguageType.Japanese && japaneseFont != null;

        foreach (var pair in originals)
        {
            TMP_Text text = pair.Key;
            if (text == null) continue;

            if (useJapanese)
            {
                if (text.font == japaneseFont) continue;
                text.font = japaneseFont;
                text.fontSharedMaterial = japaneseFont.material;
            }
            else
            {
                if (text.font == pair.Value.font) continue;
                text.font = pair.Value.font;
                text.fontSharedMaterial = pair.Value.material;
            }
        }
    }
}