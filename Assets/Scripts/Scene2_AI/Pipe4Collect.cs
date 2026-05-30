using UnityEngine;

public class Pipe4Collect : MonoBehaviour
{
    public HintManager hintManager;
    bool collected = false;

    public void Collect()
    {
        if (collected) return;
        collected = true;
        hintManager.currentPlayerState.foundClues.Add("clue_pipe_4");
        Player_Inventory.AddItem("pipe_4");
        gameObject.SetActive(false);
        Debug.Log("[Pipe4] 파이프 4 수집");
    }
}