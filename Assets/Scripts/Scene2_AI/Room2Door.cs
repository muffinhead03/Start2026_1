using UnityEngine;

public class Room2Door : MonoBehaviour
{
    public HintManager hintManager;
    public GameObject door;
    bool opened = false;

    public void TryOpen()
    {
        if (opened) return;
        if (!Player_Inventory.hasKey("key_room2"))
        {
            Debug.Log("[Room2Door] 열쇠 없음");
            return;
        }
        opened = true;
        door.SetActive(false);
        hintManager.currentPlayerState.completedSteps.Add(3);
        Debug.Log("[Room2Door] 방2 개방");
    }
}