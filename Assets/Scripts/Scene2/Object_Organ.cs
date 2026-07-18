using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class Object_Organ : MonoBehaviour
{
    [Header("Answer")]
    [SerializeField] string answer;

    [Header("해제 완료")]
    [SerializeField] UnityEvent Unlock;

    public bool isDebug;
    bool isPipeChanged;
    bool isActive;
    string input;

    InputAction click;

    void Start()
    {
        isPipeChanged = false;
        if (isDebug) isPipeChanged = true;

        input = "";

        click = InputSystem.actions.FindAction("Click");
        click.Disable();

        isActive = false;
    }

    public void SetActive(bool active)
    {
        isActive = active;

        if (active)
        {
            input = "";
            click.performed += ctx => PressButton();
            click.Enable();
        }
        else
        {
            click.performed -= ctx => PressButton();
            click.Disable();
        }
    }

    public void PressButton()
    {
        if (!isPipeChanged) return;

        if (!isActive) return;

        Vector2 mousePosition = Mouse.current.position.ReadValue();

        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 0.8f))
        {
            var button = hit.collider.GetComponent<Object_Pwd_Button>();

            if (button != null)
            {
                string n = button.id;

                button.Pressed();

                input += n;
                if (input.Length >= answer.Length) CheckAnswer();
            }
        }
    }

    void CheckAnswer()
    {
        if (answer == input)
        {
            Unlock?.Invoke();
            GetComponent<Object_FixCamera>().UnFixCamera();
        }
        else
        {
            GetComponent<Object_FixCamera>().UnFixCamera();
        }
    }
}
