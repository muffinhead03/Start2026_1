using UnityEngine;
using UnityEngine.InputSystem;

public class Player_MouseInteraction : MonoBehaviour
{
    [Header("Raycast Settings")]
    [SerializeField] private float rayDistance = 0.5f;

    [Header("Button Layer")]
    [SerializeField] LayerMask button_layer;

    private IMouseInteractable CurrentTarget;
    private IMouseInteractable DragTarget;

    InputAction click;

    bool isActive;

    private void Start()
    {
        isActive = false;

        click = InputSystem.actions.FindAction("Click");
    }

    public void SetEnable()
    {
        isActive = true;

        RegisterInput();
    }

    public void SetDisable()
    {
        isActive = false;

        UnRegisterInput();
    }

    void RegisterInput()
    {
        click.performed += OnClickPerformed;
        click.canceled += OnClickReleased;
    }

    void UnRegisterInput()
    {
        click.performed -= OnClickPerformed;
        click.canceled -= OnClickReleased;
    }

    private void OnClickPerformed(InputAction.CallbackContext ctx)
    {
        OnClick();
    }

    private void OnClickReleased(InputAction.CallbackContext ctx)
    {
        OnReleased();
    }

    private void Update()
    {
        if (!isActive || DragTarget != null) return;

        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, rayDistance, button_layer))
        {
            var interactable = hit.collider.GetComponent<IMouseInteractable>();

            if (interactable != null)
            {
                if (CurrentTarget != interactable)
                {
                    CurrentTarget?.OnMouseExit();

                    CurrentTarget = interactable;
                    CurrentTarget.OnMouseEnter();
                }
            }
            else
            {
                if (CurrentTarget != null)
                {
                    CurrentTarget.OnMouseExit();
                    CurrentTarget = null;
                }
            }
        }
        else
        {
            if (CurrentTarget != null)
            {
                CurrentTarget.OnMouseExit();
                CurrentTarget = null;
            }
        }
    }
    
    void OnClick()
    {
        if(!isActive) return;

        if(CurrentTarget != null)
        {
            CurrentTarget.OnMouseClick();

            CurrentTarget.OnMouseExit();

            DragTarget = CurrentTarget;
        }
    }

    void OnReleased()
    {
        if (!isActive) return;

        if (DragTarget != null)
        {
            DragTarget.OnMouseReleased();
            DragTarget = null;

            CurrentTarget = null;
        }
    }
}