using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.UI;

/// <summary>
/// 슬롯 하나의 버튼, 이름 텍스트,
/// 선택 프레임과 장착 프레임을 묶어서 관리합니다.
/// </summary>
[Serializable]
public sealed class InventorySlotView
{
    [SerializeField]
    private Button button;

    [SerializeField]
    private TMP_Text itemNameText;

    [SerializeField]
    private GameObject selectedFrame;

    [SerializeField]
    private GameObject equippedFrame;

    private UnityAction clickAction;
    private int boundSlotIndex = -1;

    public void Bind(
        int slotIndex,
        Action<int> onClicked)
    {
        Unbind();

        if (button == null)
        {
            Debug.LogError(
                $"[InventorySlotView] " +
                $"Index {slotIndex}의 Button이 연결되지 않았습니다."
            );

            return;
        }

        if (onClicked == null)
        {
            Debug.LogError(
                $"[InventorySlotView] " +
                $"Index {slotIndex}의 클릭 콜백이 없습니다.",
                button
            );

            return;
        }

        boundSlotIndex = slotIndex;

        clickAction = () =>
        {
            onClicked.Invoke(
                boundSlotIndex
            );
        };

        button.onClick.AddListener(
            clickAction
        );

        button.interactable = true;
    }

    public void Unbind()
    {
        if (button != null &&
            clickAction != null)
        {
            button.onClick.RemoveListener(
                clickAction
            );
        }

        clickAction = null;
        boundSlotIndex = -1;
    }

    public void Refresh(
        string itemName,
        bool selected,
        bool equipped)
    {
        if (itemNameText != null)
        {
            itemNameText.text =
                string.IsNullOrWhiteSpace(itemName)
                    ? "—"
                    : itemName;
        }

        if (selectedFrame != null)
        {
            selectedFrame.SetActive(
                selected
            );
        }

        if (equippedFrame != null)
        {
            equippedFrame.SetActive(
                equipped
            );
        }
    }
}

[DisallowMultipleComponent]
public sealed class InventoryUIEffect : MonoBehaviour
{
    [Header("Canvas")]
    [SerializeField]
    private GameObject inventoryRoot;

    [Header("Slots")]
    [Tooltip("Index 0부터 Index 7까지 순서대로 연결합니다.")]
    [SerializeField]
    private InventorySlotView[] slots =
        new InventorySlotView[8];

    [Header("Selected Item")]
    [SerializeField]
    private TMP_Text selectedNameText;

    [SerializeField]
    private TMP_Text selectedDescriptionText;

    [Header("Preview Hierarchy")]
    [Tooltip(
        "Main Camera 아래의 ObjectSelected를 연결합니다. " +
        "프리뷰가 있을 때만 활성화됩니다."
    )]
    [SerializeField]
    private GameObject objectSelected;

    [Tooltip(
        "ObjectSelected 아래의 SelectedRoot를 연결합니다. " +
        "생성된 Preview 오브젝트의 부모가 됩니다."
    )]
    [SerializeField]
    private Transform selectedRoot;

    [Header("Preview Transform")]
    [Tooltip("SelectedRoot 기준 프리뷰 위치입니다.")]
    [SerializeField]
    private Vector3 previewPosition =
        Vector3.zero;

    [Tooltip("프리뷰에 적용할 기본 회전값입니다.")]
    [SerializeField]
    private Vector3 previewRotation =
        new Vector3(15f, -25f, 0f);

    [Tooltip(
        "프리뷰 전체 Bounds의 가장 긴 축을 " +
        "이 크기에 맞춥니다."
    )]
    [Min(0.0001f)]
    [SerializeField]
    private float previewTargetSize = 0.3f;

    [Header("Preview Renderer")]
    [SerializeField]
    private bool disablePreviewShadows = true;

    [Header("Preview Debug")]
    [SerializeField]
    private bool showDebugLog;

    private GameObject currentPreviewObject;
    private Object_Grabbable currentPreviewSource;

    private void Awake()
    {
        SetObjectSelectedActive(false);
    }

    private void OnDestroy()
    {
        UnbindSlots();
        HidePreview();
    }

    /// <summary>
    /// 각 슬롯 버튼을 InventoryUIManager.SelectSlot에 연결합니다.
    /// </summary>
    public void BindSlots(
        Action<int> onSlotClicked)
    {
        if (slots == null ||
            slots.Length == 0)
        {
            Debug.LogError(
                "[InventoryUIEffect] Slots 배열이 비어 있습니다.",
                this
            );

            return;
        }

        if (onSlotClicked == null)
        {
            Debug.LogError(
                "[InventoryUIEffect] 슬롯 클릭 콜백이 없습니다.",
                this
            );

            return;
        }

        UnbindSlots();

        for (int i = 0;
             i < slots.Length;
             i++)
        {
            InventorySlotView slot =
                slots[i];

            if (slot == null)
            {
                Debug.LogError(
                    $"[InventoryUIEffect] " +
                    $"Slots Element {i}가 비어 있습니다.",
                    this
                );

                continue;
            }

            slot.Bind(
                i,
                onSlotClicked
            );
        }
    }

    /// <summary>
    /// 인벤토리 창을 열거나 닫습니다.
    /// </summary>
    public void SetOpen(bool open)
    {
        if (inventoryRoot != null)
        {
            inventoryRoot.SetActive(
                open
            );
        }
        else
        {
            //Debug.LogError(
            //    "[InventoryUIEffect] " +
            //    "Inventory Root가 연결되지 않았습니다.",
            //    this
            //);
        }

        if (!open)
        {
            HidePreview();
        }
    }

    /// <summary>
    /// 슬롯, 우측 정보, 3D 프리뷰를 갱신합니다.
    /// </summary>
    public void Refresh(
        InventoryData inventoryData,
        InventoryDisplayData selectedDisplayData,
        bool open)
    {
        RefreshSlots(
            inventoryData
        );

        RefreshSelectedInformation(
            selectedDisplayData,
            open
        );
    }

    public void PlayInventoryFull()
    {
        Debug.Log(
            "[InventoryUIEffect] 인벤토리가 가득 찼습니다.",
            this
        );
    }

    /// <summary>
    /// 8개 슬롯의 이름과 선택/장착 프레임을 갱신합니다.
    /// InventoryData에서 이름을 직접 가져옵니다.
    /// </summary>
    private void RefreshSlots(
        InventoryData inventoryData)
    {
        if (slots == null)
        {
            return;
        }

        for (int i = 0;
             i < slots.Length;
             i++)
        {
            InventorySlotView slot =
                slots[i];

            if (slot == null)
            {
                continue;
            }

            string itemName =
                inventoryData != null
                    ? inventoryData.GetObjectNameAt(i)
                    : "—";

            bool selected =
                inventoryData != null &&
                inventoryData.SelectedIndex == i;

            bool equipped =
                inventoryData != null &&
                inventoryData.EquippedIndex == i;

            slot.Refresh(
                itemName,
                selected,
                equipped
            );
        }
    }

    /// <summary>
    /// 우측 이름, 설명과 3D 프리뷰를 갱신합니다.
    /// </summary>
    private void RefreshSelectedInformation(
        InventoryDisplayData selectedDisplayData,
        bool open)
    {
        if (selectedNameText != null)
        {
            selectedNameText.text =
                selectedDisplayData != null
                    ? selectedDisplayData.ItemName
                    : "—";
        }

        if (selectedDescriptionText != null)
        {
            selectedDescriptionText.text =
                selectedDisplayData != null
                    ? selectedDisplayData.Description
                    : string.Empty;
        }

        if (!open ||
            selectedDisplayData == null)
        {
            HidePreview();
            return;
        }

        /*
         * Preview Mesh 생성 실패가 Inventory 입력 전체까지
         * 끊어지지 않도록 여기서 복구합니다.
         */
        try
        {
            ShowPreview(
                selectedDisplayData
            );
        }
        catch (Exception exception)
        {
            Debug.LogWarning(
                "[InventoryUIEffect] Preview 갱신 중 예외를 복구했습니다. " +
                exception.GetType().Name + ": " +
                exception.Message,
                this
            );

            HidePreview();
        }
    }

    /// <summary>
    /// 다음 구조로 프리뷰를 생성합니다.
    ///
    /// ObjectSelected
    /// └─ SelectedRoot
    ///    └─ Preview_ItemName
    ///       └─ Geometry
    ///          ├─ MeshPart_0
    ///          └─ MeshPart_1
    /// </summary>
    private void ShowPreview(
        InventoryDisplayData displayData)
    {
        if (displayData == null ||
            displayData.SourceObject == null ||
            displayData.MeshParts == null ||
            displayData.MeshParts.Count == 0)
        {
            HidePreview();
            return;
        }

        if (!ValidatePreviewHierarchy())
        {
            HidePreview();
            return;
        }

        /*
         * 현재 선택된 물체의 프리뷰가 이미 생성되어 있으면
         * 다시 만들지 않고 ObjectSelected만 활성화합니다.
         */
        if (currentPreviewObject != null &&
            currentPreviewSource ==
            displayData.SourceObject)
        {
            currentPreviewObject.SetActive(true);
            SetObjectSelectedActive(true);
            return;
        }

        HidePreview();

        currentPreviewSource =
            displayData.SourceObject;

        currentPreviewObject =
            new GameObject(
                $"Preview_{displayData.ItemName}"
            );

        Transform previewTransform =
            currentPreviewObject.transform;

        previewTransform.SetParent(
            selectedRoot,
            false
        );

        previewTransform.localPosition =
            previewPosition;

        previewTransform.localRotation =
            Quaternion.Euler(
                previewRotation
            );

        /*
         * Mesh를 담는 Geometry 루트입니다.
         * 프리뷰의 중심을 맞출 때 이 Transform만 이동합니다.
         */
        GameObject geometryObject =
            new GameObject("Geometry");

        Transform geometryRoot =
            geometryObject.transform;

        geometryRoot.SetParent(
            previewTransform,
            false
        );

        geometryRoot.localPosition =
            Vector3.zero;

        geometryRoot.localRotation =
            Quaternion.identity;

        geometryRoot.localScale =
            Vector3.one;

        Bounds combinedBounds = default;
        bool hasBounds = false;
        int createdPartCount = 0;

        for (int i = 0;
             i < displayData.MeshParts.Count;
             i++)
        {
            InventoryMeshPartData meshPart =
                displayData.MeshParts[i];

            if (meshPart == null ||
                meshPart.Mesh == null)
            {
                continue;
            }

            Transform partTransform =
                CreatePreviewMeshPart(
                    meshPart,
                    i,
                    geometryRoot
                );

            if (partTransform == null)
            {
                continue;
            }

            createdPartCount++;

            EncapsulateMeshBounds(
                meshPart.Mesh.bounds,
                partTransform,
                previewTransform,
                ref combinedBounds,
                ref hasBounds
            );
        }

        if (createdPartCount == 0 ||
            !hasBounds)
        {
            Debug.LogWarning(
                $"[InventoryUIEffect] " +
                $"'{displayData.ItemName}'의 프리뷰 Mesh를 " +
                "생성하지 못했습니다.",
                this
            );

            HidePreview();
            return;
        }

        CenterAndResizePreview(
            previewTransform,
            geometryRoot,
            combinedBounds
        );

        currentPreviewObject.SetActive(true);
        SetObjectSelectedActive(true);

        if (showDebugLog)
        {
            Debug.Log(
                $"[InventoryUIEffect] 프리뷰 생성 완료: " +
                $"Name={displayData.ItemName}, " +
                $"CreatedParts={createdPartCount}, " +
                $"BoundsSize={combinedBounds.size}, " +
                $"TargetSize={previewTargetSize}",
                this
            );
        }
    }

    /// <summary>
    /// 프리뷰 Mesh 한 부분을 Geometry 아래에 생성합니다.
    /// </summary>
    private Transform CreatePreviewMeshPart(
        InventoryMeshPartData meshPart,
        int partIndex,
        Transform geometryRoot)
    {
        if (meshPart == null ||
            meshPart.Mesh == null ||
            geometryRoot == null)
        {
            return null;
        }

        GameObject partObject =
            new GameObject(
                $"MeshPart_{partIndex}"
            );

        Transform partTransform =
            partObject.transform;

        partTransform.SetParent(
            geometryRoot,
            false
        );

        partTransform.localPosition =
            meshPart.LocalPosition;

        partTransform.localRotation =
            meshPart.LocalRotation;

        partTransform.localScale =
            meshPart.LocalScale;

        MeshFilter meshFilter =
            partObject.AddComponent<MeshFilter>();

        meshFilter.sharedMesh =
            meshPart.Mesh;

        MeshRenderer meshRenderer =
            partObject.AddComponent<MeshRenderer>();

        Material[] materials =
            meshPart.Materials;

        if (materials != null &&
            materials.Length > 0)
        {
            meshRenderer.sharedMaterials =
                materials;
        }

        if (disablePreviewShadows)
        {
            meshRenderer.shadowCastingMode =
                ShadowCastingMode.Off;

            meshRenderer.receiveShadows =
                false;
        }

        meshRenderer.lightProbeUsage =
            LightProbeUsage.Off;

        meshRenderer.reflectionProbeUsage =
            ReflectionProbeUsage.Off;

        return partTransform;
    }

    /// <summary>
    /// Mesh의 로컬 Bounds 8개 꼭짓점을 Preview 로컬 공간으로 변환해
    /// 전체 Bounds에 포함합니다.
    /// </summary>
    private static void EncapsulateMeshBounds(
        Bounds meshBounds,
        Transform partTransform,
        Transform previewTransform,
        ref Bounds combinedBounds,
        ref bool hasBounds)
    {
        if (partTransform == null ||
            previewTransform == null)
        {
            return;
        }

        Matrix4x4 partToPreviewMatrix =
            previewTransform.worldToLocalMatrix *
            partTransform.localToWorldMatrix;

        Vector3 min =
            meshBounds.min;

        Vector3 max =
            meshBounds.max;

        for (int x = 0; x < 2; x++)
        {
            for (int y = 0; y < 2; y++)
            {
                for (int z = 0; z < 2; z++)
                {
                    Vector3 localCorner =
                        new Vector3(
                            x == 0 ? min.x : max.x,
                            y == 0 ? min.y : max.y,
                            z == 0 ? min.z : max.z
                        );

                    Vector3 previewCorner =
                        partToPreviewMatrix.MultiplyPoint3x4(
                            localCorner
                        );

                    if (!hasBounds)
                    {
                        combinedBounds =
                            new Bounds(
                                previewCorner,
                                Vector3.zero
                            );

                        hasBounds = true;
                    }
                    else
                    {
                        combinedBounds.Encapsulate(
                            previewCorner
                        );
                    }
                }
            }
        }
    }

    /// <summary>
    /// Mesh 전체 중심을 SelectedRoot 원점에 맞추고,
    /// 가장 긴 축을 previewTargetSize에 맞춥니다.
    /// </summary>
    private void CenterAndResizePreview(
        Transform previewTransform,
        Transform geometryRoot,
        Bounds combinedBounds)
    {
        if (previewTransform == null ||
            geometryRoot == null)
        {
            return;
        }

        geometryRoot.localPosition =
            -combinedBounds.center;

        float largestSize =
            Mathf.Max(
                combinedBounds.size.x,
                combinedBounds.size.y,
                combinedBounds.size.z
            );

        if (largestSize <= 0.0001f)
        {
            previewTransform.localScale =
                Vector3.one;

            return;
        }

        float normalizedScale =
            Mathf.Max(0.0001f, previewTargetSize) /
            largestSize;

        previewTransform.localScale =
            Vector3.one * normalizedScale;
    }

    /// <summary>
    /// ObjectSelected와 SelectedRoot 연결 상태를 검사합니다.
    /// </summary>
    private bool ValidatePreviewHierarchy()
    {
        if (objectSelected == null)
        {
            Debug.LogError(
                "[InventoryUIEffect] " +
                "ObjectSelected가 연결되지 않았습니다.",
                this
            );

            return false;
        }

        if (selectedRoot == null)
        {
            Debug.LogError(
                "[InventoryUIEffect] " +
                "SelectedRoot가 연결되지 않았습니다.",
                this
            );

            return false;
        }

        if (!selectedRoot.IsChildOf(
                objectSelected.transform))
        {
            Debug.LogWarning(
                "[InventoryUIEffect] " +
                "SelectedRoot가 ObjectSelected의 자식이 아닙니다. " +
                "Hierarchy 연결을 확인하세요.",
                selectedRoot
            );
        }

        return true;
    }

    /// <summary>
    /// 현재 프리뷰를 삭제하고 ObjectSelected를 비활성화합니다.
    /// 아이템 삭제, 선택 해제 또는 인벤토리 닫기 시 호출됩니다.
    /// </summary>
    private void HidePreview()
    {
        /*
         * Destroy는 프레임 끝에 실행되므로
         * 제일 먼저 Preview 부모 전체를 비활성화합니다.
         * 이렇게 하면 삭제 대기 중 Mesh도 화면에 남지 않습니다.
         */
        SetObjectSelectedActive(
            false
        );

        GameObject trackedPreview =
            currentPreviewObject;

        if (trackedPreview != null)
        {
            trackedPreview.SetActive(
                false
            );
        }

        bool trackedWasUnderSelectedRoot =
            trackedPreview != null &&
            selectedRoot != null &&
            trackedPreview.transform.IsChildOf(
                selectedRoot
            );

        /*
         * SelectedRoot 아래에서 이 스크립트가 만든 Preview_*만 정리합니다.
         * 다른 수동 자식 오브젝트는 건드리지 않습니다.
         */
        if (selectedRoot != null)
        {
            for (int i = selectedRoot.childCount - 1;
                 i >= 0;
                 i--)
            {
                Transform child =
                    selectedRoot.GetChild(i);

                if (child == null)
                {
                    continue;
                }

                GameObject childObject =
                    child.gameObject;

                bool isGeneratedPreview =
                    childObject == trackedPreview ||
                    childObject.name.StartsWith(
                        "Preview_",
                        StringComparison.Ordinal
                    );

                if (!isGeneratedPreview)
                {
                    continue;
                }

                childObject.SetActive(
                    false
                );

                if (Application.isPlaying)
                {
                    Destroy(
                        childObject
                    );
                }
                else
                {
                    DestroyImmediate(
                        childObject
                    );
                }
            }
        }

        /*
         * 추적 중인 Preview가 Hierarchy 이상으로 SelectedRoot 밖에 있다면
         * 그것도 따로 제거합니다.
         */
        if (trackedPreview != null &&
            !trackedWasUnderSelectedRoot)
        {
            if (Application.isPlaying)
            {
                Destroy(
                    trackedPreview
                );
            }
            else
            {
                DestroyImmediate(
                    trackedPreview
                );
            }
        }

        currentPreviewObject = null;
        currentPreviewSource = null;
    }

    private void SetObjectSelectedActive(
        bool active)
    {
        if (objectSelected != null &&
            objectSelected.activeSelf != active)
        {
            objectSelected.SetActive(
                active
            );
        }
    }

    private void UnbindSlots()
    {
        if (slots == null)
        {
            return;
        }

        for (int i = 0;
             i < slots.Length;
             i++)
        {
            slots[i]?.Unbind();
        }
    }
}