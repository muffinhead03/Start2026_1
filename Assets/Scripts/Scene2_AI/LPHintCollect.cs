using UnityEngine;

public class LPHintCollect : MonoBehaviour
{
    public HintManager hintManager;
    bool collected = false;

    public void Read()
    {
        if (collected) return;
        collected = true;
        hintManager.currentPlayerState.foundClues.Add("clue_lp_hint");
        hintManager.currentPlayerState.completedSteps.Add(6);
        Debug.Log("[LPHint] LP 단서 확인 — 초록색 7번");
    }
}