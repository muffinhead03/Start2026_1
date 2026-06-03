using UnityEngine;

public class LPPlayer : MonoBehaviour
{
    public HintManager hintManager;
    public GameObject pipe4Object;
    bool solved = false;

    // LP 선택 시 호출 (lpId: "green_7" 등)
    public void InsertLP(string lpId)
    {
        if (solved) return;
        if (lpId == "green_7")
        {
            solved = true;
            pipe4Object.SetActive(true);
            hintManager.currentPlayerState.completedSteps.Add(7);
            Debug.Log("[LPPlayer] 정답 LP 삽입 — 파이프 4 등장");
        }
        else
        {
            Debug.Log("[LPPlayer] 오답 LP: " + lpId);
        }
    }

    public void SolveLP()
    {
        if (solved) return;
        solved = true;
        hintManager.currentPlayerState.completedSteps.Add(7);
        Debug.Log("[LPPlayer] 정답 LP 삽입 — 파이프 4 등장");
    }
}