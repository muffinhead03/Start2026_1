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
        click = InputSystem.actions.FindAction("click");

        click.performed += ctx => PressButton();

        active = false;
    }

    void PressButton()
    {
        if (!active) return;

        OnClick?.Invoke();
        if (pivot.rotation.eulerAngles.z >= 30) count++;
        else count--;

        Debug.Log(count);
        if (count == 0) GetComponent<Object_FixCamera>().OnFix();
        else if (count == 5)
        {
            obj.UnlockDoor();
            GetComponent<Object_FixCamera>().OnFix();
        }
    }

    public void SetEnable(bool active)
    {
        this.active = active;

        if (active) ResetGame();
    }

    void ResetGame()
    {
        count = 4;
    }
}
