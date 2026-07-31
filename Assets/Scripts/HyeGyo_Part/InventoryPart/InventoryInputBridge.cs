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
        RegisterAction(
            inventoryToggleAction,
            HandleInventoryToggle
        );

        RegisterAction(
            inventoryCloseAction,
            HandleInventoryClose
        );

        RegisterAction(
            inventoryConfirmAction,
            HandleInventoryConfirm
        );
    }

    private void OnDisable()
    {
        UnregisterAction(
            inventoryToggleAction,
            HandleInventoryToggle
        );

        UnregisterAction(
            inventoryCloseAction,
            HandleInventoryClose
        );

        UnregisterAction(
            inventoryConfirmAction,
            HandleInventoryConfirm
        );
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

    private static void RegisterAction(
        InputActionReference actionReference,
        System.Action<InputAction.CallbackContext> callback)
    {
        if (actionReference == null ||
            actionReference.action == null)
        {
            return;
        }

        // 혹시 이전 등록이 남아 있어도 중복 실행되지 않게 합니다.
        actionReference.action.performed -= callback;
        actionReference.action.performed += callback;

        actionReference.action.Enable();
    }

    private static void UnregisterAction(
        InputActionReference actionReference,
        System.Action<InputAction.CallbackContext> callback)
    {
        if (actionReference == null ||
            actionReference.action == null)
        {
            return;
        }

        actionReference.action.performed -= callback;
        actionReference.action.Disable();
    }
}