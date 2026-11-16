using TMPro;
using UnityEngine;

/// <summary>
/// 힌트 패널에 고정으로 박혀 있는 TMP 라벨(퍼즐 힌트 버튼 / ESC: 닫기 / 폴백 배지 등)을 현재 언어로 바꿔 주는 컴포넌트.
///
/// - 글자를 바꿀 TMP 텍스트 오브젝트(또는 그 부모, 예: 버튼)에 붙이고 Text Id만 고르면 됨
///   (같은 오브젝트 → 없으면 자식에서 TMP_Text를 자동으로 잡음)
/// - 문장은 HintLanguageData.UI.cs에 한·영·일로 있음 → 문구 수정은 거기서
/// - LanguageCurrentStatus.OnLanguageChanged를 구독하므로 게임 중 설정창에서 언어를 바꿔도 즉시 반영
/// - 일본어 폰트 교체는 캔버스 루트의 LanguageFontSwitcher가 따로 처리함 (이 컴포넌트는 글자만 바꿈)
/// </summary>
[DisallowMultipleComponent]
public class HintUILocalizedLabel : MonoBehaviour
{
    [SerializeField] HintUITextId textId = HintUITextId.HintButton;

    static readonly IHintUITextProvider Provider = new LocalizedHintUIText();

    TMP_Text label;

    void Awake()
    {
        FindLabel();
    }

    void FindLabel()
    {
        if (label != null) return;
        label = GetComponent<TMP_Text>();
        if (label == null) label = GetComponentInChildren<TMP_Text>(true);
    }

    void OnEnable()
    {
        LanguageCurrentStatus.OnLanguageChanged += OnLanguageChanged;
        Refresh(); // 꺼져 있는 동안 언어가 바뀌었을 수도 있어서 켜질 때 한 번 맞춤
    }

    void OnDisable()
    {
        LanguageCurrentStatus.OnLanguageChanged -= OnLanguageChanged;
    }

    void OnLanguageChanged(LanguageType _) => Refresh();

    public void Refresh()
    {
        FindLabel();
        if (label == null)
        {
            Debug.LogWarning($"[HintUILocalizedLabel] {name}: 이 오브젝트나 자식에 TMP 텍스트가 없음", this);
            return;
        }
        label.text = Provider.Get(textId);
    }
}