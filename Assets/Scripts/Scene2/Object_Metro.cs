using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class Object_Metro : MonoBehaviour
{
    public Object_Door obj;
    public UnityEvent OnClick;
    public Transform pivot;

    [Header("빛 효과")]
    public MeshRenderer[] lights;
    public Material[] materials;

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
        if (pivot.rotation.eulerAngles.z >= 30)
        {
            if (count < 0) count = 1;
            else count++;
        }
        else count--;

        TurnLight(count);

        Debug.Log(count);
        if (count == -3) GetComponent<Object_FixCamera>().UnFixCamera();
        else if (count == 3)
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
            click.performed += ctx => PressButton();
            click.Enable();
        }
        else
        {
            click.performed -= ctx => PressButton();
            click.Disable(); 
            
            ResetGame();
        }
    }

    void ResetGame()
    {
        count = 0;

        TurnLight(0);
    }

    void TurnLight(int count)
    {
        int i;
        for(i = 0; i < lights.Length && i < count; i++)
        {
            lights[i].material = materials[0];
        }

        for (; i < lights.Length; i++)
        {
            lights[i].material = materials[1];
        }
    }
}
