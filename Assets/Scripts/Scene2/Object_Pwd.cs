using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.InputSystem;

public class Object_Pwd : MonoBehaviour
{
    public string pwd;
    public Object_Pwd_Locked obj;
    public HintManager hintManager; // 추가

    bool isFixing;

    private string input="";

    InputAction click;

    void Start()
    {
        click = InputSystem.actions.FindAction("Click");

        click.performed += ctx => PressButton();

        isFixing = false;
        SetActiveButtons();
    }

    public void SetActiveButtons()
    {
        foreach(Object_Pwd_Button button in GetComponentsInChildren<Object_Pwd_Button>())
        {
            button.gameObject.GetComponent<Collider>().enabled = isFixing;
        }

        isFixing = !isFixing;
    }

    public void PressButton()
    {
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
                        obj.UnlockDoor();
                        hintManager.currentPlayerState.completedSteps.Add(5);
                    }
                    else input = "";
                }
                else input += n;
            }
        }
    }
}
