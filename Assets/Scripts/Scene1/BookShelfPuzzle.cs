using System.Collections.Generic;
using UnityEngine;

public class BookShelfPuzzle : MonoBehaviour
{
    [Header("정답 계산기 연결 (PuzzleSolver)")]
    public PuzzleSolver puzzleSolver;

    [Header("책 배치 슬롯 (0~3, 와인병 순서와 동일)")]
    public BookSlot[] slots = new BookSlot[4];

    [Header("배치용 책 프리팹 (PlacementBookItem.cs 붙은 것)")]
    public GameObject placementBookPrefab;
    public Transform[] placementSpawnPoints;

    [Header("정답 시 연출")]
    public Animator bookshelfAnimator;
    public GameObject hiddenKey;

    [Header("HintManager 연결")]
    public HintManager hintManager;

    List<GameObject> spawnedBooks = new List<GameObject>();
    PlacementBookItem heldItem;
    bool isSolved = false;
    bool isFocused = false;

    // ── 책장에서 책 뽑았을 때 (Book.cs가 호출) ──
    public void OnBookCollected(char letter)
    {
        hintManager?.AddLastAction("collect_book_" + letter);

        Debug.Log($"[BookShelfPuzzle] 책 '{letter}' 수집됨");   // ← 추가

        if (HasAllAnswerLetters() && hintManager != null &&
            !hintManager.currentPlayerState.completedSteps.Contains(5))
        {
            hintManager.currentPlayerState.completedSteps.Add(5);
            Debug.Log("[BookShelfPuzzle] Step 5 완료 — 정답 알파벳 4권 모두 수집");   // ← 추가
        }
    }

    bool HasAllAnswerLetters()
    {
        foreach (char c in puzzleSolver.answerFurnitureName)
            if (InventoryManager.Instance == null || !InventoryManager.Instance.HasItem("book_" + char.ToUpper(c)))
                return false;
        return true;
    }

    // ── 빈 칸(슬롯 영역) 클릭했을 때 ──
    public void OnBookshelfClicked()
    {
        if (isSolved || isFocused) return;
        isFocused = true;

        Debug.Log("[BookShelfPuzzle] 책장 슬롯 영역 포커스 진입"); 

        SpawnPlacementBooks();
    }

    void SpawnPlacementBooks()
    {
        ClearSpawnedBooks();
        if (InventoryManager.Instance == null) return;

        var bookLetters = new List<char>();
        foreach (string itemId in InventoryManager.Instance.Items)
        {
            if (itemId.StartsWith("book_") && itemId.Length == 6)
                bookLetters.Add(itemId[5]);
        }

        for (int i = 0; i < bookLetters.Count && i < placementSpawnPoints.Length; i++)
        {
            GameObject obj = Instantiate(placementBookPrefab, placementSpawnPoints[i].position, placementSpawnPoints[i].rotation);
            var item = obj.GetComponent<PlacementBookItem>();
            item.letter = bookLetters[i];
            item.bookShelfPuzzle = this;
            spawnedBooks.Add(obj);
        }

        Debug.Log($"[BookShelfPuzzle] 배치용 책 {spawnedBooks.Count}권 스폰됨");   // ← 추가
    }

    void ClearSpawnedBooks()
    {
        foreach (var obj in spawnedBooks)
            if (obj != null) Destroy(obj);
        spawnedBooks.Clear();
    }

    // ── 스폰된 책 클릭 → 손에 들기 ──
    public void TryHoldBook(PlacementBookItem item)
    {
        if (isSolved || heldItem != null) return;

        heldItem = item;
        item.SetHeld(true);
        hintManager?.AddLastAction("hold_book_" + item.letter);
    }

    // ── 슬롯 클릭 → 배치 ──
    public void TryPlaceBook(BookSlot slot)
    {
        if (isSolved || heldItem == null || slot.placedLetter != '\0') return;

        slot.placedLetter = heldItem.letter;
        slot.SetFilled(true);   // 유령책 → 불투명 전환

        InventoryManager.Instance?.RemoveItem("book_" + heldItem.letter);
        hintManager?.AddLastAction("place_book_slot_" + slot.slotIndex);

        spawnedBooks.Remove(heldItem.gameObject);
        Destroy(heldItem.gameObject);
        heldItem = null;

        CheckCompletion();
    }

    void CheckCompletion()
    {
        foreach (var slot in slots)
            if (slot.placedLetter == '\0') return;

        string placedOrder = "";
        foreach (var slot in slots)
            placedOrder += slot.placedLetter;

        if (placedOrder.ToUpper() == puzzleSolver.answerFurnitureName.ToUpper())
        {
            isSolved = true;
            Solve();
        }
        else
        {
            Debug.Log("[BookShelfPuzzle] 순서 오답: " + placedOrder + " (정답: " + puzzleSolver.answerFurnitureName + ")");
            hintManager?.AddLastAction("wrong_book_order");
            ResetSlots();
        }
    }

    void ResetSlots()
    {
        foreach (var slot in slots)
        {
            if (slot.placedLetter != '\0')
                InventoryManager.Instance?.AddItem("book_" + slot.placedLetter);

            slot.placedLetter = '\0';
            slot.SetFilled(false);   // 다시 반투명 유령책으로
        }
        SpawnPlacementBooks();
    }

    void Solve()
    {
        Debug.Log("[BookShelfPuzzle] 정답! 책장이 밀려나며 열쇠 발견");

        ClearSpawnedBooks();
        //focus?.EndFocus();
        isFocused = false;

        if (bookshelfAnimator != null) bookshelfAnimator.SetTrigger("Slide");
        if (hiddenKey != null) hiddenKey.SetActive(true);

        if (hintManager != null)
        {
            if (!hintManager.currentPlayerState.completedSteps.Contains(6))
            {
                hintManager.currentPlayerState.completedSteps.Add(6);
                Debug.Log("[BookShelfPuzzle] Step 6 완료");   // ← 추가
            }
            if (!hintManager.currentPlayerState.foundClues.Contains("clue_bookshelf_order"))
            {
                hintManager.currentPlayerState.foundClues.Add("clue_bookshelf_order");
                Debug.Log("[BookShelfPuzzle] clue_bookshelf_order 획득 — 와인방 퍼즐 완료");   // ← 추가
            }
        }
    }

    // ── 포커스 도중 나가기 (ESC 등, 선택사항) ──
    public void ExitFocus()
    {
        if (isSolved) return;
        isFocused = false;
        //focus?.EndFocus();
        ClearSpawnedBooks();
    }
}