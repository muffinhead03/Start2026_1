using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public sealed class InventoryObjectPreview : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private Camera previewCamera;

    [SerializeField]
    private RawImage previewRawImage;

    [SerializeField]
    private Transform previewRoot;

    [Header("Optional Root Detection")]
    [SerializeField]
    private InventoryData inventoryData;

    [SerializeField]
    private PerceiveObjectHandPivot handPerception;

    [Header("Render Texture")]
    [SerializeField]
    private int textureSize = 512;

    [Header("Preview")]
    [SerializeField]
    private string previewLayerName =
        "InventoryPreview";

    [SerializeField]
    private float targetSize = 1.2f;

    [SerializeField]
    private Vector3 previewRotation =
        new Vector3(
            15f,
            -25f,
            0f
        );

    private RenderTexture previewTexture;

    private GameObject currentPreview;


    private void Awake()
    {
        AutoResolveReferences();

        SetupPreviewCamera();
    }


    private void OnDestroy()
    {
        Clear();

        ReleaseRenderTexture();
    }


    private void AutoResolveReferences()
    {
        /*
         * PreviewRig이 InventorySystem의 자식이라는
         * 현재 구조를 기준으로 자동 검색합니다.
         */

        Transform parent =
            transform.parent;

        if (parent == null)
        {
            return;
        }

        if (inventoryData == null)
        {
            inventoryData =
                parent.GetComponentInChildren
                    <InventoryData>(true);
        }

        if (handPerception == null)
        {
            handPerception =
                parent.GetComponentInChildren
                    <PerceiveObjectHandPivot>(true);
        }
    }


    private void SetupPreviewCamera()
    {
        if (previewCamera == null)
        {
            Debug.LogError(
                "[InventoryObjectPreview] " +
                "PreviewCamera가 연결되지 않았습니다.",
                this
            );

            return;
        }

        if (previewRawImage == null)
        {
            Debug.LogError(
                "[InventoryObjectPreview] " +
                "PreviewRawImage가 연결되지 않았습니다.",
                this
            );

            return;
        }

        /*
         * RenderTexture 연결 전에는
         * Display 1을 렌더하지 않도록 끕니다.
         */
        previewCamera.enabled = false;

        previewTexture =
            new RenderTexture(
                textureSize,
                textureSize,
                24,
                RenderTextureFormat.ARGB32
            );

        previewTexture.name =
            "InventoryPreview_Runtime";

        previewTexture.Create();

        /*
         * 투명 배경.
         */
        previewCamera.clearFlags =
            CameraClearFlags.SolidColor;

        previewCamera.backgroundColor =
            new Color(
                0f,
                0f,
                0f,
                0f
            );

        previewCamera.targetTexture =
            previewTexture;

        previewRawImage.texture =
            previewTexture;

        previewCamera.enabled = true;
    }


    /// <summary>
    /// 선택된 실제 아이템을 복사해서
    /// PreviewRoot 아래에 표시합니다.
    /// </summary>
    public void Show(
        Object_Grabbable sourceObject)
    {
        Clear();

        if (sourceObject == null)
        {
            return;
        }

        if (previewRoot == null)
        {
            Debug.LogError(
                "[InventoryObjectPreview] " +
                "PreviewRoot가 연결되지 않았습니다.",
                this
            );

            return;
        }

        Transform itemRoot =
            ResolveItemRoot(
                sourceObject
            );

        if (itemRoot == null)
        {
            return;
        }

        /*
         * 임의의 Gameplay Script가 복제되자마자
         * OnEnable 되는 것을 최대한 막기 위해
         * PreviewRoot를 잠시 비활성화합니다.
         */
        bool previewRootWasActive =
            previewRoot.gameObject.activeSelf;

        previewRoot.gameObject.SetActive(
            false
        );

        currentPreview =
            Instantiate(
                itemRoot.gameObject,
                previewRoot
            );

        currentPreview.name =
            "Preview_" +
            itemRoot.gameObject.name;

        Transform previewTransform =
            currentPreview.transform;

        previewTransform.localPosition =
            Vector3.zero;

        previewTransform.localRotation =
            Quaternion.Euler(
                previewRotation
            );

        previewTransform.localScale =
            Vector3.one;

        /*
         * 먼저 게임 기능을 제거합니다.
         */
        DisableGameplayComponents(
            currentPreview
        );

        /*
         * PreviewCamera만 볼 수 있는 Layer로 변경.
         */
        SetPreviewLayer(
            currentPreview
        );

        /*
         * 복제품 자체는 활성 상태로 만듭니다.
         * 부모 PreviewRoot가 아직 꺼져 있으므로
         * 현재 시점에서는 화면에 나오지 않습니다.
         */
        currentPreview.SetActive(
            true
        );

        previewRoot.gameObject.SetActive(
            previewRootWasActive
        );

        /*
         * Renderer Bounds를 이용해
         * 중앙 정렬 및 크기 통일.
         */
        CenterAndResize(
            currentPreview
        );
    }


    public void Clear()
    {
        if (currentPreview == null)
        {
            return;
        }

        currentPreview.SetActive(
            false
        );

        if (Application.isPlaying)
        {
            Destroy(
                currentPreview
            );
        }
        else
        {
            DestroyImmediate(
                currentPreview
            );
        }

        currentPreview = null;
    }


    /// <summary>
    /// Object_Grabbable이 아이템 내부 자식에 붙어 있어도
    /// 가능한 경우 실제 아이템 Root를 찾아냅니다.
    /// </summary>
    private Transform ResolveItemRoot(
        Object_Grabbable sourceObject)
    {
        Transform sourceTransform =
            sourceObject.transform;

        /*
         * InventoryData 아래에 보관 중인 경우.
         */
        if (inventoryData != null)
        {
            Transform inventoryRoot =
                FindDirectChildRoot(
                    inventoryData.transform,
                    sourceTransform
                );

            if (inventoryRoot != null)
            {
                return inventoryRoot;
            }
        }

        /*
         * 현재 HandPivot에 장착되어 있는 경우.
         */
        if (handPerception != null &&
            handPerception.HandPivot != null)
        {
            Transform handRoot =
                FindDirectChildRoot(
                    handPerception.HandPivot,
                    sourceTransform
                );

            if (handRoot != null)
            {
                return handRoot;
            }
        }

        /*
         * 별도 Root를 찾지 못하면
         * Object_Grabbable이 붙은 GameObject 자체를 사용.
         */
        return sourceTransform;
    }


    private static Transform FindDirectChildRoot(
        Transform parent,
        Transform descendant)
    {
        if (parent == null ||
            descendant == null ||
            descendant == parent ||
            !descendant.IsChildOf(parent))
        {
            return null;
        }

        Transform current =
            descendant;

        while (current.parent != null &&
               current.parent != parent)
        {
            current =
                current.parent;
        }

        return current.parent == parent
            ? current
            : null;
    }


    private static void DisableGameplayComponents(
        GameObject root)
    {
        /*
         * Collider
         */
        Collider[] colliders =
            root.GetComponentsInChildren
                <Collider>(true);

        for (int i = 0;
             i < colliders.Length;
             i++)
        {
            colliders[i].enabled =
                false;
        }

        /*
         * Rigidbody
         */
        Rigidbody[] rigidbodies =
            root.GetComponentsInChildren
                <Rigidbody>(true);

        for (int i = 0;
             i < rigidbodies.Length;
             i++)
        {
            rigidbodies[i].isKinematic =
                true;

            rigidbodies[i].detectCollisions =
                false;
        }

        /*
         * 복사된 아이템의 Gameplay Script는
         * 프리뷰에서 실행하지 않습니다.
         */
        MonoBehaviour[] behaviours =
            root.GetComponentsInChildren
                <MonoBehaviour>(true);

        for (int i = 0;
             i < behaviours.Length;
             i++)
        {
            behaviours[i].enabled =
                false;
        }

        /*
         * 복사된 Camera가 있다면 끕니다.
         */
        Camera[] cameras =
            root.GetComponentsInChildren
                <Camera>(true);

        for (int i = 0;
             i < cameras.Length;
             i++)
        {
            cameras[i].enabled =
                false;
        }

        /*
         * 아이템 자체 Light가 Preview 조명에
         * 영향을 주지 않게 합니다.
         */
        Light[] lights =
            root.GetComponentsInChildren
                <Light>(true);

        for (int i = 0;
             i < lights.Length;
             i++)
        {
            lights[i].enabled =
                false;
        }

        /*
         * Audio도 차단.
         */
        AudioSource[] audioSources =
            root.GetComponentsInChildren
                <AudioSource>(true);

        for (int i = 0;
             i < audioSources.Length;
             i++)
        {
            audioSources[i].enabled =
                false;
        }

        /*
         * Animator는 정지된 Preview로 사용.
         */
        Animator[] animators =
            root.GetComponentsInChildren
                <Animator>(true);

        for (int i = 0;
             i < animators.Length;
             i++)
        {
            animators[i].enabled =
                false;
        }

        /*
         * Particle도 중지.
         */
        ParticleSystem[] particles =
            root.GetComponentsInChildren
                <ParticleSystem>(true);

        for (int i = 0;
             i < particles.Length;
             i++)
        {
            particles[i].Stop(
                true,
                ParticleSystemStopBehavior
                    .StopEmittingAndClear
            );
        }
    }


    private void SetPreviewLayer(
        GameObject root)
    {
        int layer =
            LayerMask.NameToLayer(
                previewLayerName
            );

        if (layer < 0)
        {
            Debug.LogError(
                "[InventoryObjectPreview] " +
                $"'{previewLayerName}' Layer가 없습니다.",
                this
            );

            return;
        }

        SetLayerRecursive(
            root.transform,
            layer
        );
    }


    private static void SetLayerRecursive(
        Transform target,
        int layer)
    {
        if (target == null)
        {
            return;
        }

        target.gameObject.layer =
            layer;

        for (int i = 0;
             i < target.childCount;
             i++)
        {
            SetLayerRecursive(
                target.GetChild(i),
                layer
            );
        }
    }


    private void CenterAndResize(
        GameObject root)
    {
        Renderer[] renderers =
            root.GetComponentsInChildren
                <Renderer>(true);

        Bounds bounds = default;

        bool hasBounds =
            false;

        for (int i = 0;
             i < renderers.Length;
             i++)
        {
            Renderer renderer =
                renderers[i];

            if (renderer == null ||
                !IsGeometryRenderer(renderer))
            {
                continue;
            }

            if (!hasBounds)
            {
                bounds =
                    renderer.bounds;

                hasBounds =
                    true;
            }
            else
            {
                bounds.Encapsulate(
                    renderer.bounds
                );
            }
        }

        if (!hasBounds)
        {
            Debug.LogWarning(
                "[InventoryObjectPreview] " +
                "표시할 Renderer를 찾지 못했습니다.",
                root
            );

            return;
        }

        float largestSize =
            Mathf.Max(
                bounds.size.x,
                bounds.size.y,
                bounds.size.z
            );

        if (largestSize <= 0.0001f)
        {
            return;
        }

        /*
         * 모든 아이템의 가장 긴 축을
         * targetSize로 맞춥니다.
         */
        float scale =
            targetSize /
            largestSize;

        root.transform.localScale *=
            scale;

        /*
         * Scale 변경 이후 Bounds를 다시 계산합니다.
         */
        renderers =
            root.GetComponentsInChildren
                <Renderer>(true);

        hasBounds =
            false;

        for (int i = 0;
             i < renderers.Length;
             i++)
        {
            Renderer renderer =
                renderers[i];

            if (renderer == null ||
                !IsGeometryRenderer(renderer))
            {
                continue;
            }

            if (!hasBounds)
            {
                bounds =
                    renderer.bounds;

                hasBounds =
                    true;
            }
            else
            {
                bounds.Encapsulate(
                    renderer.bounds
                );
            }
        }

        if (!hasBounds)
        {
            return;
        }

        /*
         * 물체 Bounds 중심을 PreviewRoot 위치와 맞춤.
         */
        Vector3 offset =
            previewRoot.position -
            bounds.center;

        root.transform.position +=
            offset;
    }


    private static bool IsGeometryRenderer(
        Renderer renderer)
    {
        return renderer is MeshRenderer ||
               renderer is SkinnedMeshRenderer;
    }


    private void ReleaseRenderTexture()
    {
        if (previewCamera != null)
        {
            previewCamera.targetTexture =
                null;
        }

        if (previewRawImage != null)
        {
            previewRawImage.texture =
                null;
        }

        if (previewTexture == null)
        {
            return;
        }

        previewTexture.Release();

        Destroy(
            previewTexture
        );

        previewTexture = null;
    }
}