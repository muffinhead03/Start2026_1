using System;
using TMPro;
using UnityEngine;

[DisallowMultipleComponent]
public sealed class InventoryUIEffect : MonoBehaviour
{
    [Header("Canvas")]
    [SerializeField]
    private GameObject inventoryRoot;

    [Header("Slots")]
    [SerializeField]
    private InventorySlotView[] slots;

    [Header("Selected Item")]
    [SerializeField]
    private TMP_Text selectedNameText;

    [SerializeField]
    private TMP_Text selectedDescriptionText;

    [Header("Preview")]
    [SerializeField]
    private InventoryPreviewView previewView;

    private int lastLoggedSelectedIndex = int.MinValue;

    public void BindSlots(Action<int> onSlotClicked)
    {
        if (slots == null || slots.Length == 0)
        {
            Debug.LogError(
                "[InventoryUIEffect] Slots 배열이 비어 있습니다.",
                this
            );

            return;
        }

        if (onSlotClicked == null)
        {
            Debug.LogError(
                "[InventoryUIEffect] 슬롯 클릭 콜백이 없습니다.",
                this
            );

            return;
        }

        for (int i = 0; i < slots.Length; i++)
        {
            InventorySlotView slot = slots[i];

            if (slot == null)
            {
                Debug.LogError(
                    $"[InventoryUIEffect] Slots Element {i}가 비어 있습니다.",
                    this
                );

                continue;
            }

            slot.Bind(i, onSlotClicked);
        }
    }

    public void SetOpen(bool open)
    {
        if (inventoryRoot != null)
        {
            inventoryRoot.SetActive(open);
        }
        else
        {
            Debug.LogError(
                "[InventoryUIEffect] Inventory Root가 연결되지 않았습니다.",
                this
            );
        }

        if (!open)
            previewView?.Hide();
    }

    public void Refresh(
        InventoryData data,
        InventoryItemData selectedItem,
        bool inventoryIsOpen)
    {
        RefreshSlots(data);

        RefreshSelectedInformation(
            selectedItem,
            inventoryIsOpen
        );
    }

    public void PlayInventoryFull()
    {
        Debug.Log(
            "[InventoryUIEffect] 인벤토리가 가득 찼습니다.",
            this
        );
    }

    private void RefreshSlots(InventoryData data)
    {
        if (slots == null || slots.Length == 0)
            return;

        int selectedIndex =
            data != null
                ? data.SelectedIndex
                : -1;

        int equippedIndex =
            data != null
                ? data.EquippedIndex
                : -1;

        if (lastLoggedSelectedIndex != selectedIndex)
        {
            lastLoggedSelectedIndex = selectedIndex;

            Debug.Log(
                $"[InventoryUIEffect] " +
                $"SelectedIndex={selectedIndex}, " +
                $"EquippedIndex={equippedIndex}",
                this
            );
        }

        for (int i = 0; i < slots.Length; i++)
        {
            InventorySlotView slot = slots[i];

            if (slot == null)
                continue;

            InventoryItemData item =
                data != null
                    ? data.GetItemAt(i)
                    : null;

            bool isSelected =
                i == selectedIndex;

            bool isEquipped =
                i == equippedIndex;

            string displayName =
                item != null
                    ? item.ItemName
                    : "—";

            /*
             * 아이템 존재 여부와 관계없이 SelectedIndex가 같으면
             * 선택 프레임을 활성화합니다.
             */
            slot.Refresh(
                displayName,
                isSelected,
                isEquipped
            );
        }
    }

    private void RefreshSelectedInformation(
        InventoryItemData selectedItem,
        bool inventoryIsOpen)
    {
        if (selectedNameText != null)
        {
            selectedNameText.text =
                selectedItem != null
                    ? selectedItem.ItemName
                    : "—";
        }

        if (selectedDescriptionText != null)
        {
            selectedDescriptionText.text =
                selectedItem != null
                    ? selectedItem.Description
                    : string.Empty;
        }

        if (!inventoryIsOpen ||
            selectedItem == null)
        {
            previewView?.Hide();
            return;
        }

        previewView?.Show(selectedItem);
    }
}