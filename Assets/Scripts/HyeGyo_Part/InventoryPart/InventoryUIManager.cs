using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[Serializable]
public class InventorySlotView
{
    [SerializeField] private RectTransform root;
    [SerializeField] private Button button;
    [SerializeField] private TMP_Text itemNameText;

    [Header("상태 표시")]
    [SerializeField] private GameObject selectedFrame;
    [SerializeField] private GameObject equippedFrame;

    public RectTransform Root
    {
        get
        {
            if (root != null)
                return root;

            if (button != null)
                return button.transform as RectTransform;

            return null;
        }
    }

    public void Bind(int slotIndex, Action<int> onClicked)
    {
        if (button == null)
            return;

        button.onClick.AddListener(
            () => onClicked?.Invoke(slotIndex)
        );
    }

    public void Refresh(
        string itemName,
        bool isSelected,
        bool isEquipped)
    {
        if (itemNameText != null)
        {
            itemNameText.text =
                string.IsNullOrWhiteSpace(itemName)
                    ? "—"
                    : itemName;
        }

        if (selectedFrame != null)
            selectedFrame.SetActive(isSelected);

        if (equippedFrame != null)
            equippedFrame.SetActive(isEquipped);
    }
}

[Serializable]
public class GrabbableUnityEvent :
    UnityEvent<Object_Grabbable>
{
}

/// <summary>
/// 인벤토리의 기능적 UI 처리를 담당합니다.
/// 애니메이션은 InventoryUIEffect에 위임합니다.
/// </summary>
public class InventoryUIManager : MonoBehaviour
{
    [Header("데이터")]
    [SerializeField] private InventoryData inventoryData;

    [Header("인벤토리 창")]
    [SerializeField] private GameObject inventoryRoot;
    [SerializeField] private bool startClosed = true;

    [Header("입력")]
    [SerializeField] private KeyCode openKey = KeyCode.I;
    [SerializeField] private KeyCode confirmKey = KeyCode.E;
    [SerializeField] private KeyCode closeKey = KeyCode.Escape;

    [Header("슬롯 8개")]
    [SerializeField] private InventorySlotView[] slots;

    [Header("선택된 아이템 정보")]
    [SerializeField] private TMP_Text selectedNameText;
    [SerializeField] private TMP_Text selectedDescriptionText;

    [Header("3D 프리뷰 UI")]
    [SerializeField] private RawImage previewRawImage;

    [Header("3D 프리뷰 씬")]
    [SerializeField] private Camera previewCamera;
    [SerializeField] private Transform previewRoot;
    [SerializeField] private Light previewLight;

    [Header("프리뷰 렌더 설정")]
    [SerializeField] private RenderTexture previewTexture;
    [SerializeField] private int runtimeTextureSize = 512;
    [SerializeField] private string previewLayerName =
        "InventoryPreview";

    [Header("프리뷰 모델 설정")]
    [SerializeField] private Vector3 previewRotation =
        new Vector3(15f, 30f, 0f);

    [SerializeField] private float previewCameraDistance = 10f;
    [SerializeField] private float previewPadding = 1.3f;

    [SerializeField] private bool autoRotatePreview = true;
    [SerializeField] private float previewRotationSpeed = 20f;

    [Header("인벤토리 활성 중")]
    [SerializeField] private bool pauseGameWhileOpen;

    [Tooltip(
        "인벤토리 중 비활성화할 이동, 시점, Ray 입력 스크립트"
    )]
    [SerializeField]
    private MonoBehaviour[] disableWhileOpen;

    [Header("UI 연출")]
    [SerializeField] private InventoryUIEffect uiEffect;

    [Header("장착 요청")]
    [Tooltip(
        "선택 아이템을 실제 손으로 옮기는 코드와 나중에 연결합니다. " +
        "빈 슬롯이면 null이 전달됩니다."
    )]
    [SerializeField]
    private GrabbableUnityEvent onEquipRequested;

    private GameObject currentPreviewObject;
    private InventoryItemData currentPreviewItem;

    private RenderTexture runtimeRenderTexture;
    private bool ownsRuntimeRenderTexture;

    private bool isOpen;

    private int lastSelectedIndex = -999;
    private int lastEquippedIndex = -999;

    private float previousTimeScale;
    private CursorLockMode previousCursorLock;
    private bool previousCursorVisible;

    private bool[] previousBehaviourStates;

    public bool IsOpen => isOpen;

    private void Awake()
    {
        BindSlotButtons();
        ConfigurePreviewOutput();

        isOpen = !startClosed;

        if (uiEffect != null)
        {
            uiEffect.SetImmediate(
                inventoryRoot,
                isOpen
            );
        }
        else if (inventoryRoot != null)
        {
            inventoryRoot.SetActive(isOpen);
        }
    }

    private void OnEnable()
    {
        if (inventoryData != null)
        {
            inventoryData.OnChanged += HandleInventoryChanged;
            inventoryData.OnInventoryFull += HandleInventoryFull;
        }
    }

    private void Start()
    {
        RefreshAll();
    }

    private void OnDisable()
    {
        if (inventoryData != null)
        {
            inventoryData.OnChanged -= HandleInventoryChanged;
            inventoryData.OnInventoryFull -= HandleInventoryFull;
        }
    }

    private void OnDestroy()
    {
        if (isOpen)
            RestoreGameplayState();

        if (ownsRuntimeRenderTexture &&
            runtimeRenderTexture != null)
        {
            runtimeRenderTexture.Release();
            Destroy(runtimeRenderTexture);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(openKey))
        {
            ToggleInventory();
            return;
        }

        if (!isOpen)
            return;

        if (Input.GetKeyDown(closeKey))
        {
            CloseInventory();
            return;
        }

        if (Input.GetKeyDown(confirmKey))
        {
            ConfirmSelectedItem();
        }

        if (autoRotatePreview &&
            currentPreviewObject != null)
        {
            currentPreviewObject.transform.Rotate(
                previewRoot != null
                    ? previewRoot.up
                    : Vector3.up,
                previewRotationSpeed *
                Time.unscaledDeltaTime,
                Space.World
            );
        }
    }

    private void BindSlotButtons()
    {
        if (slots == null)
            return;

        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] == null)
                continue;

            slots[i].Bind(i, SelectSlot);
        }
    }

    public void ToggleInventory()
    {
        if (isOpen)
            CloseInventory();
        else
            OpenInventory();
    }

    public void OpenInventory()
    {
        if (isOpen)
            return;

        isOpen = true;

        SaveAndDisableGameplayState();

        if (uiEffect != null)
            uiEffect.PlayOpen(inventoryRoot);
        else if (inventoryRoot != null)
            inventoryRoot.SetActive(true);

        RefreshAll();
        SetPreviewCameraEnabled(currentPreviewObject != null);
    }

    public void CloseInventory()
    {
        if (!isOpen)
            return;

        isOpen = false;

        if (uiEffect != null)
            uiEffect.PlayClose(inventoryRoot);
        else if (inventoryRoot != null)
            inventoryRoot.SetActive(false);

        SetPreviewCameraEnabled(false);
        RestoreGameplayState();
    }

    private void SaveAndDisableGameplayState()
    {
        previousCursorLock = Cursor.lockState;
        previousCursorVisible = Cursor.visible;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (pauseGameWhileOpen)
        {
            previousTimeScale = Time.timeScale;
            Time.timeScale = 0f;
        }

        if (disableWhileOpen == null)
            return;

        previousBehaviourStates =
            new bool[disableWhileOpen.Length];

        for (int i = 0;
             i < disableWhileOpen.Length;
             i++)
        {
            MonoBehaviour behaviour =
                disableWhileOpen[i];

            if (behaviour == null)
                continue;

            previousBehaviourStates[i] =
                behaviour.enabled;

            behaviour.enabled = false;
        }
    }

    private void RestoreGameplayState()
    {
        Cursor.lockState = previousCursorLock;
        Cursor.visible = previousCursorVisible;

        if (pauseGameWhileOpen)
            Time.timeScale = previousTimeScale;

        if (disableWhileOpen == null ||
            previousBehaviourStates == null)
        {
            return;
        }

        int count = Mathf.Min(
            disableWhileOpen.Length,
            previousBehaviourStates.Length
        );

        for (int i = 0; i < count; i++)
        {
            MonoBehaviour behaviour =
                disableWhileOpen[i];

            if (behaviour != null)
            {
                behaviour.enabled =
                    previousBehaviourStates[i];
            }
        }
    }

    public void SelectSlot(int slotIndex)
    {
        if (inventoryData == null)
            return;

        inventoryData.SelectSlot(slotIndex);
    }

    /// <summary>
    /// 슬롯의 E키 장착 요청입니다.
    /// 실제 교체 연출은 Player_Grab 또는 별도 장착 코드와 연결합니다.
    /// </summary>
    public void ConfirmSelectedItem()
    {
        Object_Grabbable selectedObject = null;

        if (inventoryData != null &&
            inventoryData.SelectedItem != null)
        {
            selectedObject =
                inventoryData.SelectedItem.SourceObject;
        }

        onEquipRequested?.Invoke(selectedObject);
    }

    private void HandleInventoryChanged()
    {
        RefreshAll();
    }

    private void HandleInventoryFull()
    {
        Debug.Log("인벤토리가 가득 찼습니다.");

        if (uiEffect != null)
            uiEffect.PlayInventoryFull();
    }

    private void RefreshAll()
    {
        RefreshSlots();
        RefreshSelectedInformation();
        RefreshStateEffects();
    }

    private void RefreshSlots()
    {
        if (slots == null || inventoryData == null)
            return;

        for (int i = 0; i < slots.Length; i++)
        {
            InventoryItemData item =
                inventoryData.GetItemAt(i);

            string itemName =
                item != null
                    ? item.ItemName
                    : "—";

            bool isSelected =
                i == inventoryData.SelectedIndex;

            bool isEquipped =
                i == inventoryData.EquippedIndex;

            slots[i]?.Refresh(
                itemName,
                isSelected,
                isEquipped
            );
        }
    }

    private void RefreshSelectedInformation()
    {
        if (inventoryData == null)
        {
            ClearSelectedInformation();
            return;
        }

        InventoryItemData selectedItem =
            inventoryData.SelectedItem;

        if (selectedItem == null)
        {
            ClearSelectedInformation();
            return;
        }

        if (selectedNameText != null)
            selectedNameText.text = selectedItem.ItemName;

        if (selectedDescriptionText != null)
        {
            selectedDescriptionText.text =
                selectedItem.Description;
        }

        /*
         * 창이 닫혀 있을 때는 3D 오브젝트를 만들지 않습니다.
         * 다음에 열 때 다시 생성합니다.
         */
        if (!isOpen)
        {
            ClearPreview();
            return;
        }

        if (currentPreviewItem != selectedItem ||
            currentPreviewObject == null)
        {
            BuildPreview(selectedItem);
        }
    }

    private void ClearSelectedInformation()
    {
        if (selectedNameText != null)
            selectedNameText.text = "—";

        if (selectedDescriptionText != null)
            selectedDescriptionText.text = string.Empty;

        ClearPreview();
    }

    private void RefreshStateEffects()
    {
        if (inventoryData == null)
            return;

        int selectedIndex =
            inventoryData.SelectedIndex;

        int equippedIndex =
            inventoryData.EquippedIndex;

        if (isOpen &&
            selectedIndex != lastSelectedIndex &&
            IsValidSlot(selectedIndex))
        {
            uiEffect?.PlaySlotSelected(
                slots[selectedIndex].Root
            );
        }

        if (isOpen &&
            equippedIndex != lastEquippedIndex &&
            IsValidSlot(equippedIndex))
        {
            uiEffect?.PlayEquippedChanged(
                slots[equippedIndex].Root
            );
        }

        lastSelectedIndex = selectedIndex;
        lastEquippedIndex = equippedIndex;
    }

    private bool IsValidSlot(int index)
    {
        return slots != null &&
               index >= 0 &&
               index < slots.Length &&
               slots[index] != null;
    }

    private void ConfigurePreviewOutput()
    {
        if (previewCamera == null ||
            previewRawImage == null)
        {
            return;
        }

        if (previewTexture == null)
        {
            int size = Mathf.Max(
                64,
                runtimeTextureSize
            );

            runtimeRenderTexture =
                new RenderTexture(
                    size,
                    size,
                    24,
                    RenderTextureFormat.ARGB32
                );

            runtimeRenderTexture.name =
                "RT_InventoryPreview_Runtime";

            runtimeRenderTexture.Create();

            previewTexture = runtimeRenderTexture;
            ownsRuntimeRenderTexture = true;
        }

        previewCamera.targetTexture = previewTexture;
        previewCamera.orthographic = true;
        previewCamera.clearFlags =
            CameraClearFlags.SolidColor;

        previewCamera.backgroundColor =
            new Color(0f, 0f, 0f, 0f);

        previewRawImage.texture = previewTexture;

        // 프리뷰 RawImage가 클릭을 가로채지 않습니다.
        previewRawImage.raycastTarget = false;
        previewRawImage.enabled = false;

        int previewLayer =
            LayerMask.NameToLayer(previewLayerName);

        if (previewLayer >= 0)
        {
            previewCamera.cullingMask =
                1 << previewLayer;
        }
        else
        {
            Debug.LogWarning(
                $"'{previewLayerName}' 레이어가 없습니다."
            );
        }

        previewCamera.enabled = false;
    }

    private void BuildPreview(
        InventoryItemData item)
    {
        ClearPreview();

        if (item == null ||
            previewRoot == null ||
            item.MeshParts.Count == 0)
        {
            return;
        }

        int previewLayer =
            LayerMask.NameToLayer(previewLayerName);

        /*
         * Container는 회전축 역할을 합니다.
         * Content는 Mesh의 중심을 Container 중앙에 맞추는 역할입니다.
         */
        currentPreviewObject =
            new GameObject(
                $"{item.ItemName}_PreviewContainer"
            );

        currentPreviewObject.transform.SetParent(
            previewRoot,
            false
        );

        currentPreviewObject.transform.localPosition =
            Vector3.zero;

        currentPreviewObject.transform.localRotation =
            Quaternion.Euler(previewRotation);

        currentPreviewObject.transform.localScale =
            Vector3.one;

        SetLayer(
            currentPreviewObject,
            previewLayer
        );

        GameObject contentObject =
            new GameObject("Content");

        contentObject.transform.SetParent(
            currentPreviewObject.transform,
            false
        );

        SetLayer(contentObject, previewLayer);

        for (int i = 0;
             i < item.MeshParts.Count;
             i++)
        {
            InventoryMeshPartData part =
                item.MeshParts[i];

            if (part == null || part.Mesh == null)
                continue;

            GameObject meshObject =
                new GameObject($"MeshPart_{i}");

            meshObject.transform.SetParent(
                contentObject.transform,
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

            SetLayer(meshObject, previewLayer);
        }

        CenterPreviewContent(
            contentObject.transform
        );

        FitPreviewCamera();

        currentPreviewItem = item;

        if (previewRawImage != null)
            previewRawImage.enabled = true;

        SetPreviewCameraEnabled(isOpen);

        uiEffect?.PlayPreviewChanged();
    }

    private void CenterPreviewContent(
        Transform content)
    {
        Renderer[] renderers =
            content.GetComponentsInChildren<Renderer>();

        if (renderers.Length == 0)
            return;

        Bounds bounds = renderers[0].bounds;

        for (int i = 1;
             i < renderers.Length;
             i++)
        {
            bounds.Encapsulate(
                renderers[i].bounds
            );
        }

        /*
         * Mesh 전체의 Bounds 중앙이 PreviewRoot 위치에 오도록
         * Content만 이동합니다.
         */
        Vector3 targetCenter =
            currentPreviewObject.transform.position;

        content.position +=
            targetCenter - bounds.center;
    }

    private void FitPreviewCamera()
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

        for (int i = 1;
             i < renderers.Length;
             i++)
        {
            bounds.Encapsulate(
                renderers[i].bounds
            );
        }

        Vector3 cameraForward =
            previewRoot.forward;

        previewCamera.transform.position =
            previewRoot.position -
            cameraForward * previewCameraDistance;

        previewCamera.transform.rotation =
            Quaternion.LookRotation(
                cameraForward,
                previewRoot.up
            );

        float radius =
            Mathf.Max(
                bounds.extents.magnitude,
                0.05f
            );

        previewCamera.orthographicSize =
            radius * Mathf.Max(1f, previewPadding);

        previewCamera.nearClipPlane = 0.01f;

        previewCamera.farClipPlane =
            Mathf.Max(
                100f,
                previewCameraDistance +
                radius * 4f
            );
    }

    private void ClearPreview()
    {
        if (currentPreviewObject != null)
        {
            Destroy(currentPreviewObject);
            currentPreviewObject = null;
        }

        currentPreviewItem = null;

        if (previewRawImage != null)
            previewRawImage.enabled = false;

        SetPreviewCameraEnabled(false);
    }

    private void SetPreviewCameraEnabled(bool enabled)
    {
        if (previewCamera != null)
            previewCamera.enabled = enabled;

        if (previewLight != null)
            previewLight.enabled = enabled;
    }

    private void SetLayer(
        GameObject target,
        int layer)
    {
        if (target == null || layer < 0)
            return;

        target.layer = layer;
    }
}