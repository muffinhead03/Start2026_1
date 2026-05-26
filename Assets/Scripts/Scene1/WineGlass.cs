using UnityEngine;
using TMPro;

// 사용법:
// 1. 와인잔 오브젝트에 이 스크립트 추가
// 2. wineColor에 색깔 이름 입력 (ex. "딥레드", "로제핑크", "버건디", "오렌지레드")
// 3. Event_On_ 컴포넌트 OnClick → Inspect() 연결
public class WineGlass : MonoBehaviour
{
    [Header("이 와인잔의 색깔")]
    public string wineColor; // ex. "딥레드"

    [Header("UI 텍스트 (화면 하단)")]
    public TextMeshProUGUI infoText;

    [Header("힌트 매니저 연결")]
    public HintManager hintManager;
    
    void Start()
    {
        if (infoText != null)
            infoText.text = "";
    }

    public void Inspect()
    {
        if (hintManager != null)
        {
        hintManager.AddLastAction("inspect_wine_glass_" + wineColor);
        if (!hintManager.currentPlayerState.foundClues.Contains("clue_A"))
            hintManager.currentPlayerState.foundClues.Add("clue_A");
        if (!hintManager.currentPlayerState.completedSteps.Contains(1))
            hintManager.currentPlayerState.completedSteps.Add(1);
        }
        if (infoText != null)
            infoText.text = $"색깔: {wineColor}";
        Debug.Log($"[WineGlass] 색깔 확인: {wineColor}");
    }
}
