using System;
using TMPro;
using UnityEngine;

[DisallowMultipleComponent]
public sealed class InventoryUIEffect : MonoBehaviour
{
    [Header("Canvas")]
    [SerializeField] private GameObject inventoryRoot;

    [Header("Slots")]
    [SerializeField] private InventorySlotView[] slots;

    [Header("Selected Item")]
    [SerializeField] private TMP_Text selectedNameText;
    [SerializeField] private TMP_Text selectedDescriptionText;

    [Header("Preview")]
    [SerializeField] private InventoryPreviewView previewView;

    public void BindSlots(Action<int> onSlotClicked)
    {
        if (slots == null)
            return;

        for (int i = 0; i < slots.Length; i++)
            slots[i]?.Bind(i, onSlotClicked);
    }

    public void SetOpen(bool open)
    {
        if (inventoryRoot != null)
            inventoryRoot.SetActive(open);

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
        Debug.Log("[InventoryUIEffect] 인벤토리가 가득 찼습니다.", this);
    }

    private void RefreshSlots(InventoryData data)
    {
        if (slots == null)
            return;

        for (int i = 0; i < slots.Length; i++)
        {
            InventoryItemData item =
                data != null
                    ? data.GetItemAt(i)
                    : null;

            slots[i]?.Refresh(
                item != null ? item.ItemName : "—",
                data != null &&
                    i == data.SelectedIndex,
                data != null &&
                    i == data.EquippedIndex
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
