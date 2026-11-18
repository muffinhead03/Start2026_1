using TMPro;
using UnityEngine;

/*
 * LocalizedText
 *
 * TextMeshPro 텍스트에 붙여서 현재 언어(LanguageCurrentStatus.CurrentLanguage)에 맞게
 * 글자와 폰트를 바꿔 주는 컴포넌트.
 *
 * - English  : 원래 폰트 / 머티리얼 그대로
 * - Korean   : koreanFont   (인벤토리와 같은 goorm-sans-regular SDF)
 * - Japanese : japaneseFont (인벤토리와 같은 goorm-sans-regular SDF + NotoSansJP Fallback)
 *
 * key가 있으면 LanguageData.Get(key)로 글자도 바꾼다.
 * key가 비어 있으면 폰트만 바꾼다. (Easy/English처럼 코드가 글자를 넣는 텍스트용)
 *
 * 언어가 바뀌면 OnLanguageChanged 이벤트를 받아 바로 다시 적용한다.
 */
[DisallowMultipleComponent]
[RequireComponent(typeof(TMP_Text))]
public class LocalizedText : MonoBehaviour
{
    [Tooltip("LanguageDataKey 이름. 비워두면 글자는 그대로 두고 폰트만 바꿈")]
    [SerializeField] private string key;

    [Tooltip("한국어일 때 폰트 (비워두면 원래 폰트)")]
    [SerializeField] private TMP_FontAsset koreanFont;

    [Tooltip("일본어일 때 폰트 (비워두면 원래 폰트)")]
    [SerializeField] private TMP_FontAsset japaneseFont;

    private TMP_Text text;
    private TMP_FontAsset originalFont;
    private Material originalMaterial;
    private bool initialized;

    private void Awake()
    {
        Initialize();
    }

    private void OnEnable()
    {
        LanguageCurrentStatus.OnLanguageChanged += Apply;

        // 꺼져 있는 동안 언어가 바뀌었을 수 있으므로 켜질 때마다 적용
        Apply(LanguageCurrentStatus.CurrentLanguage);
    }

    private void OnDisable()
    {
        LanguageCurrentStatus.OnLanguageChanged -= Apply;
    }

    private void Initialize()
    {
        if (initialized) return;

        text = GetComponent<TMP_Text>();
        originalFont = text.font;
        originalMaterial = text.fontSharedMaterial;
        initialized = true;
    }

    public void Apply(LanguageType language)
    {
        Initialize();

        // 1. 폰트
        TMP_FontAsset targetFont = language switch
        {
            LanguageType.Korean => koreanFont,
            LanguageType.Japanese => japaneseFont,
            _ => null
        };

        if (targetFont != null)
        {
            if (text.font != targetFont)
            {
                text.font = targetFont;
                text.fontSharedMaterial = targetFont.material;
            }
        }
        else if (text.font != originalFont)
        {
            text.font = originalFont;
            text.fontSharedMaterial = originalMaterial;
        }

        // 2. 글자
        if (!string.IsNullOrEmpty(key))
        {
            text.text = LanguageData.Get(key);
        }
    }
}
