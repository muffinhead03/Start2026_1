using UnityEngine;

public class BookSlotSetup : MonoBehaviour
{
    public PuzzleSolver puzzleSolver;
    public Object_Puzzle objectPuzzle;
    public WineBookPutOn[] slots = new WineBookPutOn[4];

    // 변경: Start → Awake
    // 모든 Awake는 모든 Start보다 먼저 실행되므로,
    // Object_Puzzle.Start()가 pwd 길이로 배열을 만들기 전에 pwd가 확실히 세팅됨
    void Awake()
    {
        // 변경: answerFurnitureName 대신 GetAnswer() — PuzzleSolver.Start()가 아직 안 돌았어도 정답을 계산해서 줌
        string answer = puzzleSolver.GetAnswer();

        for (int i = 0; i < slots.Length && i < answer.Length; i++)
            slots[i].SetKeyName("book_" + char.ToUpper(answer[i]));

        objectPuzzle.pwd = answer.ToUpper();
    }
}