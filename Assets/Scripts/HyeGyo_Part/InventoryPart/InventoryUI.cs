using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private InventorySlot[] slots;

    private int selectedSlotIndex;

    public int SelectedSlotIndex => selectedSlotIndex;

    public string SelectedItemId
    {
        get
        {
            if (!IsValidSlotIndex(selectedSlotIndex))
            {
                return null;
            }

            return slots[selectedSlotIndex].CurrentItemId;
        }
    }

    private void Start()
    {
        if (slots == null || slots.Length != 8)
        {
            Debug.LogError("Slots 배열에 슬롯 8개를 연결해야 합니다.");
            return;
        }

        foreach (InventorySlot slot in slots)
        {
            if (slot != null)
            {
                slot.ClearSlot();
            }
        }

        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.OnInventoryChanged += RefreshUI;
        }

        SelectSlot(0);
        RefreshUI();
    }

    private void Update()
    {
        CheckNumberKeyInput();
    }

    private void OnDestroy()
    {
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.OnInventoryChanged -= RefreshUI;
        }
    }

    private void CheckNumberKeyInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            SelectSlot(0);
        else if (Input.GetKeyDown(KeyCode.Alpha2))
            SelectSlot(1);
        else if (Input.GetKeyDown(KeyCode.Alpha3))
            SelectSlot(2);
        else if (Input.GetKeyDown(KeyCode.Alpha4))
            SelectSlot(3);
        else if (Input.GetKeyDown(KeyCode.Alpha5))
            SelectSlot(4);
        else if (Input.GetKeyDown(KeyCode.Alpha6))
            SelectSlot(5);
        else if (Input.GetKeyDown(KeyCode.Alpha7))
            SelectSlot(6);
        else if (Input.GetKeyDown(KeyCode.Alpha8))
            SelectSlot(7);
    }

    public void SelectSlot(int index)
    {
        if (!IsValidSlotIndex(index))
        {
            return;
        }

        selectedSlotIndex = index;

        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] != null)
            {
                slots[i].SetSelected(i == selectedSlotIndex);
            }
        }

        Debug.Log(
            $"선택 슬롯: {selectedSlotIndex + 1}, " +
            $"아이템: {SelectedItemId ?? "없음"}"
        );
    }

    public void RefreshUI()
    {
        if (InventoryManager.Instance == null)
        {
            return;
        }

        foreach (InventorySlot slot in slots)
        {
            if (slot != null)
            {
                slot.ClearSlot();
            }
        }

        int itemCount = Mathf.Min(
            InventoryManager.Instance.Items.Count,
            slots.Length
        );

        for (int i = 0; i < itemCount; i++)
        {
            string itemId = InventoryManager.Instance.Items[i];

            slots[i].SetItem(
                itemId,
                GetItemIcon(itemId)
            );
        }

        SelectSlot(selectedSlotIndex);
    }

    private Sprite GetItemIcon(string itemId)
    {
        // 추후 아이템별 이미지 반환
        return null;
    }

    private bool IsValidSlotIndex(int index)
    {
        return slots != null
            && index >= 0
            && index < slots.Length
            && slots[index] != null;
    }
}