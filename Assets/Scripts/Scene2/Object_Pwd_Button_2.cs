using UnityEngine;
using UnityEngine.Events;

public class Object_Pwd_Button_2 : APwdButton, IMouseInteractable
{
    public UnityEvent OnPressed;

    public void OnMouseEnter() { }

    public void OnMouseExit() { }

    public void OnMouseReleased() { }

    public void OnMouseClick()
    {
        Pressed();
    }

    public override void Pressed()
    {
        object_pwd.PressButton(id);

        OnPressed?.Invoke();
    }
}
