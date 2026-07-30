using System.Collections;
using UnityEngine;

/// <summary>
/// 인벤토리의 시각적 연출만 담당합니다.
/// 데이터 조회나 선택 상태 변경은 하지 않습니다.
/// </summary>
public class InventoryUIEffect : MonoBehaviour
{
    [Header("창 연출")]
    [SerializeField] private CanvasGroup windowCanvasGroup;
    [SerializeField] private RectTransform windowPanel;

    [SerializeField, Min(0f)]
    private float windowDuration = 0.15f;

    [SerializeField]
    private Vector3 closedScale =
        new Vector3(0.96f, 0.96f, 1f);

    [Header("슬롯 연출")]
    [SerializeField, Min(0f)]
    private float slotPulseDuration = 0.1f;

    [SerializeField, Min(1f)]
    private float slotPulseScale = 1.04f;

    [Header("프리뷰 연출")]
    [SerializeField] private CanvasGroup previewCanvasGroup;

    [SerializeField, Min(0f)]
    private float previewFadeDuration = 0.1f;

    private Coroutine windowCoroutine;
    private Coroutine previewCoroutine;

    public void SetImmediate(
        GameObject inventoryRoot,
        bool visible)
    {
        if (windowCoroutine != null)
        {
            StopCoroutine(windowCoroutine);
            windowCoroutine = null;
        }

        if (inventoryRoot != null)
            inventoryRoot.SetActive(visible);

        if (windowCanvasGroup != null)
        {
            windowCanvasGroup.alpha =
                visible ? 1f : 0f;

            windowCanvasGroup.interactable =
                visible;

            windowCanvasGroup.blocksRaycasts =
                visible;
        }

        if (windowPanel != null)
        {
            windowPanel.localScale =
                visible
                    ? Vector3.one
                    : closedScale;
        }
    }

    public void PlayOpen(GameObject inventoryRoot)
    {
        if (windowCoroutine != null)
            StopCoroutine(windowCoroutine);

        if (inventoryRoot != null)
            inventoryRoot.SetActive(true);

        windowCoroutine =
            StartCoroutine(
                WindowRoutine(
                    inventoryRoot,
                    true
                )
            );
    }

    public void PlayClose(GameObject inventoryRoot)
    {
        if (windowCoroutine != null)
            StopCoroutine(windowCoroutine);

        windowCoroutine =
            StartCoroutine(
                WindowRoutine(
                    inventoryRoot,
                    false
                )
            );
    }

    public void PlaySlotSelected(
        RectTransform slot)
    {
        if (slot == null)
            return;

        StartCoroutine(
            SlotPulseRoutine(
                slot,
                slotPulseScale
            )
        );
    }

    public void PlayEquippedChanged(
        RectTransform slot)
    {
        if (slot == null)
            return;

        // 현재는 선택 효과와 동일한 틀을 사용합니다.
        // 이후 색상, 흔들림, 강조 효과를 별도로 추가할 수 있습니다.
        StartCoroutine(
            SlotPulseRoutine(
                slot,
                slotPulseScale
            )
        );
    }

    public void PlayPreviewChanged()
    {
        if (previewCanvasGroup == null)
            return;

        if (previewCoroutine != null)
            StopCoroutine(previewCoroutine);

        previewCoroutine =
            StartCoroutine(
                PreviewFadeRoutine()
            );
    }

    public void PlayInventoryFull()
    {
        /*
         * 나중에 다음 연출을 넣을 위치입니다.
         *
         * - 전체 슬롯 흔들기
         * - "인벤토리가 가득 찼습니다" 메시지
         * - 빨간색 점멸
         */
        Debug.Log("InventoryUIEffect: 인벤토리 가득 참 연출");
    }

    private IEnumerator WindowRoutine(
        GameObject inventoryRoot,
        bool opening)
    {
        float startAlpha =
            windowCanvasGroup != null
                ? windowCanvasGroup.alpha
                : opening ? 0f : 1f;

        float targetAlpha =
            opening ? 1f : 0f;

        Vector3 startScale =
            windowPanel != null
                ? windowPanel.localScale
                : Vector3.one;

        Vector3 targetScale =
            opening
                ? Vector3.one
                : closedScale;

        if (windowCanvasGroup != null)
        {
            windowCanvasGroup.interactable = false;
            windowCanvasGroup.blocksRaycasts = false;
        }

        if (windowDuration <= 0f)
        {
            ApplyWindowState(
                inventoryRoot,
                opening,
                targetAlpha,
                targetScale
            );

            yield break;
        }

        float elapsed = 0f;

        while (elapsed < windowDuration)
        {
            elapsed += Time.unscaledDeltaTime;

            float t = Mathf.Clamp01(
                elapsed / windowDuration
            );

            t = Mathf.SmoothStep(0f, 1f, t);

            if (windowCanvasGroup != null)
            {
                windowCanvasGroup.alpha =
                    Mathf.Lerp(
                        startAlpha,
                        targetAlpha,
                        t
                    );
            }

            if (windowPanel != null)
            {
                windowPanel.localScale =
                    Vector3.Lerp(
                        startScale,
                        targetScale,
                        t
                    );
            }

            yield return null;
        }

        ApplyWindowState(
            inventoryRoot,
            opening,
            targetAlpha,
            targetScale
        );
    }

    private void ApplyWindowState(
        GameObject inventoryRoot,
        bool visible,
        float alpha,
        Vector3 scale)
    {
        if (windowCanvasGroup != null)
        {
            windowCanvasGroup.alpha = alpha;
            windowCanvasGroup.interactable = visible;
            windowCanvasGroup.blocksRaycasts = visible;
        }

        if (windowPanel != null)
            windowPanel.localScale = scale;

        if (!visible && inventoryRoot != null)
            inventoryRoot.SetActive(false);

        windowCoroutine = null;
    }

    private IEnumerator SlotPulseRoutine(
        RectTransform slot,
        float targetScaleValue)
    {
        Vector3 originalScale =
            slot.localScale;

        Vector3 targetScale =
            originalScale * targetScaleValue;

        float halfDuration =
            Mathf.Max(
                0.01f,
                slotPulseDuration * 0.5f
            );

        float elapsed = 0f;

        while (elapsed < halfDuration)
        {
            elapsed += Time.unscaledDeltaTime;

            float t = Mathf.Clamp01(
                elapsed / halfDuration
            );

            slot.localScale =
                Vector3.Lerp(
                    originalScale,
                    targetScale,
                    t
                );

            yield return null;
        }

        elapsed = 0f;

        while (elapsed < halfDuration)
        {
            elapsed += Time.unscaledDeltaTime;

            float t = Mathf.Clamp01(
                elapsed / halfDuration
            );

            slot.localScale =
                Vector3.Lerp(
                    targetScale,
                    originalScale,
                    t
                );

            yield return null;
        }

        slot.localScale = originalScale;
    }

    private IEnumerator PreviewFadeRoutine()
    {
        previewCanvasGroup.alpha = 0f;

        if (previewFadeDuration <= 0f)
        {
            previewCanvasGroup.alpha = 1f;
            previewCoroutine = null;
            yield break;
        }

        float elapsed = 0f;

        while (elapsed < previewFadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;

            previewCanvasGroup.alpha =
                Mathf.Clamp01(
                    elapsed /
                    previewFadeDuration
                );

            yield return null;
        }

        previewCanvasGroup.alpha = 1f;
        previewCoroutine = null;
    }
}