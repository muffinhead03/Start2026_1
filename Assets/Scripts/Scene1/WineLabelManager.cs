using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WineLabelManager : MonoBehaviour
{
    [Header("힌트 매니저 연결")]
    public HintManager hintManager;

    [Header("전체 라벨 개수")]
    public int totalLabels = 4;

    [Header("완료 이벤트")]
    public UnityEvent onAllLabelsInspected;

    HashSet<GameObject> inspected = new HashSet<GameObject>();

    public void OnLabelInspected(GameObject label)
    {
        bool isFirst = inspected.Count == 0;
        inspected.Add(label);

        Debug.Log($"[WineLabelManager] {label.name} 조사됨 ({inspected.Count}/{totalLabels})");

        if (hintManager == null) return;

        if (isFirst && !hintManager.currentPlayerState.completedSteps.Contains(3))
        {
            hintManager.currentPlayerState.completedSteps.Add(3);
            Debug.Log("[WineLabelManager] Step 3 완료");
        }

        if (inspected.Count >= totalLabels)
        {
            if (!hintManager.currentPlayerState.completedSteps.Contains(4))
                hintManager.currentPlayerState.completedSteps.Add(4);
            if (!hintManager.currentPlayerState.foundClues.Contains("clue_wine_labels"))
                hintManager.currentPlayerState.foundClues.Add("clue_wine_labels");

            Debug.Log("[WineLabelManager] Step 4 완료 + clue_wine_labels 획득");

            onAllLabelsInspected?.Invoke();   // 추가 — 여기서 책장 슬라이드 트리거
        }
    }
}