using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[Serializable]
public sealed class InventorySlotView
{
    [SerializeField] private Button button;
    [SerializeField] private TMP_Text itemNameText;
    [SerializeField] private GameObject selectedFrame;
    [SerializeField] private GameObject equippedFrame;

    private UnityAction clickAction;

    public void Bind(
        int slotIndex,
        Action<int> onClicked)
    {
        if (button == null)
            return;

        if (clickAction != null)
            button.onClick.RemoveListener(clickAction);

        clickAction =
            () => onClicked?.Invoke(slotIndex);

        button.onClick.AddListener(clickAction);
    }

    public void Refresh(
        string itemName,
        bool selected,
        bool equipped)
    {
        if (itemNameText != null)
        {
            itemNameText.text =
                string.IsNullOrWhiteSpace(itemName)
                    ? "—"
                    : itemName;
        }

        if (selectedFrame != null)
            selectedFrame.SetActive(selected);

        if (equippedFrame != null)
            equippedFrame.SetActive(equipped);
    }
}
