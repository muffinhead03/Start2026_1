using UnityEngine;
using TMPro;

// 사용법:
// 1. 교차점 오브젝트에 이 스크립트 추가
// 2. stainColor, intersectionNumber 입력 (ex. "딥레드", 18)
// 3. Event_On_ 컴포넌트 OnClick → Inspect() 연결
// 4. hintManager 슬롯에 HintManager 오브젝트 연결
public class StainIntersection : MonoBehaviour
{
    [Header("교차점 정보")]
    public string stainColor;       // ex. "딥레드"
    public int    intersectionNumber; // ex. 18

    [Header("UI 텍스트")]
    public TextMeshProUGUI infoText;

    [Header("힌트 매니저 연결")]
    public HintManager hintManager;

    bool collected = false;

    public void Inspect()
    {
        if (infoText != null)
            infoText.text = $"{stainColor} 교차점: {intersectionNumber}";
        if (!collected)
        {
            collected = true;
            if (hintManager != null)
            {
                hintManager.currentPlayerState.foundClues.Add("clue_C");
                if (!hintManager.currentPlayerState.completedSteps.Contains(3))
                    hintManager.currentPlayerState.completedSteps.Add(3);
            }
            Debug.Log($"[StainIntersection] 단서 C 수집 — {stainColor}: {intersectionNumber}");
        }
    }

        public void Hide()
    {
        if (infoText != null)
            infoText.text = "";
    }
}
