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
    [Tooltip("인벤토리가 열렸을 때 이동/시점 입력 스크립트")]
    [SerializeField]
    private MonoBehaviour[] disableWhileOpen;

    [SerializeField]
    private bool pauseGameWhileOpen;

    [Header("Optional Equip Request")]
    [SerializeField]
    private GrabbableEvent onEquipRequested;

    [Header("Debug")]
    [SerializeField]
    private bool showDebugLog = true;

    private Object_Grabbable currentHeldObject;
    private InventoryDisplayData selectedDisplayData;
    private bool isOpen;

    private CursorLockMode previousCursorLock;
    private bool previousCursorVisible;
    private bool[] previousBehaviourStates;
    private float previousTimeScale = 1f;
    private bool inventoryControlModeApplied;

    public bool IsOpen => isOpen;

    public Object_Grabbable CurrentHeldObject =>
        currentHeldObject;

    public InventoryDisplayData SelectedDisplayData =>
        selectedDisplayData;

    private void Awake()
    {
        isOpen = !startClosed;

        if (uiEffect != null)
        {
            uiEffect.BindSlots(
                SelectSlot
            );

            uiEffect.SetOpen(
                isOpen
            );
        }
    }

    private void OnEnable()
    {
        if (inventoryData != null)
        {
            inventoryData.Changed +=
                HandleInventoryChanged;

            inventoryData.InventoryFull +=
                HandleInventoryFull;
        }

        if (handPerception != null)
        {
            handPerception.HandObjectChanged +=
                HandleHandObjectChanged;
        }
    }

    private void Start()
    {
        if (isOpen)
        {
            EnterInventoryControlMode();
        }

        if (handPerception != null)
        {
            handPerception.ForceScan();
        }

        SyncSelectedDisplayData();
        RefreshView();
    }

    private void LateUpdate()
    {
        if (!isOpen)
        {
            return;
        }

        Cursor.lockState =
            CursorLockMode.None;

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

        if (uiEffect != null)
        {
            uiEffect.SetOpen(false);
        }

        ExitInventoryControlMode();
    }

    public void ToggleInventory()
    {
        if (isOpen)
        {
            CloseInventory();
        }
        else
        {
            OpenInventory();
        }
    }

    public void OpenInventory()
    {
        if (isOpen)
        {
            return;
        }

        isOpen = true;



        EnterInventoryControlMode();

        if (uiEffect != null)
        {
            uiEffect.SetOpen(true);
        }

        if (handPerception != null)
        {
            handPerception.ForceScan();
        }

        SyncSelectedDisplayData();
        RefreshView();
    }

    public void CloseInventory()
    {
        if (!isOpen)
        {
            return;
        }

        isOpen = false;



        if (uiEffect != null)
        {
            uiEffect.SetOpen(false);
        }

        SyncSelectedDisplayData();
        RefreshView();

        ExitInventoryControlMode();
    }

    public void SelectSlot(
        int slotIndex)
    {
        if (inventoryData == null)
        {
            Debug.LogError(
                "[InventoryUIManager] InventoryData가 없습니다.",
                this
            );

            return;
        }

        if (showDebugLog)
        {
            Debug.Log(
                $"[InventoryUIManager] 슬롯 선택 요청: {slotIndex}",
                this
            );
        }

        inventoryData.Select(
            slotIndex
        );

        SyncSelectedDisplayData();
        RefreshView();

        if (showDebugLog)
        {
            string selectedName =
                selectedDisplayData != null
                    ? selectedDisplayData.ItemName
                    : "null";

            Debug.Log(
                $"[InventoryUIManager] 선택 결과: " +
                $"SelectedIndex={inventoryData.SelectedIndex}, " +
                $"EquippedIndex={inventoryData.EquippedIndex}, " +
                $"SelectedItem={selectedName}",
                this
            );
        }
    }

    public void ConfirmSelectedItem()
    {
        if (inventoryData == null)
        {
            return;
        }

        Object_Grabbable selectedObject =
            inventoryData.SelectedObject;

        if (selectedObject == null)
        {
            return;
        }

        onEquipRequested?.Invoke(
            selectedObject
        );
    }

    private void HandleHandObjectChanged(
        Object_Grabbable detectedObject)
    {
        if (inventoryData == null)
        {
            Debug.LogError(
                "[InventoryUIManager] InventoryData가 없습니다.",
                this
            );

            return;
        }

        /*
         * 실제 HandPivot 상태만 기록합니다.
         *
         * EquippedIndex는 이제 HandPivot 자체가 아니라
         * "장착 슬롯 커서"가 소유합니다.
         *
         * 따라서 HandPivot에서 기존 물건이 계속 감지되더라도
         * 스크롤로 옮긴 EquippedIndex를 이전 슬롯으로 되돌리면 안 됩니다.
         */
        currentHeldObject =
            detectedObject;

        if (showDebugLog)
        {
            Debug.Log(
                "[InventoryUIManager] HandObjectChanged: " +
                $"Source=" +
                $"{(detectedObject != null ? detectedObject.gameObject.name : "null")}, " +
                $"ObjectName=" +
                $"{(detectedObject != null ? ResolveObjectName(detectedObject) : "null")}, " +
                $"EquippedIndex={inventoryData.EquippedIndex}",
                this
            );
        }

        /*
         * 빈손:
         * EquippedIndex는 그대로 유지합니다.
         */
        if (currentHeldObject == null)
        {
            RefreshView();
            return;
        }

        /*
         * 손에 들어온 실제 물건이 Inventory에 있는지 찾습니다.
         */
        int index =
            inventoryData.FindIndexBySource(
                currentHeldObject
            );

        bool newlyAdded = false;

        /*
         * 월드에서 처음 집은 물건이라면
         * Inventory에 새로 등록합니다.
         */
        if (index < 0)
        {
            if (!inventoryData.TryAdd(
                    currentHeldObject,
                    out index))
            {
                RefreshView();
                return;
            }

            newlyAdded = true;
        }

        /*
         * EquippedIndex를 Hand 감지 때문에 무조건 덮어쓰지 않습니다.
         *
         * 새로 획득한 물건:
         *   -> 그 물건이 들어간 슬롯을 Equip
         *
         * 아직 Equip 커서가 없는 상태(-1):
         *   -> 현재 손 물건 슬롯으로 초기화
         *
         * 그 외:
         *   -> Scroll이 정한 EquippedIndex를 그대로 유지
         */
        if (newlyAdded ||
            inventoryData.EquippedIndex < 0)
        {
            inventoryData.SetEquipped(
                index
            );
        }

        RefreshView();

        if (showDebugLog)
        {
            if (inventoryData.EquippedIndex == index)
            {
                Debug.Log(
                    "[InventoryUIManager] Hand/Equip 일치: " +
                    $"HandIndex={index}, " +
                    $"EquippedIndex={inventoryData.EquippedIndex}, " +
                    $"Source={currentHeldObject.gameObject.name}",
                    currentHeldObject
                );
            }
            else
            {
                Debug.Log(
                    "[InventoryUIManager] Equip 커서 유지: " +
                    $"HandIndex={index}, " +
                    $"EquippedIndex={inventoryData.EquippedIndex}, " +
                    $"Hand={currentHeldObject.gameObject.name}",
                    this
                );
            }
        }
    }

    private void HandleInventoryChanged()
    {
        SyncSelectedDisplayData();
        RefreshView();
    }

    private void HandleInventoryFull()
    {
        if (uiEffect != null)
        {
            uiEffect.PlayInventoryFull();
        }
    }

    private void SyncSelectedDisplayData()
    {
        if (inventoryData == null ||
            bringData == null)
        {
            selectedDisplayData = null;
            return;
        }

        selectedDisplayData =
            bringData.BuildDisplayData(
                inventoryData,
                inventoryData.SelectedIndex
            );
    }

    private void RefreshView()
    {
        if (uiEffect == null)
        {
            return;
        }

        uiEffect.Refresh(
            inventoryData,
            selectedDisplayData,
            isOpen
        );
    }

    private static string ResolveObjectName(
        Object_Grabbable sourceObject)
    {
        if (sourceObject == null)
        {
            return "Unknown";
        }

        if (!string.IsNullOrWhiteSpace(
                sourceObject.objectName))
        {
            return sourceObject.objectName;
        }

        return sourceObject.gameObject.name;
    }

    private void EnterInventoryControlMode()
    {
        if (inventoryControlModeApplied)
        {
            return;
        }

        inventoryControlModeApplied = true;

        previousCursorLock =
            Cursor.lockState;

        previousCursorVisible =
            Cursor.visible;

        Cursor.lockState =
            CursorLockMode.None;

        Cursor.visible = true;

        if (pauseGameWhileOpen)
        {
            previousTimeScale =
                Time.timeScale;

            Time.timeScale = 0f;
        }

        if (disableWhileOpen == null)
        {
            return;
        }

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
        {
            return;
        }

        inventoryControlModeApplied = false;

        if (pauseGameWhileOpen)
        {
            Time.timeScale =
                previousTimeScale;
        }

        if (disableWhileOpen != null &&
            previousBehaviourStates != null)
        {
            int count =
                Mathf.Min(
                    disableWhileOpen.Length,
                    previousBehaviourStates.Length
                );

            for (int i = 0;
                 i < count;
                 i++)
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

        Cursor.lockState =
            previousCursorLock;

        Cursor.visible =
            previousCursorVisible;
    }

    public void CycleSelectedItem(
        int direction)
    {
        if (inventoryData == null)
        {
            Debug.LogError(
                "[InventoryEquipScroll] InventoryData가 없습니다.",
                this
            );

            return;
        }

        if (direction == 0)
        {
            return;
        }

        int slotCount =
            inventoryData.SlotCount;

        if (slotCount <= 0)
        {
            return;
        }

        int normalizedDirection =
            direction < 0
                ? -1
                : 1;

        int currentIndex =
            inventoryData.EquippedIndex;

        int nextIndex;

        /*
         * Equip cursor가 아직 없는 경우:
         * 아래 -> 0
         * 위   -> 마지막 슬롯
         */
        if (currentIndex < 0 ||
            currentIndex >= slotCount)
        {
            nextIndex =
                normalizedDirection > 0
                    ? 0
                    : slotCount - 1;
        }
        else
        {
            nextIndex =
                currentIndex +
                normalizedDirection;

            if (nextIndex < 0)
            {
                nextIndex =
                    slotCount - 1;
            }

            if (nextIndex >= slotCount)
            {
                nextIndex = 0;
            }
        }

        Debug.Log(
            "[InventoryEquipScroll] 스크롤 처리: " +
            $"Direction={normalizedDirection}, " +
            $"Before={currentIndex}, " +
            $"Next={nextIndex}",
            this
        );

        /*
         * 1. Equip cursor / EquippedFrame 먼저 이동
         *
         * 빈 슬롯도 EquippedIndex로 허용됩니다.
         */
        inventoryData.SetEquipped(
            nextIndex
        );

        RefreshView();

        /*
         * 2. EquippedIndex가 가리키는 슬롯 상태에 맞춰
         * 실제 HandPivot의 Object도 변경합니다.
         */
        ApplyEquippedSlotToHand(
            nextIndex
        );
    }


    /// <summary>
    /// EquippedIndex가 가리키는 슬롯 상태를
    /// 실제 HandPivot 상태에 적용합니다.
    ///
    /// 빈 슬롯:
    ///     현재 Hand Object -> InventoryData 아래로 이동
    ///     HandPivot -> 빈손
    ///
    /// Object 존재:
    ///     기존 Hand Object -> InventoryData 아래로 이동
    ///     target Object -> HandPivot 아래로 이동
    /// </summary>
    private void ApplyEquippedSlotToHand(
        int slotIndex)
    {
        if (inventoryData == null)
        {
            return;
        }

        if (handPerception == null ||
            handPerception.HandPivot == null)
        {
            Debug.LogError(
                "[InventoryUIManager] " +
                "PerceiveObjectHandPivot 또는 HandPivot이 없습니다.",
                this
            );

            return;
        }

        Transform handPivot =
            handPerception.HandPivot;

        Object_Grabbable targetObject =
            inventoryData.GetObjectAt(
                slotIndex
            );

        /*
         * 현재 HandPivot에 실제로 있는 물체를 다시 확인합니다.
         *
         * currentHeldObject가 오래된 참조일 수 있으므로
         * HandPivot 자식도 같이 확인합니다.
         */
        Object_Grabbable heldObject =
            FindObjectUnderHandPivot(
                handPivot
            );

        if (heldObject != null)
        {
            currentHeldObject =
                heldObject;
        }


        /*
         * =====================================================
         * A. 빈 슬롯
         * =====================================================
         */
        if (targetObject == null)
        {
            if (heldObject != null)
            {
                MoveObjectToInventoryStorage(
                    heldObject
                );
            }

            currentHeldObject = null;

            if (showDebugLog)
            {
                Debug.Log(
                    "[InventoryEquipScroll] 빈 슬롯 적용 완료: " +
                    $"EquippedIndex={slotIndex}, " +
                    "Hand=Empty",
                    this
                );
            }

            /*
             * 이미 실제로 HandPivot에서 빼낸 뒤이므로
             * 이제 ForceScan해도 이전 물체가 다시 감지되지 않습니다.
             */
            handPerception.ForceScan();

            RefreshView();
            return;
        }


        /*
         * 이미 target이 손에 있다면 이동할 필요가 없습니다.
         */
        if (heldObject == targetObject &&
            targetObject.transform.IsChildOf(handPivot))
        {
            currentHeldObject =
                targetObject;

            if (!targetObject.gameObject.activeSelf)
            {
                targetObject.gameObject.SetActive(true);
            }

            handPerception.ForceScan();
            RefreshView();
            return;
        }


        /*
         * =====================================================
         * B. 다른 Object가 있는 슬롯
         * =====================================================
         *
         * 기존에 들고 있던 물체가 있으면 먼저 보관합니다.
         */
        if (heldObject != null &&
            heldObject != targetObject)
        {
            MoveObjectToInventoryStorage(
                heldObject
            );
        }


        /*
         * target을 실제 HandPivot 아래로 이동합니다.
         */
        MoveObjectToHandPivot(
            targetObject,
            handPivot
        );

        currentHeldObject =
            targetObject;

        if (showDebugLog)
        {
            Debug.Log(
                "[InventoryEquipScroll] 실제 Hand 교체 완료: " +
                $"EquippedIndex={slotIndex}, " +
                $"Hand={targetObject.gameObject.name}",
                targetObject
            );
        }

        /*
         * 물리적인 Parent 변경이 끝난 뒤 감지기를 동기화합니다.
         */
        handPerception.ForceScan();

        RefreshView();
    }


    /// <summary>
    /// HandPivot 바로 아래 또는 하위 계층에서
    /// Object_Grabbable 하나를 찾습니다.
    /// </summary>
    private static Object_Grabbable FindObjectUnderHandPivot(
        Transform handPivot)
    {
        if (handPivot == null)
        {
            return null;
        }

        for (int i = handPivot.childCount - 1;
             i >= 0;
             i--)
        {
            Transform child =
                handPivot.GetChild(i);

            if (child == null)
            {
                continue;
            }

            Object_Grabbable direct =
                child.GetComponent<Object_Grabbable>();

            if (direct != null)
            {
                return direct;
            }

            Object_Grabbable nested =
                child.GetComponentInChildren<Object_Grabbable>(
                    true
                );

            if (nested != null)
            {
                return nested;
            }
        }

        return null;
    }


    /// <summary>
    /// 현재 손에 있던 물체를 InventoryData GameObject 아래로 옮겨
    /// 실제 HandPivot을 비웁니다.
    ///
    /// SourceObject 참조는 InventoryData 슬롯에 그대로 남습니다.
    /// 비활성화된 GameObject도 Unity 참조 자체는 유지됩니다.
    /// </summary>
    private void MoveObjectToInventoryStorage(
        Object_Grabbable sourceObject)
    {
        if (sourceObject == null ||
            inventoryData == null)
        {
            return;
        }

        Transform sourceTransform =
            sourceObject.transform;

        /*
         * InventoryData 아래로 Parent 이동.
         * worldPositionStays=true로 월드 Transform이 갑자기 깨지는 것을 막습니다.
         */
        sourceTransform.SetParent(
            inventoryData.transform,
            true
        );

        /*
         * 보관 중에는 Scene에 보이거나 충돌하지 않도록 비활성화합니다.
         *
         * InventoryData.GetObjectAt()은 비활성 GameObject 참조도
         * 정상적인 SourceObject로 유지할 수 있습니다.
         */
        sourceObject.gameObject.SetActive(
            false
        );

        if (showDebugLog)
        {
            Debug.Log(
                "[InventoryUIManager] Hand Object 보관: " +
                $"Object={sourceObject.gameObject.name}, " +
                $"Parent={inventoryData.gameObject.name}",
                this
            );
        }
    }


    /// <summary>
    /// InventoryData 아래에 보관된 Object를
    /// 실제 HandPivot으로 꺼내 장착합니다.
    /// </summary>
    private void MoveObjectToHandPivot(
        Object_Grabbable targetObject,
        Transform handPivot)
    {
        if (targetObject == null ||
            handPivot == null)
        {
            return;
        }

        /*
         * 먼저 활성화합니다.
         */
        if (!targetObject.gameObject.activeSelf)
        {
            targetObject.gameObject.SetActive(
                true
            );
        }

        Transform targetTransform =
            targetObject.transform;

        /*
         * worldPositionStays=true를 사용한 뒤
         * HandPivot 위치/회전에 정확히 맞춥니다.
         *
         * 이렇게 하면 기존 Object의 world scale을 가능한 한 보존하면서
         * 손 위치로 이동할 수 있습니다.
         */
        targetTransform.SetParent(
            handPivot,
            true
        );

        targetTransform.position =
            handPivot.position;

        targetTransform.rotation =
            handPivot.rotation;

        if (showDebugLog)
        {
            Debug.Log(
                "[InventoryUIManager] Inventory Object 장착: " +
                $"Object={targetObject.gameObject.name}, " +
                $"Parent={handPivot.gameObject.name}",
                targetObject
            );
        }
    }

}