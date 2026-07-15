using UnityEngine;
using UnityEngine.Events;

public class Object_Pwd_Button : MonoBehaviour
{
    public string id;
    public UnityEvent OnPressed;

    public void Pressed() => OnPressed?.Invoke();
}
