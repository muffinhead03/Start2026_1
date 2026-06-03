using UnityEngine;

public class Pipe3Collect : MonoBehaviour
{
    public HintManager hintManager;
    bool collected = false;

    public void Collect()
    {
        if (collected) return;
        collected = true;
        hintManager.currentPlayerState.foundClues.Add("clue_pipe_3");
        gameObject.SetActive(false);
        Debug.Log("[Pipe3] 파이프 3 수집");
    }
}