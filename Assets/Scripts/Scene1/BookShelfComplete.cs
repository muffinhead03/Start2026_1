using UnityEngine;

public class BookShelfComplete : MonoBehaviour
{
    [Header("연결")]
    public PuzzleSolver puzzleSolver;
    public HintManager hintManager;
    public Animator bookshelfAnimator;
    public GameObject hiddenKey;

    // Object_Puzzle의 UnlockEvent에 연결
    public void OnBookshelfSolved()
    {
        Debug.Log("[BookShelfComplete] 정답! 책장이 밀려나며 열쇠 발견");

        if (bookshelfAnimator != null) bookshelfAnimator.SetTrigger("Slide");
        if (hiddenKey != null) hiddenKey.SetActive(true);

        if (hintManager != null)
        {
            if (!hintManager.currentPlayerState.completedSteps.Contains(5))
                hintManager.currentPlayerState.completedSteps.Add(5);

            if (!hintManager.currentPlayerState.completedSteps.Contains(6))
                hintManager.currentPlayerState.completedSteps.Add(6);

            if (!hintManager.currentPlayerState.foundClues.Contains("clue_bookshelf_order"))
                hintManager.currentPlayerState.foundClues.Add("clue_bookshelf_order");
        }
    }
}