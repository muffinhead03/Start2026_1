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
    if (inventoryUIManager == null)
    {
        Debug.LogError(
            "[InventoryInputBridge] " +
            "InventoryUIManager가 없어 " +
            "스크롤을 처리할 수 없습니다.",
            this
        );

        return;
    }

    /*
     * UI/ScrollWheel은 float가 아니라
     * Vector2 값을 전달합니다.
     *
     * x: 가로 스크롤
     * y: 세로 마우스 휠
     */
    Vector2 scrollValue =
        context.ReadValue<Vector2>();

    float scrollY =
        scrollValue.y;

    if (showDebugLog)
    {
        string controlPath = "null";

        if (context.control != null)
        {
            controlPath =
                context.control.path;
        }

        Debug.Log(
            "[InventoryInputBridge] 스크롤 입력 감지: " +
            "X=" + scrollValue.x +
            ", Y=" + scrollValue.y +
            ", Control=" + controlPath,
            this
        );
    }

    /*
     * 입력이 끝날 때 전달되는 0 값은 무시합니다.
     */
    if (Mathf.Abs(scrollY) < 0.01f)
    {
        return;
    }

    /*
     * 휠 위: 이전 아이템
     * 휠 아래: 다음 아이템
     */
    int direction =
        scrollY > 0f
            ? -1
            : 1;

    if (showDebugLog)
    {
        Debug.Log(
            "[InventoryInputBridge] 순환 요청 전달: " +
            "Direction=" + direction,
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