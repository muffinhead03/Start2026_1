using UnityEngine;
using TMPro;

// 사용법:
// 1. 와인랙 라벨 오브젝트에 이 스크립트 추가
// 2. vintageNumber, wineColor 입력 (ex. 18, "딥레드")
// 3. Event_On_ 컴포넌트 OnClick → Collect() 연결
// 4. hintManager, labelManager 슬롯 연결
public class WineRackLabel : MonoBehaviour
{
    [Header("와인 정보")]
    public int vintageNumber;   // 빈티지 뒤 두 자리 숫자 (시프트 값)
    public string wineColor;

    [Header("UI 텍스트")]
    public TextMeshProUGUI infoText;

    [Header("연결")]
    public HintManager hintManager;
    public WineLabelManager labelManager;

    bool collected = false;

    public void Collect()
    {
        if (infoText != null)
            infoText.text = $"{wineColor} 와인: {vintageNumber:D2}";

        if (!collected)
        {
            collected = true;
            hintManager?.AddLastAction("inspect_wine_label_" + wineColor);
            labelManager?.OnLabelInspected(gameObject);
        }
    }

    public void Hide()
    {
        if (infoText != null) infoText.text = "";
    }
}