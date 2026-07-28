using UnityEngine;

// 사용법:
// 1. book_sc1 복제해서 만든 배치용 프리팹에 이 스크립트 추가 (Book.cs는 떼어내기)
// 2. Event_On_Ray 컴포넌트 OnClick → OnClickBook() 연결
// 3. letter, bookShelfPuzzle은 BookShelfPuzzle.SpawnPlacementBooks()에서 자동으로 채워줌 (인스펙터에서 직접 입력 안 해도 됨)
public class PlacementBookItem : MonoBehaviour
{
    [HideInInspector] public char letter;
    [HideInInspector] public BookShelfPuzzle bookShelfPuzzle;

    bool isHeld = false;

    public void OnClickBook()
    {
        if (isHeld) return;
        bookShelfPuzzle.TryHoldBook(this);
    }

    public void SetHeld(bool held)
    {
        isHeld = held;
        // TODO: 아트 붙으면 하이라이트/기울임 등 시각 표시 추가
    }
}