using System;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public sealed class GrabbableEvent :
    UnityEvent<Object_Grabbable>
{
}

[DisallowMultipleComponent]
public sealed class InventoryUIManager : MonoBehaviour
{
    [Header("Core")]
    [SerializeField] private InventoryData inventoryData;
    [SerializeField] private PerceiveObjectHandPivot handPerception;
    [SerializeField] private BringData bringData;
    [SerializeField] private InventoryUIEffect uiEffect;

    [Header("Window")]
    [SerializeField] private bool startClosed = true;

    [Header("Optional Equip Request")]
    [SerializeField] private GrabbableEvent onEquipRequested;

    private Object_Grabbable currentHeldObject;
    private InventoryItemData selectedItem;
    private bool isOpen;

    public bool IsOpen => isOpen;
    public Object_Grabbable CurrentHeldObject => currentHeldObject;
    public InventoryItemData SelectedItem => selectedItem;

    private void Awake()
    {
        isOpen = !startClosed;

        uiEffect?.BindSlots(SelectSlot);
        uiEffect?.SetOpen(isOpen);
    }

    private void OnEnable()
    {
        if (handPerception != null)
            handPerception.HandObjectChanged += HandleHandObjectChanged;

        if (inventoryData != null)
        {
            inventoryData.Changed += HandleInventoryChanged;
            inventoryData.InventoryFull += HandleInventoryFull;
        }
    }

    private void Start()
    {
        handPerception?.ForceScan();
        SyncSelectedItem();
        RefreshView();
    }

    private void OnDisable()
    {
        if (handPerception != null)
            handPerception.HandObjectChanged -= HandleHandObjectChanged;

        if (inventoryData != null)
        {
            inventoryData.Changed -= HandleInventoryChanged;
            inventoryData.InventoryFull -= HandleInventoryFull;
        }
    }

    public void ToggleInventory()
    {
        if (isOpen)
            CloseInventory();
        else
            OpenInventory();
    }

    public void OpenInventory()
    {
        if (isOpen)
            return;

        handPerception?.ForceScan();

        isOpen = true;
        uiEffect?.SetOpen(true);
        RefreshView();
    }

    public void CloseInventory()
    {
        if (!isOpen)
            return;

        isOpen = false;
        uiEffect?.SetOpen(false);
    }

    public void SelectSlot(int slotIndex)
    {
        inventoryData?.Select(slotIndex);
    }

    public void ConfirmSelectedItem()
    {
        SyncSelectedItem();

        if (selectedItem == null)
            return;

        /*
         * Player_Grab을 수정할 수 없는 현재 구조에서는
         * 여기서 EquippedIndex를 임의로 바꾸지 않습니다.
         * 실제 HandPivot의 물체가 바뀌었을 때만
         * HandleHandObjectChanged()가 장착 상태를 갱신합니다.
         */
        onEquipRequested?.Invoke(
            selectedItem.SourceObject
        );
    }

    private void HandleHandObjectChanged(
        Object_Grabbable detectedObject)
    {
        currentHeldObject = detectedObject;

        if (inventoryData == null)
        {
            RefreshView();
            return;
        }

        if (currentHeldObject == null)
        {
            inventoryData.SetEquipped(-1);
            RefreshView();
            return;
        }

        int index =
            inventoryData.FindIndexBySource(
                currentHeldObject
            );

        if (index < 0)
        {
            InventoryItemData newItem =
                bringData != null
                    ? bringData.Capture(
                        currentHeldObject
                    )
                    : null;

            if (!inventoryData.TryAdd(
                    newItem,
                    out index))
            {
                RefreshView();
                return;
            }
        }

        // 새로 손에 들어온 실제 물체를 선택 + 장착 상태로 맞춥니다.
        inventoryData.SetSelectedAndEquipped(index);
    }

    private void HandleInventoryChanged()
    {
        SyncSelectedItem();
        RefreshView();
    }

    private void HandleInventoryFull()
    {
        uiEffect?.PlayInventoryFull();
    }

    private void SyncSelectedItem()
    {
        selectedItem =
            inventoryData != null
                ? inventoryData.SelectedItem
                : null;
    }

    private void RefreshView()
    {
        uiEffect?.Refresh(
            inventoryData,
            selectedItem,
            isOpen
        );
    }
}
