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

    [Header("인벤토리 중 게임 입력")]
    [Tooltip("인벤토리가 열렸을 때 끌 이동/시점 입력 스크립트")]
    [SerializeField] private MonoBehaviour[] disableWhileOpen;

    [SerializeField] private bool pauseGameWhileOpen;

    [Header("Optional Equip Request")]
    [SerializeField] private GrabbableEvent onEquipRequested;

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
            inventoryData.Changed += HandleInventoryChanged;
            inventoryData.InventoryFull += HandleInventoryFull;
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

        // 다른 플레이어 스크립트가 커서를 다시 잠가도 UI 상태를 유지합니다.
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
            inventoryData.Changed -= HandleInventoryChanged;
            inventoryData.InventoryFull -= HandleInventoryFull;
        }

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
        ExitInventoryControlMode();
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

        onEquipRequested?.Invoke(
            selectedItem.SourceObject
        );
    }

    private void HandleHandObjectChanged(
        Object_Grabbable detectedObject)
    {
        /*
         * 이전 물체를 저장한 뒤 현재 물체를 갱신합니다.
         * HandPivot에서 물체가 빠지면 detectedObject는 null입니다.
         */
        Object_Grabbable previousHeldObject =
            currentHeldObject;

        if (previousHeldObject == detectedObject)
            return;

        currentHeldObject = detectedObject;

        if (inventoryData == null)
        {
            SyncSelectedItem();
            RefreshView();
            return;
        }

        /*
         * 이전 물체가 HandPivot에서 빠졌거나 다른 물체로 교체됐다면
         * 이전 물체의 데이터와 Stored_x 오브젝트를 함께 제거합니다.
         */
        if (previousHeldObject != null &&
            previousHeldObject != detectedObject)
        {
            inventoryData.RemoveBySource(
                previousHeldObject
            );
        }

        /*
         * 현재 손이 비어 있으면 장착 상태를 해제하고 종료합니다.
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

        /*
         * HandPivot에 처음 들어온 물체라면 이름/설명/Mesh 정보를
         * 수집하여 InventoryData에 등록합니다.
         */
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
                SyncSelectedItem();
                RefreshView();
                return;
            }
        }

        // 실제 HandPivot 물체의 슬롯을 선택 + 장착 상태로 맞춥니다.
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