using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public sealed class InventoryMeshPartData
{
    [SerializeField] private Mesh mesh;
    [SerializeField] private Material[] materials = Array.Empty<Material>();
    [SerializeField] private Vector3 localPosition;
    [SerializeField] private Quaternion localRotation = Quaternion.identity;
    [SerializeField] private Vector3 localScale = Vector3.one;

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
        this.materials = materials ?? Array.Empty<Material>();
        this.localPosition = localPosition;
        this.localRotation = localRotation;
        this.localScale = localScale;
    }
}

[Serializable]
public sealed class InventoryItemData
{
    [SerializeField] private string itemName;
    [SerializeField, TextArea] private string description;
    [SerializeField] private Object_Grabbable sourceObject;
    [SerializeField] private GameObject storedVisualObject;
    [SerializeField] private List<InventoryMeshPartData> meshParts = new();

    public string ItemName => itemName;
    public string Description => description;
    public Object_Grabbable SourceObject => sourceObject;
    public GameObject StoredVisualObject => storedVisualObject;
    public IReadOnlyList<InventoryMeshPartData> MeshParts => meshParts;

    public InventoryItemData(
        string itemName,
        string description,
        Object_Grabbable sourceObject,
        List<InventoryMeshPartData> meshParts)
    {
        this.itemName = itemName;
        this.description = description;
        this.sourceObject = sourceObject;
        this.meshParts = meshParts ?? new List<InventoryMeshPartData>();
    }

    public void SetStoredVisualObject(GameObject value)
    {
        storedVisualObject = value;
    }
}
