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

    // HandPivot 정리 중 발생하는 재귀 이벤트를 막습니다.
    private bool isSynchronizingHand;

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

        TryForceHandScan();

        SyncSelectedDisplayData();
        RefreshView();
    }

    private void LateUpdate()
    {
        /*
         * 외부 Grab 코드가 HandPivot 아래에 여러 Grabbable을
         * 동시에 넣더라도 프레임 끝에서 1개만 남도록 정리합니다.
         */
        EnforceSingleHandObjectIfNeeded();

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

        TryForceHandScan();

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
        /*
         * ForceScan()으로 인해 같은 이벤트가 다시 들어오는 동안에는
         * 현재 감지값만 갱신하고 중복 정리는 하지 않습니다.
         */
        if (isSynchronizingHand)
        {
            currentHeldObject =
                detectedObject;

            return;
        }

        if (inventoryData == null)
        {
            Debug.LogWarning(
                "[InventoryUIManager] InventoryData가 없습니다.",
                this
            );

            return;
        }

        currentHeldObject =
            detectedObject;

        /*
         * 빈손이면 UI만 갱신합니다.
         */
        if (detectedObject == null)
        {
            SyncSelectedDisplayData();
            RefreshView();
            return;
        }

        isSynchronizingHand = true;

        try
        {
            int index;
            bool newlyAdded;

            if (!TryEnsureInventoryRegistration(
                    detectedObject,
                    out index,
                    out newlyAdded))
            {
                return;
            }

            /*
             * 새로 들어온/감지된 물체가 keepObject입니다.
             * HandPivot 아래의 다른 Grabbable 루트는 모두
             * 비활성 상태로 InventoryData 아래로 보냅니다.
             */
            StoreAllHandObjectsExcept(
                detectedObject
            );

            /*
             * 새로 주운 물건이면 그 슬롯을 Equipped로 맞춥니다.
             * 기존에 Equip 커서가 없던 경우에도 초기화합니다.
             */
            if (newlyAdded ||
                inventoryData.EquippedIndex < 0)
            {
                inventoryData.SetEquipped(
                    index
                );
            }

            TryForceHandScan();

            if (handPerception != null)
            {
                currentHeldObject =
                    handPerception.CurrentObject;
            }
            else
            {
                currentHeldObject =
                    detectedObject;
            }
        }
        catch (Exception exception)
        {
            /*
             * 이 콜백의 예외가 Input System까지 전파되어
             * performed callback 에러가 되는 것을 막습니다.
             */
            Debug.LogWarning(
                "[InventoryUIManager] Hand 동기화 중 예외를 복구했습니다. " +
                exception.GetType().Name + ": " +
                exception.Message,
                this
            );
        }
        finally
        {
            isSynchronizingHand = false;
        }

        SyncSelectedDisplayData();
        RefreshView();

        if (showDebugLog)
        {
            Debug.Log(
                "[InventoryUIManager] Hand 동기화 완료: " +
                $"Current=" +
                $"{(currentHeldObject != null ? currentHeldObject.gameObject.name : "null")}, " +
                $"EquippedIndex={inventoryData.EquippedIndex}",
                this
            );
        }
    }

    private void HandleInventoryChanged()
    {
        /*
         * Hand 이동 중에는 TryAdd/SetEquipped에서 Changed 이벤트가
         * 여러 번 들어올 수 있습니다.
         * 중간 상태의 Preview를 만들지 않고 마지막에 한 번만 갱신합니다.
         */
        if (isSynchronizingHand)
        {
            return;
        }

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
            Debug.LogWarning(
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

        isSynchronizingHand = true;

        try
        {
            /*
             * 빈 슬롯이면 HandPivot의 모든 Grabbable을
             * 비활성 상태로 InventoryData 아래에 보관합니다.
             */
            if (targetObject == null)
            {
                StoreAllHandObjectsExcept(
                    null
                );

                currentHeldObject = null;

                TryForceHandScan();
            }
            else
            {
                /*
                 * target이 아닌 현재 Hand Object를 먼저 보관합니다.
                 */
                StoreAllHandObjectsExcept(
                    targetObject
                );

                /*
                 * target은 비활성 상태로 HandPivot에 이동한 뒤
                 * Transform 정렬이 끝난 마지막에 활성화됩니다.
                 */
                MoveObjectToHandPivot(
                    targetObject,
                    handPivot
                );

                /*
                 * 외부 코드가 같은 프레임에 다른 물체를 넣었더라도
                 * 다시 한 번 target 하나만 남깁니다.
                 */
                StoreAllHandObjectsExcept(
                    targetObject
                );

                currentHeldObject =
                    targetObject;

                TryForceHandScan();

                if (handPerception.CurrentObject != null)
                {
                    currentHeldObject =
                        handPerception.CurrentObject;
                }
            }
        }
        catch (Exception exception)
        {
            Debug.LogWarning(
                "[InventoryUIManager] Equip 변경 중 예외를 복구했습니다. " +
                exception.GetType().Name + ": " +
                exception.Message,
                this
            );
        }
        finally
        {
            isSynchronizingHand = false;
        }

        SyncSelectedDisplayData();
        RefreshView();

        if (showDebugLog)
        {
            Debug.Log(
                "[InventoryEquip] 적용 완료: " +
                $"EquippedIndex={slotIndex}, " +
                $"Hand=" +
                $"{(currentHeldObject != null ? currentHeldObject.gameObject.name : "Empty")}",
                this
            );
        }
    }


    /// <summary>
    /// 월드 Grab 코드의 구현과 관계없이 HandPivot의 활성 Grabbable을
    /// 최대 1개로 유지합니다.
    /// </summary>
    private void EnforceSingleHandObjectIfNeeded()
    {
        if (isSynchronizingHand ||
            inventoryData == null ||
            handPerception == null ||
            handPerception.HandPivot == null)
        {
            return;
        }

        Transform handPivot =
            handPerception.HandPivot;

        Object_Grabbable[] handObjects =
            handPivot.GetComponentsInChildren
                <Object_Grabbable>(false);

        if (handObjects == null ||
            handObjects.Length <= 1)
        {
            return;
        }

        /*
         * Perception이 현재 보고 있는 물체를 우선 유지합니다.
         * 없으면 배열의 마지막 활성 Grabbable을 유지합니다.
         */
        Object_Grabbable keepObject =
            handPerception.CurrentObject;

        if (keepObject == null ||
            !keepObject.transform.IsChildOf(handPivot))
        {
            keepObject =
                handObjects[handObjects.Length - 1];
        }

        isSynchronizingHand = true;

        try
        {
            int keepIndex;
            bool newlyAdded;

            if (TryEnsureInventoryRegistration(
                    keepObject,
                    out keepIndex,
                    out newlyAdded))
            {
                if (newlyAdded ||
                    inventoryData.EquippedIndex < 0)
                {
                    inventoryData.SetEquipped(
                        keepIndex
                    );
                }
            }

            StoreAllHandObjectsExcept(
                keepObject
            );

            currentHeldObject =
                keepObject;

            TryForceHandScan();

            if (handPerception.CurrentObject != null)
            {
                currentHeldObject =
                    handPerception.CurrentObject;
            }
        }
        catch (Exception exception)
        {
            Debug.LogWarning(
                "[InventoryUIManager] HandPivot 단일화 중 예외를 복구했습니다. " +
                exception.GetType().Name + ": " +
                exception.Message,
                this
            );
        }
        finally
        {
            isSynchronizingHand = false;
        }

        SyncSelectedDisplayData();
        RefreshView();
    }


    /// <summary>
    /// keepObject가 포함된 Hand 루트만 남기고
    /// 나머지 Grabbable 루트는 모두 InventoryData 아래에 보관합니다.
    /// keepObject가 null이면 전부 보관합니다.
    /// </summary>
    private void StoreAllHandObjectsExcept(
        Object_Grabbable keepObject)
    {
        if (inventoryData == null ||
            handPerception == null ||
            handPerception.HandPivot == null)
        {
            return;
        }

        Transform handPivot =
            handPerception.HandPivot;

        Object_Grabbable[] handObjects =
            handPivot.GetComponentsInChildren
                <Object_Grabbable>(true);

        if (handObjects == null ||
            handObjects.Length == 0)
        {
            return;
        }

        Transform keepRoot = null;

        if (keepObject != null)
        {
            keepRoot =
                FindDirectChildRoot(
                    handPivot,
                    keepObject.transform
                );
        }

        for (int i = 0;
             i < handObjects.Length;
             i++)
        {
            Object_Grabbable handObject =
                handObjects[i];

            if (handObject == null)
            {
                continue;
            }

            Transform objectRoot =
                FindDirectChildRoot(
                    handPivot,
                    handObject.transform
                );

            /*
             * 이미 HandPivot 밖으로 이동한 Object는 무시합니다.
             */
            if (objectRoot == null)
            {
                continue;
            }

            /*
             * keepObject와 같은 실제 Hand 루트에 포함된 component라면
             * 그 루트는 그대로 유지합니다.
             */
            if (keepRoot != null &&
                objectRoot == keepRoot)
            {
                continue;
            }

            int index;
            bool newlyAdded;

            if (!TryEnsureInventoryRegistration(
                    handObject,
                    out index,
                    out newlyAdded))
            {
                continue;
            }

            MoveObjectToInventoryStorage(
                handObject
            );
        }
    }


    /// <summary>
    /// InventoryData에 Object가 없으면 등록합니다.
    /// </summary>
    private bool TryEnsureInventoryRegistration(
        Object_Grabbable sourceObject,
        out int index,
        out bool newlyAdded)
    {
        index = -1;
        newlyAdded = false;

        if (inventoryData == null ||
            sourceObject == null)
        {
            return false;
        }

        index =
            inventoryData.FindIndexBySource(
                sourceObject
            );

        if (index >= 0)
        {
            return true;
        }

        if (!inventoryData.TryAdd(
                sourceObject,
                out index))
        {
            return false;
        }

        newlyAdded = true;
        return true;
    }


    /// <summary>
    /// parent 아래에 descendant가 있을 때
    /// parent의 바로 아래에 해당하는 루트를 반환합니다.
    /// </summary>
    private static Transform FindDirectChildRoot(
        Transform parent,
        Transform descendant)
    {
        if (parent == null ||
            descendant == null ||
            descendant == parent ||
            !descendant.IsChildOf(parent))
        {
            return null;
        }

        Transform current =
            descendant;

        while (current.parent != null &&
               current.parent != parent)
        {
            current =
                current.parent;
        }

        return current.parent == parent
            ? current
            : null;
    }


    /// <summary>
    /// ForceScan에서 외부 component 예외가 발생하더라도
    /// Inventory 입력 콜백 밖으로 전파시키지 않습니다.
    /// </summary>
    private void TryForceHandScan()
    {
        if (handPerception == null)
        {
            return;
        }

        try
        {
            handPerception.ForceScan();
        }
        catch (Exception exception)
        {
            Debug.LogWarning(
                "[InventoryUIManager] ForceScan 예외를 복구했습니다. " +
                exception.GetType().Name + ": " +
                exception.Message,
                this
            );
        }
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

        /*
         * Object_Grabbable이 손 오브젝트의 자식에 붙어 있는 경우에도
         * 실제 HandPivot 바로 아래 루트 전체를 이동해야
         * Mesh/Collider가 남지 않습니다.
         */
        Transform handPivot =
            handPerception != null
                ? handPerception.HandPivot
                : null;

        Transform sourceTransform =
            sourceObject.transform;

        Transform storageRoot =
            FindDirectChildRoot(
                handPivot,
                sourceTransform
            );

        if (storageRoot == null)
        {
            storageRoot =
                FindDirectChildRoot(
                    inventoryData.transform,
                    sourceTransform
                );
        }

        if (storageRoot == null)
        {
            storageRoot =
                sourceTransform;
        }

        GameObject storageObject =
            storageRoot.gameObject;

        /*
         * 중요:
         * Parent/Position을 바꾸기 전에 먼저 비활성화합니다.
         * MeshRenderer, Collider, Behaviour가 보이지 않는 상태에서
         * InventoryData 아래로 이동합니다.
         */
        if (storageObject.activeSelf)
        {
            storageObject.SetActive(
                false
            );
        }

        if (storageRoot.parent !=
            inventoryData.transform)
        {
            storageRoot.SetParent(
                inventoryData.transform,
                true
            );
        }

        if (showDebugLog)
        {
            Debug.Log(
                "[InventoryUIManager] Object 보관 완료: " +
                $"Object={sourceObject.gameObject.name}, " +
                $"Root={storageObject.name}, " +
                $"Active={storageObject.activeSelf}, " +
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
         * InventoryData 아래에 저장된 루트 전체를 찾아 이동합니다.
         * Object_Grabbable이 nested component여도 Mesh 루트가 남지 않습니다.
         */
        Transform targetTransform =
            targetObject.transform;

        Transform handRoot =
            FindDirectChildRoot(
                inventoryData != null
                    ? inventoryData.transform
                    : null,
                targetTransform
            );

        if (handRoot == null)
        {
            handRoot =
                FindDirectChildRoot(
                    handPivot,
                    targetTransform
                );
        }

        if (handRoot == null)
        {
            handRoot =
                targetTransform;
        }

        GameObject handObject =
            handRoot.gameObject;

        bool alreadyInHand =
            handRoot.parent == handPivot;

        /*
         * 이동 과정에서 Mesh가 순간적으로 보이지 않도록
         * 먼저 비활성화합니다.
         */
        if (!alreadyInHand &&
            handObject.activeSelf)
        {
            handObject.SetActive(
                false
            );
        }

        if (!alreadyInHand)
        {
            handRoot.SetParent(
                handPivot,
                true
            );

            handRoot.position =
                handPivot.position;

            handRoot.rotation =
                handPivot.rotation;
        }

        /*
         * 모든 Transform 변경이 끝난 마지막에 활성화합니다.
         */
        if (!handObject.activeSelf)
        {
            handObject.SetActive(
                true
            );
        }

        if (showDebugLog)
        {
            Debug.Log(
                "[InventoryUIManager] Object 장착 완료: " +
                $"Object={targetObject.gameObject.name}, " +
                $"Root={handObject.name}, " +
                $"Parent={handPivot.gameObject.name}",
                targetObject
            );
        }
    }

}