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
    [SerializeField]
    private InventoryData inventoryData;

    [SerializeField]
    private PerceiveObjectHandPivot handPerception;

    [SerializeField]
    private BringData bringData;

    [SerializeField]
    private InventoryUIEffect uiEffect;

    [Header("Window")]
    [SerializeField]
    private bool startClosed = true;

    [Header("인벤토리 중 게임 입력")]
    [Tooltip("인벤토리가 열렸을 때 끌 이동/시점 입력 스크립트")]
    [SerializeField]
    private MonoBehaviour[] disableWhileOpen;

    [SerializeField]
    private bool pauseGameWhileOpen;

    [Header("Optional Equip Request")]
    [SerializeField]
    private GrabbableEvent onEquipRequested;

    private Object_Grabbable currentHeldObject;
    private InventoryItemData selectedItem;
    private bool isOpen;

    private CursorLockMode previousCursorLock;
    private bool previousCursorVisible;
    private bool[] previousBehaviourStates;
    private float previousTimeScale = 1f;
    private bool inventoryControlModeApplied;

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
        {
            handPerception.HandObjectChanged +=
                HandleHandObjectChanged;
        }

        if (inventoryData != null)
        {
            inventoryData.Changed +=
                HandleInventoryChanged;

            inventoryData.InventoryFull +=
                HandleInventoryFull;
        }
    }

    private void Start()
    {
        handPerception?.ForceScan();

        if (isOpen)
            EnterInventoryControlMode();

        SyncSelectedItem();
        RefreshView();
    }

    private void LateUpdate()
    {
        if (!isOpen)
            return;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void OnDisable()
    {
        if (handPerception != null)
        {
            handPerception.HandObjectChanged -=
                HandleHandObjectChanged;
        }

        if (inventoryData != null)
        {
            inventoryData.Changed -=
                HandleInventoryChanged;

            inventoryData.InventoryFull -=
                HandleInventoryFull;
        }

        uiEffect?.SetOpen(false);

        ExitInventoryControlMode();
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

        /*
         * 인벤토리를 열 때 현재 HandPivot 상태를 다시 검사합니다.
         */
        handPerception?.ForceScan();

        isOpen = true;

        EnterInventoryControlMode();

        uiEffect?.SetOpen(true);

        SyncSelectedItem();
        RefreshView();
    }

    public void CloseInventory()
    {
        if (!isOpen)
            return;

        isOpen = false;

        uiEffect?.SetOpen(false);

        SyncSelectedItem();
        RefreshView();

        ExitInventoryControlMode();
    }

    public void SelectSlot(int slotIndex)
    {
        Debug.Log(
            $"[InventoryUIManager] 슬롯 선택 요청: {slotIndex}",
            this
        );

        if (inventoryData == null)
        {
            Debug.LogError(
                "[InventoryUIManager] InventoryData가 없습니다.",
                this
            );

            return;
        }

        inventoryData.Select(slotIndex);

        SyncSelectedItem();
        RefreshView();

        Debug.Log(
            $"[InventoryUIManager] 선택 결과: " +
            $"SelectedIndex={inventoryData.SelectedIndex}, " +
            $"EquippedIndex={inventoryData.EquippedIndex}, " +
            $"SelectedItem=" +
            $"{(selectedItem != null ? selectedItem.ItemName : "null")}",
            this
        );
    }

    public void ConfirmSelectedItem()
    {
        SyncSelectedItem();

        if (selectedItem == null)
            return;

        onEquipRequested?.Invoke(
            selectedItem.SourceObject
        );
    }

    private void HandleHandObjectChanged(
        Object_Grabbable detectedObject)
    {
        Debug.Log(
            $"[InventoryUIManager] HandObjectChanged: " +
            $"{(detectedObject != null ? detectedObject.name : "null")}",
            this
        );

        if (currentHeldObject == detectedObject)
        {
            Debug.Log(
                "[InventoryUIManager] 이전 감지 물체와 같습니다.",
                this
            );

            return;
        }

        currentHeldObject = detectedObject;

        if (inventoryData == null)
        {
            Debug.LogError(
                "[InventoryUIManager] InventoryData가 없습니다.",
                this
            );

            return;
        }

        /*
         * 손이 비었다고 인벤토리 아이템을 삭제하지 않습니다.
         * 장착 표시만 해제합니다.
         */
        if (currentHeldObject == null)
        {
            inventoryData.SetEquipped(-1);

            SyncSelectedItem();
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
                    ? bringData.Capture(currentHeldObject)
                    : null;

            if (newItem == null)
            {
                Debug.LogWarning(
                    "[InventoryUIManager] 아이템 데이터 수집 실패",
                    currentHeldObject
                );

                return;
            }

            if (!inventoryData.TryAdd(
                    newItem,
                    out index))
            {
                Debug.LogWarning(
                    $"[InventoryUIManager] " +
                    $"'{newItem.ItemName}' 등록 실패",
                    currentHeldObject
                );

                SyncSelectedItem();
                RefreshView();
                return;
            }

            Debug.Log(
                $"[InventoryUIManager] " +
                $"'{newItem.ItemName}'을 " +
                $"Slot {index + 1}에 등록했습니다.",
                currentHeldObject
            );
        }

        inventoryData.SetSelectedAndEquipped(index);

        SyncSelectedItem();
        RefreshView();
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

    private void EnterInventoryControlMode()
    {
        if (inventoryControlModeApplied)
            return;

        inventoryControlModeApplied = true;

        previousCursorLock = Cursor.lockState;
        previousCursorVisible = Cursor.visible;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (pauseGameWhileOpen)
        {
            previousTimeScale = Time.timeScale;
            Time.timeScale = 0f;
        }

        if (disableWhileOpen == null)
            return;

        previousBehaviourStates =
            new bool[disableWhileOpen.Length];

        for (int i = 0;
             i < disableWhileOpen.Length;
             i++)
        {
            MonoBehaviour behaviour =
                disableWhileOpen[i];

            if (behaviour == null ||
                behaviour == this)
            {
                continue;
            }

            previousBehaviourStates[i] =
                behaviour.enabled;

            behaviour.enabled = false;
        }
    }

    private void ExitInventoryControlMode()
    {
        if (!inventoryControlModeApplied)
            return;

        inventoryControlModeApplied = false;

        if (pauseGameWhileOpen)
            Time.timeScale = previousTimeScale;

        if (disableWhileOpen != null &&
            previousBehaviourStates != null)
        {
            int count = Mathf.Min(
                disableWhileOpen.Length,
                previousBehaviourStates.Length
            );

            for (int i = 0; i < count; i++)
            {
                MonoBehaviour behaviour =
                    disableWhileOpen[i];

                if (behaviour == null ||
                    behaviour == this)
                {
                    continue;
                }

                behaviour.enabled =
                    previousBehaviourStates[i];
            }
        }

        previousBehaviourStates = null;

        Cursor.lockState = previousCursorLock;
        Cursor.visible = previousCursorVisible;
    }
}