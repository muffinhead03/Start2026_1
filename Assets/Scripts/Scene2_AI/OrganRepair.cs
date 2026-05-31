using UnityEngine;

public class OrganRepair : MonoBehaviour
{
    public HintManager hintManager;
    bool repaired = false;

    public void TryRepair()
    {
        if (repaired) return;

        var clues = hintManager.currentPlayerState.foundClues;
        if (!clues.Contains("clue_pipe_1") ||
            !clues.Contains("clue_pipe_2") ||
            !clues.Contains("clue_pipe_3") ||
            !clues.Contains("clue_pipe_4"))
        {
            Debug.Log("[OrganRepair] 파이프가 부족합니다");
            return;
        }

        repaired = true;
        hintManager.currentPlayerState.completedSteps.Add(8);
        hintManager.currentPlayerState.completedSteps.Add(9);
        Debug.Log("[OrganRepair] 오르간 수리 완료!");
        // TODO: 수리 완료 연출 (조명 꺼짐 등) 여기서 트리거
    }
}