using UnityEngine;

public class BookSlotSetup : MonoBehaviour
{
    public PuzzleSolver puzzleSolver;
    public Object_Puzzle objectPuzzle;
    public WineBookPutOn[] slots = new WineBookPutOn[4];

    void Start()
    {
        string answer = puzzleSolver.answerFurnitureName;

        for (int i = 0; i < slots.Length && i < answer.Length; i++)
            slots[i].SetKeyName("book_" + char.ToUpper(answer[i]));

        objectPuzzle.pwd = answer.ToUpper();
    }
}