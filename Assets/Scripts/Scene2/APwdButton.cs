using UnityEngine;
using UnityEngine.Events;

public abstract class APwd: MonoBehaviour
{
    [Header("정답")]
    public string pwd;

    [Header("해제 이벤트")]
    public UnityEvent UnlockEvent;

    protected bool isActive;

    protected string input;

    public abstract void PressButton(string n);

    protected abstract bool CheckAnswer();
}

public abstract class APwdButton: MonoBehaviour
{
    [SerializeField] protected string id;

    [SerializeField] protected APwd object_pwd;


    public abstract void Pressed();
}
