using UnityEngine;
using UnityEngine.InputSystem;

[DisallowMultipleComponent]
public sealed class InventoryInputBridge : MonoBehaviour
{
    [SerializeField] private InventoryUIManager inventoryUIManager;

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
                "[InventoryInputBridge] InventoryUIManager가 연결되지 않았습니다.",
                this
            );

            enabled = false;
        }
    }

    private void Update()
    {
        Keyboard keyboard = Keyboard.current;

        if (keyboard == null ||
            inventoryUIManager == null)
        {
            return;
        }

        if (keyboard.iKey.wasPressedThisFrame)
        {
            inventoryUIManager.ToggleInventory();
            return;
        }

        if (!inventoryUIManager.IsOpen)
            return;

        if (keyboard.escapeKey.wasPressedThisFrame)
        {
            inventoryUIManager.CloseInventory();
            return;
        }

        if (keyboard.eKey.wasPressedThisFrame)
            inventoryUIManager.ConfirmSelectedItem();
    }
}
