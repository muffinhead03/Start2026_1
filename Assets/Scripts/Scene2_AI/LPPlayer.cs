using UnityEngine;

public class LPPlayer : MonoBehaviour
{
    public HintManager hintManager;
    public GameObject pipe4Object;
    bool solved = false;

    // LP 선택 시 호출 (lpId: "green_7" 등)
    // 힌트 진행 기록은 여기서 하지 않음 — 파이프 4를 집을 때 OrganSceneManager.CompleteStep(6)이 처리함
    // (예전엔 구 20스텝 번호로 completedSteps.Add(7)을 직접 넣어서 "파이프 교체" 스텝이 미리 완료되던 버그가 있었음)
    public void InsertLP(string lpId)
    {
        if (solved) return;
        if (lpId == "green_7")
        {
            solved = true;
            pipe4Object.SetActive(true);
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
        Debug.Log("[LPPlayer] 정답 LP 삽입 — 파이프 4 등장");
    }
}