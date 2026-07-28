using System.Collections.Generic;
using UnityEngine;

public class FloorStainManager : MonoBehaviour
{
    [Header("힌트 매니저 연결")]
    public HintManager hintManager;

    [Header("전체 얼룩 개수 (4색 x 2)")]
    public int totalStains = 8;

    [Header("전체 중점 알파벳 개수")]
    public int totalMidpoints = 4;

    HashSet<GameObject> inspectedStains = new HashSet<GameObject>();
    HashSet<GameObject> inspectedMidpoints = new HashSet<GameObject>();

    // 얼룩(색깔) 조사 — Step 1만 담당
    public void OnStainInspected(GameObject stain)
    {
        bool isFirst = inspectedStains.Count == 0;
        inspectedStains.Add(stain);

        Debug.Log($"[FloorStainManager] {stain.name} 조사됨 ({inspectedStains.Count}/{totalStains})");

        if (hintManager == null) return;

        if (isFirst && !hintManager.currentPlayerState.completedSteps.Contains(1))
        {
            hintManager.currentPlayerState.completedSteps.Add(1);
            Debug.Log("[FloorStainManager] Step 1 완료");
        }
    }

    // 중점 알파벳 조사 — Step 2 + clue 담당
    public void OnMidpointInspected(GameObject marker)
    {
        inspectedMidpoints.Add(marker);

        Debug.Log($"[FloorStainManager] {marker.name} 중점 조사됨 ({inspectedMidpoints.Count}/{totalMidpoints})");

        if (hintManager == null) return;

        if (inspectedMidpoints.Count >= totalMidpoints)
        {
            if (!hintManager.currentPlayerState.completedSteps.Contains(2))
                hintManager.currentPlayerState.completedSteps.Add(2);
            if (!hintManager.currentPlayerState.foundClues.Contains("clue_wine_stains"))
                hintManager.currentPlayerState.foundClues.Add("clue_wine_stains");

            Debug.Log("[FloorStainManager] Step 2 완료 + clue_wine_stains 획득");
        }
    }
}