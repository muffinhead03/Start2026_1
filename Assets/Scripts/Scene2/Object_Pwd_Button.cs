using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Object_Pwd_Button : APwdButton, IMouseInteractable
{
    public UnityEvent OnPressed;

    Collider col;

    public string GetId() { return id; }

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

        StartCoroutine(ButtonDelay());
    }

    IEnumerator ButtonDelay()
    {
        if (col == null) col = GetComponent<Collider>();

        col.enabled = false;

        yield return new WaitForSeconds(0.5f);

        col.enabled = true;
    }
}
