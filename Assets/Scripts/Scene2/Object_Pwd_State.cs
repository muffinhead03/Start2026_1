using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class Object_Pwd_State : MonoBehaviour
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
        for (int i = 0; i < input.Length; i++) input[i] = '0';
    }

    public void PressButton(int id)
    {
        if(id >= input.Length)
        {
            Debug.Log("Index out of range :" + id);
            return;
        }

        if (input[id] == '0') input[id] = '1';
        else input[id] = '0';

        CheckResult();
    }

    void CheckResult()
    {
        string input_str = new string(input);
        if (input_str == pwd) UnlockEvent?.Invoke();
    }
}
