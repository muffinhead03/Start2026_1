using System.Collections.Generic;

public sealed class InventoryDisplayData
{
    public string ItemName { get; }
    public string Description { get; }

    public Object_Grabbable SourceObject { get; }

    public IReadOnlyList<InventoryMeshPartData>
        MeshParts { get; }

    public InventoryDisplayData(
        string itemName,
        string description,
        Object_Grabbable sourceObject,
        List<InventoryMeshPartData> meshParts)
    {
        ItemName =
            string.IsNullOrWhiteSpace(itemName)
                ? "Unknown"
                : itemName;

        Description =
            description ?? string.Empty;

        SourceObject =
            sourceObject;

        MeshParts =
            meshParts ??
            new List<InventoryMeshPartData>();
    }
}