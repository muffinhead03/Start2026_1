using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public sealed class InventoryPreviewView : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private RawImage previewRawImage;

    [Header("Preview Scene")]
    [SerializeField] private Camera previewCamera;
    [SerializeField] private Transform previewRoot;
    [SerializeField] private Light previewLight;

    [Header("Render")]
    [SerializeField] private RenderTexture previewTexture;
    [SerializeField, Min(64)] private int runtimeTextureSize = 512;
    [SerializeField] private string previewLayerName =
        "InventoryPreview";

    [Header("Model")]
    [SerializeField] private Vector3 previewRotation =
        new(15f, 30f, 0f);

    [SerializeField, Min(0.1f)]
    private float cameraDistance = 10f;

    [SerializeField, Min(1f)]
    private float padding = 1.3f;

    [SerializeField] private bool autoRotate = true;
    [SerializeField] private float rotationSpeed = 20f;

    private GameObject currentPreviewObject;
    private InventoryItemData currentItem;
    private RenderTexture runtimeTexture;
    private bool ownsRuntimeTexture;

    private void Awake()
    {
        ConfigureOutput();
        Hide();
    }

    private void Update()
    {
        if (!autoRotate ||
            currentPreviewObject == null)
        {
            return;
        }

        Vector3 axis =
            previewRoot != null
                ? previewRoot.up
                : Vector3.up;

        currentPreviewObject.transform.Rotate(
            axis,
            rotationSpeed * Time.unscaledDeltaTime,
            Space.World
        );
    }

    private void OnDestroy()
    {
        ClearPreviewObject();

        if (ownsRuntimeTexture &&
            runtimeTexture != null)
        {
            runtimeTexture.Release();
            Destroy(runtimeTexture);
        }
    }

    public void Show(InventoryItemData item)
    {
        if (item == null)
        {
            Hide();
            return;
        }

        if (currentItem != item ||
            currentPreviewObject == null)
        {
            Build(item);
        }

        SetVisible(currentPreviewObject != null);
    }

    public void Hide()
    {
        SetVisible(false);
    }

    private void ConfigureOutput()
    {
        if (previewCamera == null ||
            previewRawImage == null)
        {
            return;
        }

        if (previewTexture == null)
        {
            runtimeTexture = new RenderTexture(
                runtimeTextureSize,
                runtimeTextureSize,
                24,
                RenderTextureFormat.ARGB32
            );

            runtimeTexture.name =
                "RT_InventoryPreview_Runtime";

            runtimeTexture.Create();

            previewTexture = runtimeTexture;
            ownsRuntimeTexture = true;
        }

        previewCamera.targetTexture = previewTexture;
        previewCamera.orthographic = true;
        previewCamera.clearFlags =
            CameraClearFlags.SolidColor;

        previewCamera.backgroundColor =
            new Color(0f, 0f, 0f, 0f);

        previewRawImage.texture = previewTexture;
        previewRawImage.raycastTarget = false;

        int layer =
            LayerMask.NameToLayer(previewLayerName);

        if (layer >= 0)
            previewCamera.cullingMask = 1 << layer;
        else
            Debug.LogWarning(
                $"[InventoryPreviewView] '{previewLayerName}' 레이어가 없습니다.",
                this
            );
    }

    private void Build(InventoryItemData item)
    {
        ClearPreviewObject();

        if (previewRoot == null ||
            item.MeshParts.Count == 0)
        {
            currentItem = null;
            return;
        }

        int layer =
            LayerMask.NameToLayer(previewLayerName);

        currentPreviewObject =
            new GameObject($"{item.ItemName}_Preview");

        currentPreviewObject.transform.SetParent(
            previewRoot,
            false
        );

        currentPreviewObject.transform.localPosition =
            Vector3.zero;

        currentPreviewObject.transform.localRotation =
            Quaternion.Euler(previewRotation);

        for (int i = 0; i < item.MeshParts.Count; i++)
        {
            InventoryMeshPartData part =
                item.MeshParts[i];

            if (part == null || part.Mesh == null)
                continue;

            GameObject meshObject =
                new($"MeshPart_{i}");

            meshObject.transform.SetParent(
                currentPreviewObject.transform,
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

            if (layer >= 0)
                meshObject.layer = layer;
        }

        if (layer >= 0)
            currentPreviewObject.layer = layer;

        CenterContent();
        FitCamera();

        currentItem = item;
    }

    private void CenterContent()
    {
        Renderer[] renderers =
            currentPreviewObject
                .GetComponentsInChildren<Renderer>();

        if (renderers.Length == 0)
            return;

        Bounds bounds = renderers[0].bounds;

        for (int i = 1; i < renderers.Length; i++)
            bounds.Encapsulate(renderers[i].bounds);

        currentPreviewObject.transform.position +=
            previewRoot.position - bounds.center;
    }

    private void FitCamera()
    {
        if (previewCamera == null ||
            previewRoot == null ||
            currentPreviewObject == null)
        {
            return;
        }

        Renderer[] renderers =
            currentPreviewObject
                .GetComponentsInChildren<Renderer>();

        if (renderers.Length == 0)
            return;

        Bounds bounds = renderers[0].bounds;

        for (int i = 1; i < renderers.Length; i++)
            bounds.Encapsulate(renderers[i].bounds);

        Vector3 forward = previewRoot.forward;

        previewCamera.transform.position =
            previewRoot.position -
            forward * cameraDistance;

        previewCamera.transform.rotation =
            Quaternion.LookRotation(
                forward,
                previewRoot.up
            );

        float radius =
            Mathf.Max(
                bounds.extents.magnitude,
                0.05f
            );

        previewCamera.orthographicSize =
            radius * padding;

        previewCamera.nearClipPlane = 0.01f;
        previewCamera.farClipPlane =
            Mathf.Max(
                100f,
                cameraDistance + radius * 4f
            );
    }

    private void ClearPreviewObject()
    {
        if (currentPreviewObject != null)
            Destroy(currentPreviewObject);

        currentPreviewObject = null;
        currentItem = null;
    }

    private void SetVisible(bool visible)
    {
        if (previewRawImage != null)
            previewRawImage.enabled = visible;

        if (previewCamera != null)
            previewCamera.enabled = visible;

        if (previewLight != null)
            previewLight.enabled = visible;
    }
}
