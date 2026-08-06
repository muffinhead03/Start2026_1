using UnityEngine;
using UnityEngine.InputSystem;

[DisallowMultipleComponent]
public sealed class InventoryInputBridge : MonoBehaviour
{
    [Header("Inventory")]
    [SerializeField]
    private InventoryUIManager inventoryUIManager;

    [Header("Input Actions")]
    [SerializeField]
    private InputActionReference inventoryToggleAction;

    [SerializeField]
    private InputActionReference inventoryCloseAction;

    [SerializeField]
    private InputActionReference inventoryConfirmAction;

    [SerializeField]
    private InputActionReference inventoryScrollAction;

    [Header("Debug")]
    [SerializeField]
    private bool showDebugLog = true;

    private void Awake()
    {
        if (inventoryUIManager == null)
        {
            inventoryUIManager =
                GetComponentInChildren<InventoryUIManager>(true);
        }

        if (inventoryUIManager == null)
        {
            Debug.LogError(
                "[InventoryInputBridge] " +
                "InventoryUIManager가 연결되지 않았습니다.",
                this
            );

            enabled = false;
        }
    }

    private void OnEnable()
    {
        RegisterAction(inventoryToggleAction,HandleInventoryToggle);
        RegisterAction(inventoryCloseAction,HandleInventoryClose);
        RegisterAction(inventoryConfirmAction,HandleInventoryConfirm);
        RegisterAction(inventoryScrollAction,HandleInventoryScroll);
        string scrollReferenceName =
    inventoryScrollAction != null
        ? inventoryScrollAction.name
        : "null";

if (showDebugLog)
{
    Debug.Log(
        $"[InventoryInputBridge] OnEnable 완료: " +
        $"ScrollReference={scrollReferenceName}",
        this
    );
}
    }

    private void OnDisable()
    {
        UnregisterAction(inventoryToggleAction,HandleInventoryToggle);
        UnregisterAction(inventoryCloseAction,HandleInventoryClose);
        UnregisterAction(inventoryConfirmAction,HandleInventoryConfirm);
        UnregisterAction(inventoryScrollAction,HandleInventoryScroll);
    }

    private void HandleInventoryToggle(
        InputAction.CallbackContext context)
    {
        if (inventoryUIManager == null)
            return;

        inventoryUIManager.ToggleInventory();
    }

    private void HandleInventoryClose(
        InputAction.CallbackContext context)
    {
        if (inventoryUIManager == null ||
            !inventoryUIManager.IsOpen)
        {
            return;
        }

        inventoryUIManager.CloseInventory();
    }

   private void HandleInventoryScroll(
    InputAction.CallbackContext context)
{
    if (showDebugLog)
    {
        Debug.Log(
            $"[InventoryInputBridge] 스크롤 콜백 발생: " +
            $"Phase={context.phase}, " +
            $"Control=" +
            $"{(context.control != null ? context.control.path : "null")}",
            this
        );
    }

    if (inventoryUIManager == null)
    {
        Debug.LogError(
            "[InventoryInputBridge] " +
            "InventoryUIManager가 없어서 스크롤을 처리할 수 없습니다.",
            this
        );

        return;
    }

    float scrollY =
        context.ReadValue<float>();

    if (showDebugLog)
    {
        Debug.Log(
            $"[InventoryInputBridge] 스크롤 입력값: " +
            $"ScrollY={scrollY}",
            this
        );
    }

    /*
     * Pass Through 액션은 휠 입력이 끝나면서
     * 값 0도 전달될 수 있으므로 무시합니다.
     */
    if (Mathf.Abs(scrollY) < 0.01f)
    {
        if (showDebugLog)
        {
            Debug.Log(
                "[InventoryInputBridge] " +
                "스크롤 값이 0에 가까워 무시합니다.",
                this
            );
        }

        return;
    }

    int direction =
        scrollY > 0f
            ? -1
            : 1;

    if (showDebugLog)
    {
        Debug.Log(
            $"[InventoryInputBridge] 순환 요청 전달: " +
            $"Direction={direction}",
            this
        );
    }

    inventoryUIManager.CycleSelectedItem(
        direction
    );
}

    private void HandleInventoryConfirm(
        InputAction.CallbackContext context)
    {
        if (inventoryUIManager == null ||
            !inventoryUIManager.IsOpen)
        {
            return;
        }

        inventoryUIManager.ConfirmSelectedItem();
    }

    private void RegisterAction(
    InputActionReference actionReference,
    System.Action<InputAction.CallbackContext> callback)
{
    if (actionReference == null)
    {
        Debug.LogError(
            "[InventoryInputBridge] " +
            "InputActionReference가 연결되지 않았습니다.",
            this
        );

        return;
    }

    if (actionReference.action == null)
    {
        Debug.LogError(
            "[InventoryInputBridge] " +
            "InputActionReference 안에 Action이 없습니다.",
            this
        );

        return;
    }

    InputAction action =
        actionReference.action;

    action.performed -= callback;
    action.performed += callback;

    action.Enable();

    if (showDebugLog)
    {
        Debug.Log(
            $"[InventoryInputBridge] 액션 등록 완료: " +
            $"Action={action.name}, " +
            $"Map=" +
            $"{(action.actionMap != null ? action.actionMap.name : "null")}, " +
            $"Type={action.type}, " +
            $"ExpectedControlType={action.expectedControlType}, " +
            $"BindingCount={action.bindings.Count}, " +
            $"Enabled={action.enabled}",
            this
        );
    }
}

private void UnregisterAction(
    InputActionReference actionReference,
    System.Action<InputAction.CallbackContext> callback)
{
    if (actionReference == null ||
        actionReference.action == null)
    {
        return;
    }

    InputAction action =
        actionReference.action;

    action.performed -= callback;
    action.Disable();

    if (showDebugLog)
    {
        Debug.Log(
            $"[InventoryInputBridge] 액션 해제: " +
            $"Action={action.name}",
            this
        );
    }
}
}