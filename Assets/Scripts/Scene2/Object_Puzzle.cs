using UnityEngine;
using UnityEngine.Events;

public class Object_Puzzle : MonoBehaviour
{
    char[] input;

    [Header("정답")]
    public string pwd;

    [Header("해제 이벤트")]
    public UnityEvent UnlockEvent;

    void Start()
    {
        Init();
    }

    void Init()
    {
        input = new char[pwd.Length];
        for (int i = 0; i < input.Length; i++) input[i] = '-';
    }

    public void PieceOn(int id, char num)
    {
        if (id >= input.Length)
        {
            Debug.Log("Index out of range :" + id);
            return;
        }

        input[id] = num;

        CheckResult();
    }

    public void PieceOff(int id)
    {
        if (id >= input.Length)
        {
            Debug.Log("Index out of range :" + id);
            return;
        }

        input[id] = '-';
    }

    void CheckResult()
    {
        string input_str = new string(input);
        if (input_str == pwd) UnlockEvent?.Invoke();

        Debug.Log(input_str);
    }
}
