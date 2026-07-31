using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryInputBridge : MonoBehaviour
{
    [Header("인벤토리")]
    [SerializeField]
    private InventoryUIManager inventoryUIManager;

    private void Update()
    {
        Keyboard keyboard = Keyboard.current;

        if (keyboard == null ||
            inventoryUIManager == null)
        {
            return;
        }

        // I: 인벤토리 열기/닫기
        if (keyboard.iKey.wasPressedThisFrame)
        {
            Debug.Log("InventoryInputBridge: I 키 감지");

            inventoryUIManager.ToggleInventory();
            return;
        }

        if (!inventoryUIManager.IsOpen)
            return;

        // ESC: 닫기
        if (keyboard.escapeKey.wasPressedThisFrame)
        {
            inventoryUIManager.CloseInventory();
            return;
        }

        // E: 선택한 아이템 장착
        if (keyboard.eKey.wasPressedThisFrame)
        {
            inventoryUIManager.ConfirmSelectedItem();
        }
    }
}