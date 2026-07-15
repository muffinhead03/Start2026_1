using UnityEngine;

public class Room2KeyCollect : MonoBehaviour
{
    public HintManager hintManager;
    bool collected = false;

    public void Collect()
    {
        if (collected) return;
        collected = true;
        hintManager.currentPlayerState.foundClues.Add("clue_room2_key");
        //gameObject.SetActive(false);
        Debug.Log("[Room2Key] 방2 열쇠 수집");
    }
}