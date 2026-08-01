using System;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public sealed class InventoryData : MonoBehaviour
{
    [Header("Inventory")]
    [SerializeField, Min(1)]
    private int capacity = 8;

    [SerializeField]
    private List<InventoryItemData> slots =
        new List<InventoryItemData>();

    [Header("Runtime State")]
    [SerializeField]
    private int selectedIndex = -1;

    [SerializeField]
    private int equippedIndex = -1;

    [Header("Debug")]
    [SerializeField]
    private bool showDebugLog = true;

    /// <summary>
    /// 슬롯 내용, 선택 인덱스 또는 장착 인덱스가 변경될 때 발생합니다.
    /// </summary>
    public event Action Changed;

    /// <summary>
    /// 빈 슬롯이 없어서 아이템 추가에 실패했을 때 발생합니다.
    /// </summary>
    public event Action InventoryFull;

    public int SlotCount => capacity;

    public int SelectedIndex => selectedIndex;

    public int EquippedIndex => equippedIndex;

    /// <summary>
    /// 현재 선택된 슬롯의 아이템입니다.
    /// 빈 슬롯을 선택했다면 null입니다.
    /// </summary>
    public InventoryItemData SelectedItem =>
        GetItemAt(selectedIndex);

    /// <summary>
    /// 현재 실제로 장착된 슬롯의 아이템입니다.
    /// </summary>
    public InventoryItemData EquippedItem =>
        GetItemAt(equippedIndex);

    private void Awake()
    {
        EnsureSlotCount();
        ValidateIndices();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        capacity = Mathf.Max(1, capacity);
        EnsureSlotCount();
        ValidateIndices();
    }
#endif

    /// <summary>
    /// 특정 슬롯의 아이템을 가져옵니다.
    /// 범위를 벗어나거나 빈 슬롯이면 null입니다.
    /// </summary>
    public InventoryItemData GetItemAt(int slotIndex)
    {
        EnsureSlotCount();

        if (!IsValidIndex(slotIndex))
            return null;

        return slots[slotIndex];
    }

    public bool TryGetItemAt(
        int slotIndex,
        out InventoryItemData item)
    {
        item = GetItemAt(slotIndex);
        return item != null;
    }

    /// <summary>
    /// 새 아이템을 첫 번째 빈 슬롯에 등록합니다.
    /// 성공 시 true와 등록된 슬롯 번호를 반환합니다.
    ///
    /// 동일한 SourceObject가 이미 등록돼 있다면
    /// 중복 생성하지 않고 기존 슬롯 번호를 반환합니다.
    /// </summary>
    public bool TryAdd(
        InventoryItemData item,
        out int addedIndex)
    {
        addedIndex = -1;

        EnsureSlotCount();

        if (item == null)
        {
            Debug.LogWarning(
                "[InventoryData] 추가할 InventoryItemData가 없습니다.",
                this
            );

            return false;
        }

        /*
         * HandPivot 재검사로 같은 실제 물체가 반복 감지돼도
         * 인벤토리에 중복으로 추가되지 않게 합니다.
         */
        if (item.SourceObject != null)
        {
            int existingIndex =
                FindIndexBySource(item.SourceObject);

            if (existingIndex >= 0)
            {
                addedIndex = existingIndex;

                if (showDebugLog)
                {
                    Debug.Log(
                        $"[InventoryData] '{item.ItemName}'은 이미 " +
                        $"Slot {existingIndex + 1}에 등록돼 있습니다.",
                        this
                    );
                }

                return true;
            }
        }

        int emptyIndex = FindFirstEmptyIndex();

        if (emptyIndex < 0)
        {
            Debug.LogWarning(
                $"[InventoryData] 인벤토리가 가득 차서 " +
                $"'{item.ItemName}'을 추가하지 못했습니다.",
                this
            );

            InventoryFull?.Invoke();
            return false;
        }

        slots[emptyIndex] = item;
        addedIndex = emptyIndex;

        if (showDebugLog)
        {
            Debug.Log(
                $"[InventoryData] '{item.ItemName}'을 " +
                $"Slot {emptyIndex + 1}에 등록했습니다.",
                this
            );
        }

        NotifyChanged();
        return true;
    }

    /// <summary>
    /// 실제 Object_Grabbable을 기준으로 슬롯 번호를 찾습니다.
    /// 찾지 못하면 -1을 반환합니다.
    /// </summary>
    public int FindIndexBySource(
        Object_Grabbable sourceObject)
    {
        if (sourceObject == null)
            return -1;

        EnsureSlotCount();

        for (int i = 0; i < slots.Count; i++)
        {
            InventoryItemData item = slots[i];

            if (item == null)
                continue;

            if (item.SourceObject == sourceObject)
                return i;
        }

        return -1;
    }

    /// <summary>
    /// UI에서 클릭한 슬롯을 선택합니다.
    ///
    /// 빈 슬롯이어도 selectedIndex는 해당 슬롯으로 이동합니다.
    /// 따라서 빈 슬롯에도 SelectedFrame이 표시될 수 있습니다.
    /// </summary>
    public void Select(int slotIndex)
    {
        EnsureSlotCount();

        if (!IsValidIndex(slotIndex))
        {
            Debug.LogWarning(
                $"[InventoryData] 잘못된 선택 슬롯 번호: {slotIndex}",
                this
            );

            return;
        }

        if (selectedIndex == slotIndex)
        {
            if (showDebugLog)
            {
                Debug.Log(
                    $"[InventoryData] Slot {slotIndex + 1}은 " +
                    "이미 선택돼 있습니다.",
                    this
                );
            }

            return;
        }

        selectedIndex = slotIndex;

        if (showDebugLog)
        {
            InventoryItemData item =
                GetItemAt(selectedIndex);

            Debug.Log(
                $"[InventoryData] SelectedIndex={selectedIndex}, " +
                $"Item={(item != null ? item.ItemName : "빈 슬롯")}",
                this
            );
        }

        NotifyChanged();
    }

    /// <summary>
    /// 실제 HandPivot 아래에 있는 아이템의 슬롯을 장착 상태로 설정합니다.
    /// -1을 전달하면 장착을 해제합니다.
    /// </summary>
    public void SetEquipped(int slotIndex)
    {
        EnsureSlotCount();

        if (slotIndex == -1)
        {
            if (equippedIndex == -1)
                return;

            equippedIndex = -1;

            if (showDebugLog)
            {
                Debug.Log(
                    "[InventoryData] 장착 상태를 해제했습니다.",
                    this
                );
            }

            NotifyChanged();
            return;
        }

        if (!IsValidIndex(slotIndex))
        {
            Debug.LogWarning(
                $"[InventoryData] 잘못된 장착 슬롯 번호: {slotIndex}",
                this
            );

            return;
        }

        /*
         * 장착 슬롯은 실제 아이템이 있어야 합니다.
         */
        if (slots[slotIndex] == null)
        {
            Debug.LogWarning(
                $"[InventoryData] 비어 있는 Slot {slotIndex + 1}은 " +
                "장착할 수 없습니다.",
                this
            );

            return;
        }

        if (equippedIndex == slotIndex)
            return;

        equippedIndex = slotIndex;

        if (showDebugLog)
        {
            Debug.Log(
                $"[InventoryData] EquippedIndex={equippedIndex}",
                this
            );
        }

        NotifyChanged();
    }

    /// <summary>
    /// 새 물건을 획득했을 때 선택과 장착을 동시에 맞춥니다.
    /// Changed 이벤트는 한 번만 발생합니다.
    /// </summary>
    public void SetSelectedAndEquipped(
        int slotIndex)
    {
        EnsureSlotCount();

        if (!IsValidIndex(slotIndex))
        {
            Debug.LogWarning(
                $"[InventoryData] 잘못된 슬롯 번호: {slotIndex}",
                this
            );

            return;
        }

        if (slots[slotIndex] == null)
        {
            Debug.LogWarning(
                $"[InventoryData] Slot {slotIndex + 1}에 " +
                "장착할 아이템이 없습니다.",
                this
            );

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
                $"EquippedIndex={equippedIndex}",
                this
            );
        }

        if (changed)
            NotifyChanged();
    }

    /// <summary>
    /// 슬롯 번호를 기준으로 아이템을 제거합니다.
    /// 뒤의 슬롯을 앞으로 당기지 않습니다.
    /// </summary>
    public bool RemoveAt(int slotIndex)
    {
        EnsureSlotCount();

        if (!IsValidIndex(slotIndex))
            return false;

        InventoryItemData removedItem =
            slots[slotIndex];

        if (removedItem == null)
            return false;

        slots[slotIndex] = null;

        /*
         * 빈 슬롯도 선택 가능하므로 selectedIndex는 유지합니다.
         * 제거된 슬롯은 선택된 빈 슬롯 상태가 됩니다.
         */
        if (equippedIndex == slotIndex)
            equippedIndex = -1;

        if (showDebugLog)
        {
            Debug.Log(
                $"[InventoryData] '{removedItem.ItemName}'을 " +
                $"Slot {slotIndex + 1}에서 제거했습니다.",
                this
            );
        }

        NotifyChanged();
        return true;
    }

    /// <summary>
    /// 실제 Object_Grabbable 참조를 기준으로 아이템을 제거합니다.
    /// </summary>
    public bool RemoveBySource(
        Object_Grabbable sourceObject)
    {
        int index =
            FindIndexBySource(sourceObject);

        if (index < 0)
            return false;

        return RemoveAt(index);
    }

    /// <summary>
    /// 모든 인벤토리 데이터를 초기화합니다.
    /// </summary>
    [ContextMenu("Clear Inventory")]
    public void Clear()
    {
        EnsureSlotCount();

        for (int i = 0; i < slots.Count; i++)
            slots[i] = null;

        selectedIndex = -1;
        equippedIndex = -1;

        if (showDebugLog)
        {
            Debug.Log(
                "[InventoryData] 인벤토리를 초기화했습니다.",
                this
            );
        }

        NotifyChanged();
    }

    private int FindFirstEmptyIndex()
    {
        EnsureSlotCount();

        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i] == null)
                return i;
        }

        return -1;
    }

    private bool IsValidIndex(int slotIndex)
    {
        return slotIndex >= 0 &&
               slotIndex < slots.Count;
    }

    private void EnsureSlotCount()
    {
        capacity = Mathf.Max(1, capacity);

        if (slots == null)
        {
            slots =
                new List<InventoryItemData>();
        }

        while (slots.Count < capacity)
            slots.Add(null);

        /*
         * Capacity를 줄였을 때 뒤쪽 슬롯을 제거합니다.
         */
        while (slots.Count > capacity)
            slots.RemoveAt(slots.Count - 1);
    }

    private void ValidateIndices()
    {
        /*
         * 빈 슬롯인지 여부는 검사하지 않습니다.
         * 빈 슬롯도 선택할 수 있기 때문입니다.
         */
        if (selectedIndex < -1 ||
            selectedIndex >= capacity)
        {
            selectedIndex = -1;
        }

        if (equippedIndex < -1 ||
            equippedIndex >= capacity)
        {
            equippedIndex = -1;
        }

        /*
         * 장착 슬롯에는 실제 아이템이 있어야 합니다.
         */
        if (equippedIndex >= 0 &&
            GetItemAt(equippedIndex) == null)
        {
            equippedIndex = -1;
        }
    }

    private void NotifyChanged()
    {
        Changed?.Invoke();
    }
}