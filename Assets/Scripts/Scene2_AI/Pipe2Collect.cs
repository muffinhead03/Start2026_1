using UnityEngine;

public class Pipe2Collect : MonoBehaviour
{
    public HintManager hintManager;
    bool collected = false;

    public void Collect()
    {
        if (collected) return;
        collected = true;
        hintManager.currentPlayerState.foundClues.Add("clue_pipe_2");
        Player_Inventory.AddItem("pipe_2");
        gameObject.SetActive(false);
        Debug.Log("[Pipe2] 파이프 2 수집");

        // 파이프 1, 2 둘 다 수집했으면 step 2 완료
        if (hintManager.currentPlayerState.foundClues.Contains("clue_pipe_1"))
            hintManager.currentPlayerState.completedSteps.Add(2);
    }
}