using UnityEngine;
using UnityEngine.Events;

public class Object_Pwd_Button_2 : APwdButton
{
    public UnityEvent OnPressed;

    public override void Pressed()
    {
        object_pwd.PressButton(id);

        OnPressed?.Invoke();
    }
}
