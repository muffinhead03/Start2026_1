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

        // 스텝 번호: 벽 종이 붙이기(3) 추가로 와인 라벨은 4, 알파벳 밀기는 5
        if (isFirst && !hintManager.currentPlayerState.completedSteps.Contains(4))
        {
            hintManager.currentPlayerState.completedSteps.Add(4);
            Debug.Log("[WineLabelManager] Step 4 완료");
        }

        if (inspected.Count >= totalLabels)
        {
            if (!hintManager.currentPlayerState.completedSteps.Contains(5))
                hintManager.currentPlayerState.completedSteps.Add(5);
            if (!hintManager.currentPlayerState.foundClues.Contains("clue_wine_labels"))
                hintManager.currentPlayerState.foundClues.Add("clue_wine_labels");

            Debug.Log("[WineLabelManager] Step 5 완료 + clue_wine_labels 획득");

            onAllLabelsInspected?.Invoke();   // (예전엔 책장 슬라이드 트리거 — 벽 종이가 처음부터 보이게 바뀌어서 씬에서는 Off)
        }
    }
}