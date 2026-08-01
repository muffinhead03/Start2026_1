using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

[DisallowMultipleComponent]
public sealed class BringData : MonoBehaviour
{
    [Header("Mesh 탐색")]

    [Tooltip(
        "Object_Grabbable 아래에서 Mesh를 찾지 못하면 " +
        "부모 오브젝트를 올라가며 외형을 찾습니다."
    )]
    [SerializeField]
    private bool searchParentWhenMeshMissing = true;

    [Tooltip(
        "부모 방향으로 탐색할 최대 단계입니다. " +
        "너무 크게 설정하면 HandPivot이나 Player Mesh까지 포함될 수 있습니다."
    )]
    [SerializeField, Min(0)]
    private int maxParentSearchDepth = 2;

    [Header("Debug")]
    [SerializeField]
    private bool showDebugLog = true;

    /// <summary>
    /// Object_Grabbable에서 이름, 설명, Mesh 정보를 수집합니다.
    /// </summary>
    public InventoryItemData Capture(
        Object_Grabbable source)
    {
        if (source == null)
        {
            Debug.LogWarning(
                "[BringData] 가져올 Object_Grabbable이 없습니다.",
                this
            );

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

        Transform captureRoot =
            FindCaptureRoot(source.transform);

        List<InventoryMeshPartData> meshParts =
            CaptureMeshParts(captureRoot);

        if (meshParts.Count == 0)
        {
            Debug.LogWarning(
                $"[BringData] '{itemName}'에서 표시 가능한 " +
                "MeshFilter + MeshRenderer를 찾지 못했습니다. " +
                $"탐색 루트: " +
                $"{(captureRoot != null ? captureRoot.name : "null")}",
                source
            );
        }
        else if (showDebugLog)
        {
            Debug.Log(
                $"[BringData] '{itemName}' 데이터 수집 완료. " +
                $"Root={captureRoot.name}, " +
                $"MeshParts={meshParts.Count}",
                source
            );
        }

        return new InventoryItemData(
            itemName,
            description,
            source,
            meshParts
        );
    }

    /// <summary>
    /// Mesh를 수집할 기준 루트를 찾습니다.
    /// 먼저 Object_Grabbable 자신을 검사하고,
    /// 없을 경우 설정된 깊이만큼 부모를 검사합니다.
    /// </summary>
    private Transform FindCaptureRoot(
        Transform sourceTransform)
    {
        if (sourceTransform == null)
            return null;

        if (ContainsRenderableMesh(sourceTransform))
            return sourceTransform;

        if (!searchParentWhenMeshMissing)
            return sourceTransform;

        Transform current =
            sourceTransform.parent;

        for (int depth = 0;
             depth < maxParentSearchDepth &&
             current != null;
             depth++)
        {
            if (ContainsRenderableMesh(current))
            {
                if (showDebugLog)
                {
                    Debug.Log(
                        $"[BringData] '{sourceTransform.name}' 아래에서 " +
                        $"Mesh를 찾지 못해 부모 '{current.name}'을 " +
                        "외형 루트로 사용합니다.",
                        sourceTransform
                    );
                }

                return current;
            }

            current = current.parent;
        }

        return sourceTransform;
    }

    /// <summary>
    /// 지정된 루트 아래에 표시 가능한 정적 Mesh가 있는지 확인합니다.
    /// </summary>
    private static bool ContainsRenderableMesh(
        Transform root)
    {
        if (root == null)
            return false;

        MeshFilter[] meshFilters =
            root.GetComponentsInChildren<MeshFilter>(true);

        foreach (MeshFilter meshFilter in meshFilters)
        {
            if (meshFilter == null ||
                meshFilter.sharedMesh == null)
            {
                continue;
            }

            MeshRenderer renderer =
                meshFilter.GetComponent<MeshRenderer>();

            if (renderer != null)
                return true;
        }

        return false;
    }

    /// <summary>
    /// sourceRoot 아래의 정적 Mesh들을 수집합니다.
    /// 각 파트 Transform은 sourceRoot 기준으로 저장합니다.
    /// </summary>
    private static List<InventoryMeshPartData>
        CaptureMeshParts(Transform sourceRoot)
    {
        List<InventoryMeshPartData> result =
            new List<InventoryMeshPartData>();

        if (sourceRoot == null)
            return result;

        MeshFilter[] meshFilters =
            sourceRoot.GetComponentsInChildren<MeshFilter>(true);

        foreach (MeshFilter meshFilter in meshFilters)
        {
            if (meshFilter == null ||
                meshFilter.sharedMesh == null)
            {
                continue;
            }

            MeshRenderer meshRenderer =
                meshFilter.GetComponent<MeshRenderer>();

            // MeshRenderer가 없으면 프리뷰 화면에 표시할 수 없음
            if (meshRenderer == null)
                continue;

            Matrix4x4 relativeMatrix =
                sourceRoot.worldToLocalMatrix *
                meshFilter.transform.localToWorldMatrix;

            Vector4 positionColumn =
                relativeMatrix.GetColumn(3);

            Vector3 localPosition =
                new Vector3(
                    positionColumn.x,
                    positionColumn.y,
                    positionColumn.z
                );

            Quaternion localRotation =
                relativeMatrix.rotation;

            Vector3 localScale =
                relativeMatrix.lossyScale;

            Material[] sharedMaterials =
                meshRenderer.sharedMaterials;

            Material[] materials =
                sharedMaterials != null
                    ? (Material[])sharedMaterials.Clone()
                    : Array.Empty<Material>();

            InventoryMeshPartData partData =
                new InventoryMeshPartData(
                    meshFilter.sharedMesh,
                    materials,
                    localPosition,
                    localRotation,
                    localScale
                );

            result.Add(partData);
        }

        return result;
    }

    /// <summary>
    /// 필드 또는 프로퍼티에서 문자열을 읽습니다.
    /// Object_Grabbable을 직접 수정하지 않기 위한 호환 처리입니다.
    /// </summary>
    private static string ReadStringMember(
        object target,
        string fallback,
        params string[] memberNames)
    {
        if (target == null)
            return fallback ?? string.Empty;

        const BindingFlags flags =
            BindingFlags.Instance |
            BindingFlags.Public |
            BindingFlags.NonPublic |
            BindingFlags.DeclaredOnly;

        Type currentType =
            target.GetType();

        while (currentType != null)
        {
            foreach (string memberName in memberNames)
            {
                FieldInfo field =
                    currentType.GetField(
                        memberName,
                        flags
                    );

                if (field != null &&
                    field.FieldType == typeof(string))
                {
                    string value =
                        field.GetValue(target) as string;

                    return NormalizeString(
                        value,
                        fallback
                    );
                }

                PropertyInfo property =
                    currentType.GetProperty(
                        memberName,
                        flags
                    );

                if (property == null ||
                    property.PropertyType != typeof(string) ||
                    !property.CanRead ||
                    property.GetIndexParameters().Length > 0)
                {
                    continue;
                }

                try
                {
                    string propertyValue =
                        property.GetValue(target) as string;

                    return NormalizeString(
                        propertyValue,
                        fallback
                    );
                }
                catch (Exception exception)
                {
                    Debug.LogWarning(
                        $"[BringData] '{memberName}' 프로퍼티를 " +
                        $"읽지 못했습니다: {exception.Message}"
                    );
                }
            }

            currentType =
                currentType.BaseType;
        }

        return fallback ?? string.Empty;
    }

    private static string NormalizeString(
        string value,
        string fallback)
    {
        if (string.IsNullOrWhiteSpace(value))
            return fallback?.Trim() ?? string.Empty;

        return value.Trim();
    }
}