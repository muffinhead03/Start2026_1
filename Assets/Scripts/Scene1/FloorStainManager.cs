using System.Collections.Generic;
using UnityEngine;

// 사용법:
// 1. 씬에 빈 오브젝트 만들고 이 스크립트 추가, totalStains에 얼룩 개수 입력 (기본 8)
// 2. 바닥 얼룩 8개 각각의 WineGlass.cs에서 이 매니저를 참조해 OnStainInspected() 호출
// 3. hintManager 연결
public class FloorStainManager : MonoBehaviour
{
    [Header("힌트 매니저 연결")]
    public HintManager hintManager;

    [Header("전체 얼룩 개수 (4색 x 2)")]
    public int totalStains = 8;

    HashSet<GameObject> inspected = new HashSet<GameObject>();

    public void OnStainInspected(GameObject stain)
    {
        bool isFirst = inspected.Count == 0;
        inspected.Add(stain);

        if (hintManager == null) return;

        if (isFirst && !hintManager.currentPlayerState.completedSteps.Contains(1))
            hintManager.currentPlayerState.completedSteps.Add(1);

        if (inspected.Count >= totalStains)
        {
            if (!hintManager.currentPlayerState.completedSteps.Contains(2))
                hintManager.currentPlayerState.completedSteps.Add(2);
            if (!hintManager.currentPlayerState.foundClues.Contains("clue_wine_stains"))
                hintManager.currentPlayerState.foundClues.Add("clue_wine_stains");
        }
    }
}