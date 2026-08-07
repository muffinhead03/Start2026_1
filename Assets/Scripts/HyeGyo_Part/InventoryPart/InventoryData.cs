using System;
using UnityEngine;

[DisallowMultipleComponent]
public sealed class InventoryData : MonoBehaviour
{
    private const int Capacity = 8;

    [Serializable]
    private sealed class SlotEntry
    {
        [SerializeField]
        private string objectName;

        [SerializeField]
        private Object_Grabbable sourceObject;

        public string ObjectName => objectName;

        public Object_Grabbable SourceObject =>
            sourceObject;

        public SlotEntry(
            string objectName,
            Object_Grabbable sourceObject)
        {
            this.objectName =
                string.IsNullOrWhiteSpace(objectName)
                    ? sourceObject != null
                        ? sourceObject.gameObject.name
                        : "Unknown"
                    : objectName;

            this.sourceObject = sourceObject;
        }
    }

    [Header("Slots")]
    [Tooltip("Index 0부터 Index 7까지 총 8개 슬롯입니다.")]
    [SerializeField]
    private SlotEntry[] slots =
        new SlotEntry[Capacity];

    [Header("Runtime State")]
    [SerializeField]
    private int selectedIndex = -1;

    [SerializeField]
    private int equippedIndex = -1;

    [Header("Debug")]
    [SerializeField]
    private bool showDebugLog = true;

    public event Action Changed;
    public event Action InventoryFull;

    public int SlotCount => Capacity;
    public int SelectedIndex => selectedIndex;
    public int EquippedIndex => equippedIndex;

    public Object_Grabbable SelectedObject =>
        GetObjectAt(selectedIndex);

    public Object_Grabbable EquippedObject =>
        GetObjectAt(equippedIndex);

    private void Awake()
    {
        EnsureSlots();
        ValidateIndices();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        EnsureSlots();
        ValidateIndices();
    }
#endif

    public Object_Grabbable GetObjectAt(
        int slotIndex)
    {
        EnsureSlots();

        if (!IsValidIndex(slotIndex))
        {
            return null;
        }

        SlotEntry slot =
            slots[slotIndex];

        if (slot == null)
        {
            return null;
        }

        /*
         * 원본 오브젝트가 실제로 파괴됐다면
         * 해당 슬롯도 자동으로 비웁니다.
         *
         * 단순 비활성화된 오브젝트는 null이 아니므로
         * 인벤토리에 그대로 유지됩니다.
         */
        if (slot.SourceObject == null)
        {
            /*
             * 원본 오브젝트가 파괴된 경우 슬롯 데이터만 비웁니다.
             *
             * EquippedIndex는 "장착 슬롯 커서"이므로
             * 해당 슬롯이 비어도 그대로 유지합니다.
             */
            slots[slotIndex] = null;

            return null;
        }

        return slot.SourceObject;
    }

    public string GetObjectNameAt(
        int slotIndex)
    {
        EnsureSlots();

        if (!IsValidIndex(slotIndex))
        {
            return "—";
        }

        SlotEntry slot =
            slots[slotIndex];

        if (slot == null ||
            slot.SourceObject == null)
        {
            return "—";
        }

        return string.IsNullOrWhiteSpace(
                slot.ObjectName)
            ? ResolveObjectName(slot.SourceObject)
            : slot.ObjectName;
    }

    /*
     * 이름이 아니라 실제 Object_Grabbable 참조로 찾습니다.
     *
     * 따라서 objectName이 같은 서로 다른 물체는
     * 서로 다른 슬롯에 저장할 수 있습니다.
     */
    public int FindIndexBySource(
        Object_Grabbable sourceObject)
    {
        if (sourceObject == null)
        {
            return -1;
        }

        EnsureSlots();

        for (int i = 0;
             i < Capacity;
             i++)
        {
            Object_Grabbable storedObject =
                GetObjectAt(i);

            if (storedObject == sourceObject)
            {
                return i;
            }
        }

        return -1;
    }

    /*
     * index 0부터 검사해 첫 번째 빈 슬롯에 저장합니다.
     *
     * 같은 objectName은 허용합니다.
     * 같은 실제 SourceObject의 중복 등록만 막습니다.
     */
    public bool TryAdd(
        Object_Grabbable sourceObject,
        out int addedIndex)
    {
        addedIndex = -1;

        if (sourceObject == null)
        {
            Debug.LogWarning(
                "[InventoryData] 추가할 Object_Grabbable이 없습니다.",
                this
            );

            return false;
        }

        EnsureSlots();

        int existingIndex =
            FindIndexBySource(
                sourceObject
            );

        if (existingIndex >= 0)
        {
            addedIndex = existingIndex;

            if (showDebugLog)
            {
                Debug.Log(
                    $"[InventoryData] 같은 실제 오브젝트가 이미 있습니다. " +
                    $"Index={existingIndex}, " +
                    $"ObjectName={ResolveObjectName(sourceObject)}, " +
                    $"Source={sourceObject.gameObject.name}",
                    sourceObject
                );
            }

            return true;
        }

        string objectName =
            ResolveObjectName(
                sourceObject
            );

        for (int i = 0;
             i < Capacity;
             i++)
        {
            if (GetObjectAt(i) != null)
            {
                continue;
            }

            slots[i] =
                new SlotEntry(
                    objectName,
                    sourceObject
                );

            addedIndex = i;

            if (showDebugLog)
            {
                Debug.Log(
                    $"[InventoryData] 아이템 추가 완료: " +
                    $"Index={i}, " +
                    $"ObjectName={objectName}, " +
                    $"Source={sourceObject.gameObject.name}",
                    sourceObject
                );
            }

            NotifyChanged();
            return true;
        }

        Debug.LogWarning(
            "[InventoryData] 인벤토리가 가득 찼습니다.",
            this
        );

        InventoryFull?.Invoke();
        return false;
    }

    public void Select(
        int slotIndex)
    {
        if (!IsValidIndex(slotIndex))
        {
            return;
        }

        if (selectedIndex == slotIndex)
        {
            return;
        }

        selectedIndex = slotIndex;

        if (showDebugLog)
        {
            Debug.Log(
                $"[InventoryData] 슬롯 선택: " +
                $"SelectedIndex={selectedIndex}, " +
                $"ObjectName={GetObjectNameAt(selectedIndex)}",
                this
            );
        }

        NotifyChanged();
    }

    public void SetEquipped(
        int slotIndex)
    {
        /*
         * -1:
         * 장착 슬롯 커서가 없는 상태
         *
         * 0 ~ 7:
         * 유효한 장착 슬롯 커서
         *
         * 중요:
         * 슬롯에 실제 Object가 없어도 EquippedIndex로 허용합니다.
         */
        if (slotIndex < -1 ||
            slotIndex >= Capacity)
        {
            if (showDebugLog)
            {
                Debug.LogWarning(
                    "[InventoryData] 잘못된 장착 슬롯 index: " +
                    $"Requested={slotIndex}, " +
                    $"Capacity={Capacity}",
                    this
                );
            }

            return;
        }

        if (equippedIndex == slotIndex)
        {
            return;
        }

        equippedIndex =
            slotIndex;

        if (showDebugLog)
        {
            string objectName =
                equippedIndex >= 0
                    ? GetObjectNameAt(equippedIndex)
                    : "—";

            Debug.Log(
                "[InventoryData] 장착 슬롯 변경: " +
                $"EquippedIndex={equippedIndex}, " +
                $"ObjectName={objectName}",
                this
            );
        }

        NotifyChanged();
    }

    public void SetSelectedAndEquipped(
        int slotIndex)
    {
        if (!IsValidIndex(slotIndex) ||
            GetObjectAt(slotIndex) == null)
        {
            return;
        }

        bool changed =
            selectedIndex != slotIndex ||
            equippedIndex != slotIndex;

        selectedIndex = slotIndex;
        equippedIndex = slotIndex;

        if (showDebugLog)
        {
            Debug.Log(
                $"[InventoryData] 선택 및 장착 완료: " +
                $"SelectedIndex={selectedIndex}, " +
                $"EquippedIndex={equippedIndex}, " +
                $"ObjectName={GetObjectNameAt(slotIndex)}",
                this
            );
        }

        if (changed)
        {
            NotifyChanged();
        }
    }

    public bool RemoveAt(
        int slotIndex)
    {
        EnsureSlots();

        if (!IsValidIndex(slotIndex) ||
            slots[slotIndex] == null)
        {
            return false;
        }

        string removedName =
            GetObjectNameAt(slotIndex);

        slots[slotIndex] = null;

        /*
         * selectedIndex와 equippedIndex는
         * 빈 슬롯을 가리킬 수 있으므로 그대로 유지합니다.
         *
         * 예:
         * EquippedIndex = 3
         * Slot 3의 아이템 삭제
         * -> EquippedIndex는 3 유지
         * -> EquippedFrame도 Slot 3 유지
         * -> 실제 Hand는 빈손 처리
         */
        if (showDebugLog)
        {
            Debug.Log(
                $"[InventoryData] 아이템 삭제 완료: " +
                $"Index={slotIndex}, " +
                $"ObjectName={removedName}",
                this
            );
        }

        NotifyChanged();
        return true;
    }

    public bool RemoveBySource(
        Object_Grabbable sourceObject)
    {
        int index =
            FindIndexBySource(
                sourceObject
            );

        if (index < 0)
        {
            return false;
        }

        return RemoveAt(index);
    }

    [ContextMenu("Clear Inventory")]
    public void Clear()
    {
        EnsureSlots();

        for (int i = 0;
             i < Capacity;
             i++)
        {
            slots[i] = null;
        }

        selectedIndex = -1;
        equippedIndex = -1;

        NotifyChanged();
    }

    [ContextMenu("Print Slot State")]
    private void PrintSlotState()
    {
        EnsureSlots();

        for (int i = 0;
             i < Capacity;
             i++)
        {
            Object_Grabbable source =
                GetObjectAt(i);

            Debug.Log(
                $"[InventoryData] " +
                $"Index={i}, " +
                $"ObjectName={GetObjectNameAt(i)}, " +
                $"Source={(source != null ? source.gameObject.name : "null")}",
                this
            );
        }
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

    private bool IsValidIndex(
        int index)
    {
        return index >= 0 &&
               index < Capacity;
    }

    private void EnsureSlots()
    {
        if (slots != null &&
            slots.Length == Capacity)
        {
            return;
        }

        SlotEntry[] newSlots =
            new SlotEntry[Capacity];

        if (slots != null)
        {
            int count =
                Mathf.Min(
                    slots.Length,
                    Capacity
                );

            for (int i = 0;
                 i < count;
                 i++)
            {
                newSlots[i] = slots[i];
            }
        }

        slots = newSlots;
    }

    private void ValidateIndices()
    {
        if (selectedIndex < -1 ||
            selectedIndex >= Capacity)
        {
            selectedIndex = -1;
        }

        if (equippedIndex < -1 ||
            equippedIndex >= Capacity)
        {
            equippedIndex = -1;
        }

        /*
         * EquippedIndex는 빈 슬롯도 허용합니다.
         *
         * 따라서 GetObjectAt(equippedIndex)가 null이어도
         * equippedIndex를 -1로 되돌리지 않습니다.
         */
    }

    private void NotifyChanged()
    {
        Changed?.Invoke();
    }


}