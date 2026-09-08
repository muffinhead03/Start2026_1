using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// 슬롯 하나의 버튼, 이름 텍스트,
/// 선택 프레임과 장착 프레임을 묶어서 관리합니다.
/// </summary>
[Serializable]
public sealed class InventorySlotView
{
    [SerializeField]
    private Button button;

    [SerializeField]
    private TMP_Text itemNameText;

    [SerializeField]
    private GameObject selectedFrame;

    [SerializeField]
    private GameObject equippedFrame;

    private UnityAction clickAction;
    private int boundSlotIndex = -1;

    public void Bind(
        int slotIndex,
        Action<int> onClicked)
    {
        Unbind();

        if (button == null)
        {
            Debug.LogError(
                $"[InventorySlotView] Index {slotIndex}의 Button이 연결되지 않았습니다."
            );

            return;
        }

        if (onClicked == null)
        {
            Debug.LogError(
                $"[InventorySlotView] Index {slotIndex}의 클릭 콜백이 없습니다.",
                button
            );

            return;
        }

        boundSlotIndex = slotIndex;

        clickAction = () =>
        {
            onClicked.Invoke(
                boundSlotIndex
            );
        };

        button.onClick.AddListener(
            clickAction
        );

        button.interactable = true;
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
        {
            selectedFrame.SetActive(
                selected
            );
        }

        if (equippedFrame != null)
        {
            equippedFrame.SetActive(
                equipped
            );
        }
    }
}

[DisallowMultipleComponent]
public sealed class InventoryUIEffect : MonoBehaviour
{
    [Header("Canvas")]
    [SerializeField]
    private GameObject inventoryRoot;

    [Header("Slots")]
    [Tooltip("Index 0부터 Index 7까지 순서대로 연결합니다.")]
    [SerializeField]
    private InventorySlotView[] slots =
        new InventorySlotView[8];

    [Header("Selected Item")]
    [SerializeField]
    private TMP_Text selectedNameText;

    [SerializeField]
    private TMP_Text selectedDescriptionText;

    [Header("Object Preview")]
    [Tooltip(
        "PreviewRig에 붙어 있는 InventoryObjectPreview를 연결합니다."
    )]
    [SerializeField]
    private InventoryObjectPreview objectPreview;

    [Header("Preview Debug")]
    [SerializeField]
    private bool showDebugLog;

    private void OnDestroy()
    {
        UnbindSlots();
        HidePreview();
    }

    /// <summary>
    /// 각 슬롯 버튼을 InventoryUIManager.SelectSlot에 연결합니다.
    /// </summary>
    public void BindSlots(
        Action<int> onSlotClicked)
    {
        if (slots == null ||
            slots.Length == 0)
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

        UnbindSlots();

        for (int i = 0;
             i < slots.Length;
             i++)
        {
            InventorySlotView slot =
                slots[i];

            if (slot == null)
            {
                Debug.LogError(
                    $"[InventoryUIEffect] Slots Element {i}가 비어 있습니다.",
                    this
                );

                continue;
            }

            slot.Bind(
                i,
                onSlotClicked
            );
        }
    }

    /// <summary>
    /// 인벤토리 창을 열거나 닫습니다.
    /// </summary>
    public void SetOpen(bool open)
    {
        if (inventoryRoot != null)
        {
            inventoryRoot.SetActive(
                open
            );
        }

        if (!open)
        {
            HidePreview();
        }
    }

    /// <summary>
    /// 슬롯, 우측 정보, Object Preview를 갱신합니다.
    /// </summary>
    public void Refresh(
        InventoryData inventoryData,
        InventoryDisplayData selectedDisplayData,
        bool open)
    {
        RefreshSlots(
            inventoryData
        );

        RefreshSelectedInformation(
            selectedDisplayData,
            open
        );
    }

    public void PlayInventoryFull()
    {
        Debug.Log(
            "[InventoryUIEffect] 인벤토리가 가득 찼습니다.",
            this
        );
    }

    /// <summary>
    /// 8개 슬롯의 이름과 선택/장착 프레임을 갱신합니다.
    /// </summary>
    private void RefreshSlots(
        InventoryData inventoryData)
    {
        if (slots == null)
        {
            return;
        }

        for (int i = 0;
             i < slots.Length;
             i++)
        {
            InventorySlotView slot =
                slots[i];

            if (slot == null)
            {
                continue;
            }

            string itemName =
                inventoryData != null
                    ? inventoryData.GetObjectNameAt(i)
                    : "—";

            bool selected =
                inventoryData != null &&
                inventoryData.SelectedIndex == i;

            bool equipped =
                inventoryData != null &&
                inventoryData.EquippedIndex == i;

            slot.Refresh(
                itemName,
                selected,
                equipped
            );
        }
    }

    /// <summary>
    /// 우측 이름/설명과 3D Object Preview를 갱신합니다.
    /// 기존 MeshPart 재구성 방식은 사용하지 않습니다.
    /// </summary>
    private void RefreshSelectedInformation(
        InventoryDisplayData selectedDisplayData,
        bool open)
    {
        if (selectedNameText != null)
        {
            selectedNameText.text =
                selectedDisplayData != null
                    ? selectedDisplayData.ItemName
                    : "—";
        }

        if (selectedDescriptionText != null)
        {
            selectedDescriptionText.text =
                selectedDisplayData != null
                    ? selectedDisplayData.Description
                    : string.Empty;
        }

        if (!open ||
            selectedDisplayData == null ||
            selectedDisplayData.SourceObject == null)
        {
            HidePreview();
            return;
        }

        if (objectPreview == null)
        {
            Debug.LogError(
                "[InventoryUIEffect] InventoryObjectPreview가 연결되지 않았습니다. " +
                "PreviewRig를 Object Preview 필드에 연결하세요.",
                this
            );

            return;
        }

        try
        {
            objectPreview.Show(
                selectedDisplayData.SourceObject
            );

            if (showDebugLog)
            {
                Debug.Log(
                    "[InventoryUIEffect] Object Preview 갱신: " +
                    $"Name={selectedDisplayData.ItemName}, " +
                    $"Source={selectedDisplayData.SourceObject.gameObject.name}",
                    this
                );
            }
        }
        catch (Exception exception)
        {
            Debug.LogWarning(
                "[InventoryUIEffect] Object Preview 갱신 중 예외를 복구했습니다. " +
                exception.GetType().Name + ": " +
                exception.Message,
                this
            );

            HidePreview();
        }
    }

    /// <summary>
    /// 현재 Object Preview를 제거합니다.
    /// 인벤토리 닫기, 빈 슬롯 선택, 선택 해제 등에 사용합니다.
    /// </summary>
    private void HidePreview()
    {
        if (objectPreview == null)
        {
            return;
        }

        try
        {
            objectPreview.Clear();
        }
        catch (Exception exception)
        {
            Debug.LogWarning(
                "[InventoryUIEffect] Object Preview 정리 중 예외를 복구했습니다. " +
                exception.GetType().Name + ": " +
                exception.Message,
                this
            );
        }
    }

    private void UnbindSlots()
    {
        if (slots == null)
        {
            return;
        }

        for (int i = 0;
             i < slots.Length;
             i++)
        {
            slots[i]?.Unbind();
        }
    }
}
