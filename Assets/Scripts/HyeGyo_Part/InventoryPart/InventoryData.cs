using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 아이템을 구성하는 Mesh 하나의 정보입니다.
/// 여러 자식 Mesh로 구성된 아이템도 표시할 수 있습니다.
/// </summary>
[Serializable]
public sealed class InventoryMeshPartData
{
    [SerializeField] private Mesh mesh;
    [SerializeField] private Material[] materials;

    [SerializeField] private Vector3 localPosition;
    [SerializeField] private Quaternion localRotation;
    [SerializeField] private Vector3 localScale;

    public Mesh Mesh => mesh;
    public Material[] Materials => materials;

    public Vector3 LocalPosition => localPosition;
    public Quaternion LocalRotation => localRotation;
    public Vector3 LocalScale => localScale;

    public InventoryMeshPartData(
        Mesh mesh,
        Material[] materials,
        Vector3 localPosition,
        Quaternion localRotation,
        Vector3 localScale)
    {
        this.mesh = mesh;

        this.materials = materials != null
            ? (Material[])materials.Clone()
            : Array.Empty<Material>();

        this.localPosition = localPosition;
        this.localRotation = localRotation;
        this.localScale = localScale;
    }
}

/// <summary>
/// 인벤토리에 저장되는 아이템 하나의 데이터입니다.
/// Object_Grabbable에서 이름, 설명, Mesh 정보를 가져옵니다.
/// </summary>
[Serializable]
public sealed class InventoryItemData
{
    [Header("원본 오브젝트")]
    [SerializeField] private Object_Grabbable sourceObject;

    [Header("UI 정보")]
    [SerializeField] private string itemName;

    [TextArea(3, 7)]
    [SerializeField] private string description;

    [Header("HandPivot에 들어왔을 때 Transform")]
    [SerializeField] private Vector3 heldLocalPosition;
    [SerializeField] private Quaternion heldLocalRotation;
    [SerializeField] private Vector3 heldLocalScale;

    [Header("3D Mesh 정보")]
    [SerializeField]
    private List<InventoryMeshPartData> meshParts =
        new List<InventoryMeshPartData>();

    public Object_Grabbable SourceObject => sourceObject;

    public GameObject GameObject =>
        sourceObject != null
            ? sourceObject.gameObject
            : null;

    public string ItemName => itemName;
    public string Description => description;

    public Vector3 HeldLocalPosition => heldLocalPosition;
    public Quaternion HeldLocalRotation => heldLocalRotation;
    public Vector3 HeldLocalScale => heldLocalScale;

    public IReadOnlyList<InventoryMeshPartData> MeshParts =>
        meshParts;

    public InventoryItemData(Object_Grabbable source)
    {
        sourceObject = source;

        if (source == null)
            return;

        itemName = string.IsNullOrWhiteSpace(source.objectName)
            ? source.gameObject.name
            : source.objectName;

        description = source.description;

        heldLocalPosition = source.transform.localPosition;
        heldLocalRotation = source.transform.localRotation;
        heldLocalScale = source.transform.localScale;

        CaptureMeshData(source);
    }

    private void CaptureMeshData(Object_Grabbable source)
    {
        meshParts.Clear();

        Transform itemRoot = source.transform;

        MeshFilter[] filters =
            source.GetComponentsInChildren<MeshFilter>(true);

        foreach (MeshFilter filter in filters)
        {
            if (filter.sharedMesh == null)
                continue;

            MeshRenderer renderer =
                filter.GetComponent<MeshRenderer>();

            if (renderer == null)
                continue;

            /*
             * 자식 Mesh가 원본 아이템 기준으로 어디에 있는지 저장합니다.
             * 자식이 여러 단계로 중첩되어 있어도 상대 Transform을 계산합니다.
             */
            Matrix4x4 relativeMatrix =
                itemRoot.worldToLocalMatrix *
                filter.transform.localToWorldMatrix;

            Vector3 relativePosition = new Vector3(
                relativeMatrix.m03,
                relativeMatrix.m13,
                relativeMatrix.m23
            );

            Quaternion relativeRotation =
                relativeMatrix.rotation;

            Vector3 relativeScale =
                relativeMatrix.lossyScale;

            InventoryMeshPartData part =
                new InventoryMeshPartData(
                    filter.sharedMesh,
                    renderer.sharedMaterials,
                    relativePosition,
                    relativeRotation,
                    relativeScale
                );

            meshParts.Add(part);
        }
    }
}

/// <summary>
/// HandPivot을 검사하고 인벤토리 데이터를 관리합니다.
/// 이 컴포넌트는 항상 활성화된 InventorySystem 오브젝트에 붙입니다.
/// </summary>
public class InventoryData : MonoBehaviour
{
    [Header("HandPivot")]
    [SerializeField] private Transform handPivot;

    [Header("인벤토리 설정")]
    [SerializeField, Min(1)] private int maxSlotCount = 8;

    [Header("저장된 아이템")]
    [SerializeField]
    private List<InventoryItemData> items =
        new List<InventoryItemData>();

    [Header("현재 상태")]
    [SerializeField] private int selectedIndex = 0;
    [SerializeField] private int equippedIndex = -1;

    private Object_Grabbable lastDetectedHeldObject;

    public int MaxSlotCount => maxSlotCount;

    public IReadOnlyList<InventoryItemData> Items =>
        items;

    public int SelectedIndex => selectedIndex;
    public int EquippedIndex => equippedIndex;

    public InventoryItemData SelectedItem =>
        GetItemAt(selectedIndex);

    public InventoryItemData EquippedItem =>
        GetItemAt(equippedIndex);

    /// <summary>
    /// 아이템 추가, 선택 변경, 장착 변경 시 발생합니다.
    /// </summary>
    public event Action OnChanged;

    /// <summary>
    /// 8칸이 가득 찬 상태에서 새 아이템이 감지되면 발생합니다.
    /// </summary>
    public event Action OnInventoryFull;

    private void Awake()
    {
        maxSlotCount = Mathf.Max(1, maxSlotCount);

        if (items.Count > maxSlotCount)
        {
            items.RemoveRange(
                maxSlotCount,
                items.Count - maxSlotCount
            );
        }
    }

    private void LateUpdate()
    {
        DetectHeldObject();
    }

    /// <summary>
    /// HandPivot의 활성 자식 중 Object_Grabbable을 찾습니다.
    /// </summary>
    private Object_Grabbable FindHeldObject()
    {
        if (handPivot == null)
            return null;

        /*
         * 새 아이템이 마지막 자식으로 들어오는 구조를 고려해
         * 뒤쪽 자식부터 검사합니다.
         */
        for (int i = handPivot.childCount - 1; i >= 0; i--)
        {
            Transform child = handPivot.GetChild(i);

            if (!child.gameObject.activeInHierarchy)
                continue;

            Object_Grabbable grabbable =
                child.GetComponent<Object_Grabbable>();

            if (grabbable == null)
            {
                grabbable =
                    child.GetComponentInChildren
                    <Object_Grabbable>(true);
            }

            if (grabbable != null)
                return grabbable;
        }

        return null;
    }

    private void DetectHeldObject()
    {
        Object_Grabbable currentHeldObject =
            FindHeldObject();

        // 손에 든 오브젝트가 바뀌지 않았다면 재처리하지 않습니다.
        if (currentHeldObject == lastDetectedHeldObject)
            return;

        lastDetectedHeldObject = currentHeldObject;

        // HandPivot이 비어 있으면 현재 장착 인덱스를 해제합니다.
        if (currentHeldObject == null)
        {
            if (equippedIndex != -1)
            {
                equippedIndex = -1;
                OnChanged?.Invoke();
            }

            return;
        }

        int index = FindIndex(currentHeldObject);

        // 처음 집은 오브젝트라면 인벤토리에 등록합니다.
        if (index < 0)
        {
            bool added = TryAddInternal(
                currentHeldObject,
                out index
            );

            if (!added)
            {
                OnInventoryFull?.Invoke();
                return;
            }
        }

        equippedIndex = index;

        // 처음 획득하거나 다시 손에 든 아이템을 선택 상태로 만듭니다.
        selectedIndex = index;

        OnChanged?.Invoke();
    }

    private bool TryAddInternal(
        Object_Grabbable grabbable,
        out int addedIndex)
    {
        addedIndex = -1;

        if (grabbable == null)
            return false;

        int existingIndex = FindIndex(grabbable);

        if (existingIndex >= 0)
        {
            addedIndex = existingIndex;
            return true;
        }

        if (items.Count >= maxSlotCount)
            return false;

        InventoryItemData newItem =
            new InventoryItemData(grabbable);

        items.Add(newItem);
        addedIndex = items.Count - 1;

        return true;
    }

    /// <summary>
    /// 다른 코드에서 직접 아이템을 등록할 때 사용할 수 있습니다.
    /// </summary>
    public bool TryAdd(Object_Grabbable grabbable)
    {
        bool added = TryAddInternal(
            grabbable,
            out int addedIndex
        );

        if (!added)
        {
            OnInventoryFull?.Invoke();
            return false;
        }

        selectedIndex = addedIndex;
        OnChanged?.Invoke();

        return true;
    }

    public InventoryItemData GetItemAt(int index)
    {
        if (index < 0 || index >= items.Count)
            return null;

        return items[index];
    }

    /// <summary>
    /// 빈 슬롯을 포함해 0~7번 슬롯을 선택할 수 있습니다.
    /// </summary>
    public void SelectSlot(int index)
    {
        if (index < 0 || index >= maxSlotCount)
            return;

        if (selectedIndex == index)
            return;

        selectedIndex = index;
        OnChanged?.Invoke();
    }

    public void ForceDetectFromHandPivot()
    {
        lastDetectedHeldObject = null;
        DetectHeldObject();
    }

    private int FindIndex(Object_Grabbable target)
    {
        if (target == null)
            return -1;

        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].SourceObject == target)
                return i;
        }

        return -1;
    }
}