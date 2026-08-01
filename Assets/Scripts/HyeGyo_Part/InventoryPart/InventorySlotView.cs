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
    private int boundSlotIndex = -1;

    public bool IsConfigured => button != null;

    public void Bind(
        int slotIndex,
        Action<int> onClicked)
    {
        if (button == null)
        {
            Debug.LogError(
                $"[InventorySlotView] " +
                $"슬롯 {slotIndex}의 Button이 연결되지 않았습니다."
            );

            return;
        }

        if (onClicked == null)
        {
            Debug.LogError(
                $"[InventorySlotView] " +
                $"슬롯 {slotIndex}의 클릭 콜백이 없습니다.",
                button
            );

            return;
        }

        Unbind();

        boundSlotIndex = slotIndex;

        clickAction = () =>
        {
            Debug.Log(
                $"[InventorySlotView] " +
                $"슬롯 클릭: {boundSlotIndex}",
                button
            );

            onClicked.Invoke(boundSlotIndex);
        };

        button.onClick.AddListener(clickAction);
        button.interactable = true;

        Debug.Log(
            $"[InventorySlotView] " +
            $"슬롯 바인딩 완료: {slotIndex}, " +
            $"Button={button.name}",
            button
        );
    }

    public void Unbind()
    {
        if (button != null &&
            clickAction != null)
        {
            button.onClick.RemoveListener(
                clickAction
            );
        }

        clickAction = null;
        boundSlotIndex = -1;
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