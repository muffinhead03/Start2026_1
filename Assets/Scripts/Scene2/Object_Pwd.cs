using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class Object_Pwd : MonoBehaviour
{
    [Header("정답")]
    public string pwd;

    [Header("해제 이벤트")]
    public UnityEvent UnlockEvent;

    public HintManager hintManager; // 추가

    bool isActive;

    private string input="";

    InputAction click;

    void Start()
    {
        click = InputSystem.actions.FindAction("Click");
        click.Disable();

        isActive = false;
    }

    public void SetActive(bool active)
    {
        isActive = active;

        if (active)
        {
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
        if (!isActive) return;

        Vector2 mousePosition = Mouse.current.position.ReadValue();

        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 0.5f))
        {
            var button = hit.collider.GetComponent<Object_Pwd_Button>();

            if(button != null)
            {
                string n = button.id;

                button.Pressed();

                if (n == "submit")
                {
                    if (pwd == input)
                    {
                        UnlockEvent?.Invoke();
                        hintManager?.currentPlayerState.completedSteps.Add(5);
                    }
                    else input = "";
                }
                else input += n;
            }
        }
    }
}
