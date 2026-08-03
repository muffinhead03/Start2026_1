using UnityEngine;

public class WineBookPuzzlePiece : MonoBehaviour
{
    public Object_Puzzle puzzle;
    public WineBookPutOn putOn;
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