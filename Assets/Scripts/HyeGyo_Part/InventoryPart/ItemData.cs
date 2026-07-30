using UnityEngine;

[CreateAssetMenu(
    fileName = "NewItemData",
    menuName = "Inventory/Item Data"
)]
public class ItemData : ScriptableObject
{
    [Header("기본 정보")]
    [SerializeField] private string itemId;
    [SerializeField] private string displayName;

    [TextArea(2, 5)]
    [SerializeField] private string description;

    [Header("3D 모델")]
    [SerializeField] private Mesh visualMesh;
    [SerializeField] private Material[] visualMaterials;

    public string ItemId => itemId;
    public string DisplayName => displayName;
    public string Description => description;

    public Mesh VisualMesh => visualMesh;
    public Material[] VisualMaterials => visualMaterials;
}