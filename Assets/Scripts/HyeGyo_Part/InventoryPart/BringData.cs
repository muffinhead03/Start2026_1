using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

[DisallowMultipleComponent]
public sealed class BringData : MonoBehaviour
{
    [Header("Debug")]
    [SerializeField]
    private bool showDebugLog;

    /// <summary>
    /// InventoryData의 선택 index를 기준으로
    /// 이름, 설명, Mesh 정보를 가져옵니다.
    /// </summary>
    public InventoryDisplayData BuildDisplayData(
        InventoryData inventoryData,
        int slotIndex)
    {
        if (inventoryData == null)
        {
            return null;
        }

        Object_Grabbable sourceObject =
            inventoryData.GetObjectAt(
                slotIndex
            );

        if (sourceObject == null)
        {
            return null;
        }

        string objectName =
            inventoryData.GetObjectNameAt(
                slotIndex
            );

        string description =
            ResolveDescription(
                sourceObject
            );

        List<InventoryMeshPartData> meshParts =
            CollectMeshParts(
                sourceObject.transform
            );

        InventoryDisplayData displayData =
            new InventoryDisplayData(
                objectName,
                description,
                sourceObject,
                meshParts
            );

        if (showDebugLog)
        {
            Debug.Log(
                $"[BringData] 표시 데이터 생성: " +
                $"Index={slotIndex}, " +
                $"Name={displayData.ItemName}, " +
                $"MeshParts={displayData.MeshParts.Count}",
                sourceObject
            );
        }

        return displayData;
    }

    public string GetItemNameAt(
        InventoryData inventoryData,
        int slotIndex)
    {
        if (inventoryData == null)
        {
            return "—";
        }

        return inventoryData.GetObjectNameAt(
            slotIndex
        );
    }

    private static string ResolveDescription(
        Object_Grabbable sourceObject)
    {
        if (sourceObject == null)
        {
            return string.Empty;
        }

        Type sourceType =
            sourceObject.GetType();

        FieldInfo descriptionField =
            sourceType.GetField(
                "description",
                BindingFlags.Instance |
                BindingFlags.Public |
                BindingFlags.NonPublic
            );

        if (descriptionField != null &&
            descriptionField.FieldType == typeof(string))
        {
            object fieldValue =
                descriptionField.GetValue(
                    sourceObject
                );

            return fieldValue as string
                ?? string.Empty;
        }

        PropertyInfo descriptionProperty =
            sourceType.GetProperty(
                "Description",
                BindingFlags.Instance |
                BindingFlags.Public |
                BindingFlags.NonPublic
            );

        if (descriptionProperty != null &&
            descriptionProperty.PropertyType ==
            typeof(string))
        {
            object propertyValue =
                descriptionProperty.GetValue(
                    sourceObject
                );

            return propertyValue as string
                ?? string.Empty;
        }

        return string.Empty;
    }

    private static List<InventoryMeshPartData>
        CollectMeshParts(Transform sourceRoot)
    {
        List<InventoryMeshPartData> result =
            new List<InventoryMeshPartData>();

        if (sourceRoot == null)
        {
            return result;
        }

        CollectMeshFilters(
            sourceRoot,
            result
        );

        CollectSkinnedMeshes(
            sourceRoot,
            result
        );

        return result;
    }

    private static void CollectMeshFilters(
        Transform sourceRoot,
        List<InventoryMeshPartData> result)
    {
        MeshFilter[] meshFilters =
            sourceRoot.GetComponentsInChildren<MeshFilter>(
                true
            );

        for (int i = 0;
             i < meshFilters.Length;
             i++)
        {
            MeshFilter meshFilter =
                meshFilters[i];

            if (meshFilter == null ||
                meshFilter.sharedMesh == null)
            {
                continue;
            }

            MeshRenderer meshRenderer =
                meshFilter.GetComponent<MeshRenderer>();

            Material[] materials =
                meshRenderer != null
                    ? meshRenderer.sharedMaterials
                    : Array.Empty<Material>();

            Matrix4x4 relativeMatrix =
                sourceRoot.worldToLocalMatrix *
                meshFilter.transform.localToWorldMatrix;

            DecomposeMatrix(
                relativeMatrix,
                out Vector3 localPosition,
                out Quaternion localRotation,
                out Vector3 localScale
            );

            result.Add(
                new InventoryMeshPartData(
                    meshFilter.sharedMesh,
                    materials,
                    localPosition,
                    localRotation,
                    localScale
                )
            );
        }
    }

    private static void CollectSkinnedMeshes(
        Transform sourceRoot,
        List<InventoryMeshPartData> result)
    {
        SkinnedMeshRenderer[] renderers =
            sourceRoot.GetComponentsInChildren
                <SkinnedMeshRenderer>(true);

        for (int i = 0;
             i < renderers.Length;
             i++)
        {
            SkinnedMeshRenderer renderer =
                renderers[i];

            if (renderer == null ||
                renderer.sharedMesh == null)
            {
                continue;
            }

            Matrix4x4 relativeMatrix =
                sourceRoot.worldToLocalMatrix *
                renderer.transform.localToWorldMatrix;

            DecomposeMatrix(
                relativeMatrix,
                out Vector3 localPosition,
                out Quaternion localRotation,
                out Vector3 localScale
            );

            result.Add(
                new InventoryMeshPartData(
                    renderer.sharedMesh,
                    renderer.sharedMaterials,
                    localPosition,
                    localRotation,
                    localScale
                )
            );
        }
    }

    private static void DecomposeMatrix(
        Matrix4x4 matrix,
        out Vector3 position,
        out Quaternion rotation,
        out Vector3 scale)
    {
        position =
            matrix.GetColumn(3);

        Vector3 right =
            matrix.GetColumn(0);

        Vector3 up =
            matrix.GetColumn(1);

        Vector3 forward =
            matrix.GetColumn(2);

        scale =
            new Vector3(
                right.magnitude,
                up.magnitude,
                forward.magnitude
            );

        if (scale.x > 0.0001f)
        {
            right /= scale.x;
        }

        if (scale.y > 0.0001f)
        {
            up /= scale.y;
        }

        if (scale.z > 0.0001f)
        {
            forward /= scale.z;
        }

        if (Vector3.Dot(
                Vector3.Cross(right, up),
                forward) < 0f)
        {
            scale.x *= -1f;
            right *= -1f;
        }

        if (forward.sqrMagnitude < 0.0001f)
        {
            forward = Vector3.forward;
        }

        if (up.sqrMagnitude < 0.0001f)
        {
            up = Vector3.up;
        }

        rotation =
            Quaternion.LookRotation(
                forward,
                up
            );
    }
}