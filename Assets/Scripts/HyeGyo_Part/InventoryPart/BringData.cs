using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

[DisallowMultipleComponent]
public sealed class BringData : MonoBehaviour
{
    public InventoryItemData Capture(Object_Grabbable source)
    {
        if (source == null)
        {
            Debug.LogWarning("[BringData] 가져올 Object_Grabbable이 없습니다.", this);
            return null;
        }

        string itemName = ReadStringMember(
            source,
            source.gameObject.name,
            "objectName",
            "ObjectName"
        );

        string description = ReadStringMember(
            source,
            string.Empty,
            "description",
            "Description"
        );

        List<InventoryMeshPartData> meshParts =
            CaptureMeshParts(source.transform);

        return new InventoryItemData(
            itemName,
            description,
            source,
            meshParts
        );
    }

    private static List<InventoryMeshPartData> CaptureMeshParts(
        Transform sourceRoot)
    {
        List<InventoryMeshPartData> result = new();

        MeshFilter[] meshFilters =
            sourceRoot.GetComponentsInChildren<MeshFilter>(true);

        foreach (MeshFilter meshFilter in meshFilters)
        {
            if (meshFilter == null || meshFilter.sharedMesh == null)
                continue;

            MeshRenderer meshRenderer =
                meshFilter.GetComponent<MeshRenderer>();

            Material[] materials =
                meshRenderer != null
                    ? meshRenderer.sharedMaterials
                    : Array.Empty<Material>();

            Matrix4x4 relativeMatrix =
                sourceRoot.worldToLocalMatrix *
                meshFilter.transform.localToWorldMatrix;

            Vector4 positionColumn =
                relativeMatrix.GetColumn(3);

            Vector3 localPosition = new(
                positionColumn.x,
                positionColumn.y,
                positionColumn.z
            );

            result.Add(
                new InventoryMeshPartData(
                    meshFilter.sharedMesh,
                    materials,
                    localPosition,
                    relativeMatrix.rotation,
                    relativeMatrix.lossyScale
                )
            );
        }

        return result;
    }

    private static string ReadStringMember(
        object target,
        string fallback,
        params string[] memberNames)
    {
        if (target == null)
            return fallback;

        const BindingFlags flags =
            BindingFlags.Instance |
            BindingFlags.Public |
            BindingFlags.NonPublic;

        Type type = target.GetType();

        foreach (string memberName in memberNames)
        {
            FieldInfo field = type.GetField(memberName, flags);

            if (field != null &&
                field.FieldType == typeof(string))
            {
                return field.GetValue(target) as string ?? fallback;
            }

            PropertyInfo property =
                type.GetProperty(memberName, flags);

            if (property != null &&
                property.PropertyType == typeof(string) &&
                property.CanRead)
            {
                return property.GetValue(target) as string ?? fallback;
            }
        }

        return fallback;
    }
}
