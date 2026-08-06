using UnityEngine;

public class Object_Puzzle_Piece : MonoBehaviour
{
    public Object_Puzzle puzzle;

    public Object_PutOn putOn;

    public int id;

    public void PutDown()
    {
        char num = putOn.GetKeyId();

        puzzle.PieceOn(id, num);
    }

    public void PickUp()
    {
        puzzle.PieceOff(id);
    }
}
