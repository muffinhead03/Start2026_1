using UnityEngine;
using UnityEngine.Events;

public class Object_Pwd_Button_2 : MonoBehaviour, APwdButton
{
    [SerializeField] private string id;
    public UnityEvent OnPressed;

    public string GetId() { return id; }

    public void Pressed()
    {
        OnPressed?.Invoke();
    }
}
