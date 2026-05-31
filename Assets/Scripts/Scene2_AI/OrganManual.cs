using UnityEngine;

public class OrganManual : MonoBehaviour
{
    public HintManager hintManager;
    bool read = false;

    public void Read()
    {
        if (read) return;
        read = true;
        hintManager.currentPlayerState.foundClues.Add("clue_organ_manual");
        hintManager.currentPlayerState.completedSteps.Add(1);
        Debug.Log("[OrganManual] 오르간 설명서 읽음");
    }
}