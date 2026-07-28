using UnityEngine;

// 사용법:
// 1. 책장에 꽂힌 책 오브젝트 각각에 이 스크립트 추가
// 2. letter에 이 책 제목의 첫 글자 입력 (예: 'A', 'B', ...)
// 3. bookShelfPuzzle 슬롯에 씬의 BookShelfPuzzle 오브젝트 연결
// 4. Event_On_Ray 컴포넌트 OnClick → OnClickBook() 연결
public class Book : MonoBehaviour
{
    [Header("이 책 제목의 첫 글자")]
    public char letter;

    [Header("퍼즐 매니저 연결")]
    public BookShelfPuzzle bookShelfPuzzle;

    bool isCollected = false;

    public void OnClickBook()
    {
        if (isCollected) return;

        string itemId = "book_" + letter;

        if (InventoryManager.Instance != null && InventoryManager.Instance.AddItem(itemId))
        {
            isCollected = true;
            bookShelfPuzzle.OnBookCollected(letter);
            gameObject.SetActive(false);   // 책장에서 사라짐 (인벤토리로 들어간 느낌)
        }
    }
}