using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class Object_Metro : MonoBehaviour
{
    public Object_Pwd_Locked obj;
    public UnityEvent OnClick;
    public Transform pivot;

    int count;

    InputAction click;
    bool active;

    void Start()
    {
        click = InputSystem.actions.FindAction("Click");
        click.Disable();

        active = false;
    }

    void PressButton()
    {
        if (!active) return;

        OnClick?.Invoke();
        if (pivot.rotation.eulerAngles.z >= 30) count++;
        else count--;

        Debug.Log(count);
        if (count == 0) GetComponent<Object_FixCamera>().UnFixCamera();
        else if (count == 5)
        {
            obj.UnlockDoor();
            GetComponent<Object_FixCamera>().UnFixCamera();
        }
    }

    public void SetEnable(bool active)
    {
        this.active = active;

        if (active)
        {
            ResetGame();

            click.performed += ctx => PressButton();
            click.Enable();
        }
        else
        {
            click.performed -= ctx => PressButton();
            click.Disable();
        }
    }

    void ResetGame()
    {
        count = 4;
    }
}
