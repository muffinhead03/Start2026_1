using UnityEngine;

public class Room2Door : MonoBehaviour
{
    public HintManager hintManager;
    public GameObject door;
    bool opened = false;

    public void TryOpen()
    {
        if (opened) return;
        opened = true;
        hintManager.currentPlayerState.completedSteps.Add(3);
        Debug.Log("[Room2Door] 방2 개방");
    }
}