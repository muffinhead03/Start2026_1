using UnityEngine;

public class Pipe1Collect : MonoBehaviour
{
    public HintManager hintManager;
    bool collected = false;

    public void Collect()
    {
        if (collected) return;
        collected = true;
        hintManager.currentPlayerState.foundClues.Add("clue_pipe_1");
        //gameObject.SetActive(false);
        Debug.Log("[Pipe1] 파이프 1 수집");
    }
}