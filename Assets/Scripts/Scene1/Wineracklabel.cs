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
    public string wineAlphabet; // ex. "a"
    public string wineColor;    // ex. "딥레드"

    [Header("UI 텍스트")]
    public TextMeshProUGUI infoText;

    [Header("힌트 매니저 연결")]
    public HintManager hintManager;

    bool collected = false;

    public void Collect()
    {
        if (infoText != null)
            infoText.text = $"{wineAlphabet}와인 = {wineColor}";

            if (!collected)
        {
            collected = true;
            if (hintManager != null)
            {
                hintManager.currentPlayerState.foundClues.Add("clue_B");
                if (!hintManager.currentPlayerState.completedSteps.Contains(2))
                    hintManager.currentPlayerState.completedSteps.Add(2);
            }
            Debug.Log($"[WineRackLabel] 단서 B 수집 — {wineAlphabet}와인: {wineColor}");
        }
    }

        public void Hide()
    {
        if (infoText != null)
            infoText.text = "";
    }
}
