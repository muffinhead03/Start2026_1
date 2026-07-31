using System;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public sealed class InventoryData : MonoBehaviour
{
    [SerializeField, Min(1)] private int capacity = 8;
    [SerializeField] private List<InventoryItemData> items = new();

    [SerializeField] private int selectedIndex = -1;
    [SerializeField] private int equippedIndex = -1;

    public event Action Changed;
    public event Action InventoryFull;

    public int Capacity => capacity;
    public int Count => items.Count;
    public int SelectedIndex => selectedIndex;
    public int EquippedIndex => equippedIndex;

    public InventoryItemData SelectedItem =>
        GetItemAt(selectedIndex);

    public InventoryItemData EquippedItem =>
        GetItemAt(equippedIndex);

    public InventoryItemData GetItemAt(int index)
    {
        if (index < 0 || index >= items.Count)
            return null;

        return items[index];
    }

    public int FindIndexBySource(Object_Grabbable source)
    {
        if (source == null)
            return -1;

        for (int i = 0; i < items.Count; i++)
        {
            if (items[i] != null &&
                items[i].SourceObject == source)
            {
                return i;
            }
        }

        return -1;
    }

    public bool TryAdd(
        InventoryItemData item,
        out int index)
    {
        index = -1;

        if (item == null || item.SourceObject == null)
            return false;

        int existingIndex =
            FindIndexBySource(item.SourceObject);

        if (existingIndex >= 0)
        {
            index = existingIndex;
            return true;
        }

        if (items.Count >= capacity)
        {
            InventoryFull?.Invoke();
            return false;
        }

        items.Add(item);
        index = items.Count - 1;

        GameObject storedVisual =
            CreateStoredVisual(item, index);

        item.SetStoredVisualObject(storedVisual);

        if (selectedIndex < 0)
            selectedIndex = index;

        Changed?.Invoke();
        return true;
    }

    public void Select(int index)
    {
        int normalized =
            IsValidIndex(index)
                ? index
                : -1;

        if (selectedIndex == normalized)
            return;

        selectedIndex = normalized;
        Changed?.Invoke();
    }

    public void SetEquipped(int index)
    {
        int normalized =
            IsValidIndex(index)
                ? index
                : -1;

        if (equippedIndex == normalized)
            return;

        equippedIndex = normalized;
        Changed?.Invoke();
    }

    public void SetSelectedAndEquipped(int index)
    {
        int normalized =
            IsValidIndex(index)
                ? index
                : -1;

        bool changed =
            selectedIndex != normalized ||
            equippedIndex != normalized;

        selectedIndex = normalized;
        equippedIndex = normalized;

        if (changed)
            Changed?.Invoke();
    }

    public bool RemoveAt(int index)
    {
        if (!IsValidIndex(index))
            return false;

        InventoryItemData removed = items[index];

        if (removed?.StoredVisualObject != null)
            Destroy(removed.StoredVisualObject);

        items.RemoveAt(index);

        selectedIndex = RemapIndexAfterRemove(
            selectedIndex,
            index,
            items.Count
        );

        equippedIndex = RemapIndexAfterRemove(
            equippedIndex,
            index,
            items.Count
        );

        Changed?.Invoke();
        return true;
    }

    private bool IsValidIndex(int index)
    {
        return index >= 0 && index < items.Count;
    }

    private static int RemapIndexAfterRemove(
        int currentIndex,
        int removedIndex,
        int newCount)
    {
        if (currentIndex == removedIndex)
            return -1;

        if (currentIndex > removedIndex)
            currentIndex--;

        return currentIndex >= 0 &&
               currentIndex < newCount
            ? currentIndex
            : -1;
    }

    private GameObject CreateStoredVisual(
        InventoryItemData item,
        int index)
    {
        GameObject root = new(
            $"Stored_{index}_{item.ItemName}"
        );

        root.transform.SetParent(transform, false);

        for (int i = 0; i < item.MeshParts.Count; i++)
        {
            InventoryMeshPartData part =
                item.MeshParts[i];

            if (part == null || part.Mesh == null)
                continue;

            GameObject meshObject =
                new($"MeshPart_{i}");

            meshObject.transform.SetParent(
                root.transform,
                false
            );

            meshObject.transform.localPosition =
                part.LocalPosition;

            meshObject.transform.localRotation =
                part.LocalRotation;

            meshObject.transform.localScale =
                part.LocalScale;

            MeshFilter meshFilter =
                meshObject.AddComponent<MeshFilter>();

            MeshRenderer meshRenderer =
                meshObject.AddComponent<MeshRenderer>();

            meshFilter.sharedMesh = part.Mesh;
            meshRenderer.sharedMaterials =
                part.Materials;
        }

        // Hierarchy에는 보관되지만 게임 화면에는 렌더링되지 않습니다.
        root.SetActive(false);

        return root;
    }
}
