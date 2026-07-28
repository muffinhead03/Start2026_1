using UnityEngine;
using TMPro;

// 사용법:
// 1. 와인랙 라벨 오브젝트에 이 스크립트 추가
// 2. wineAlphabet, wineColor 입력 (ex. "a", "딥레드")
// 3. Event_On_ 컴포넌트 OnClick → Collect() 연결
// 4. hintManager 슬롯에 HintManager 오브젝트 연결
public class WineRackLabel : MonoBehaviour
{
    [Header("와인 정보")]
    public string wineAlphabet;
    public string wineColor;

    [Header("UI 텍스트")]
    public TextMeshProUGUI infoText;

    [Header("연결")]
    public HintManager hintManager;
    public WineLabelManager labelManager;   // ← 추가

    bool collected = false;

    public void Collect()
    {
        if (infoText != null)
            infoText.text = $"{wineAlphabet}와인 = {wineColor}";

        if (!collected)
        {
            collected = true;
            hintManager?.AddLastAction("inspect_wine_label_" + wineAlphabet);
            labelManager?.OnLabelInspected(gameObject);   // ← clue_B 직접 추가하던 부분 교체
        }
    }

    public void Hide()
    {
        if (infoText != null) infoText.text = "";
    }
}
