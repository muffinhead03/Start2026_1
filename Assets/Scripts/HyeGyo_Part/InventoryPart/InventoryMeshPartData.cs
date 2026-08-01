using System;
using UnityEngine;

/// <summary>
/// 인벤토리 3D 프리뷰 하나를 구성하는 Mesh 정보입니다.
/// </summary>
[Serializable]
public sealed class InventoryMeshPartData
{
    private readonly Mesh mesh;
    private readonly Material[] materials;
    private readonly Vector3 localPosition;
    private readonly Quaternion localRotation;
    private readonly Vector3 localScale;

    public Mesh Mesh => mesh;

    public Material[] Materials =>
        materials;

    public Vector3 LocalPosition =>
        localPosition;

    public Quaternion LocalRotation =>
        localRotation;

    public Vector3 LocalScale =>
        localScale;

    public InventoryMeshPartData(
        Mesh mesh,
        Material[] materials,
        Vector3 localPosition,
        Quaternion localRotation,
        Vector3 localScale)
    {
        this.mesh = mesh;

        this.materials =
            materials ?? Array.Empty<Material>();

        this.localPosition =
            localPosition;

        this.localRotation =
            localRotation;

        this.localScale =
            localScale;
    }
}