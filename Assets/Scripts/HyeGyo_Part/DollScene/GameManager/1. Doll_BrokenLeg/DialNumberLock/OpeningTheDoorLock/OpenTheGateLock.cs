using System.Collections;
using UnityEngine;

public class OpenTheGateLock : MonoBehaviour
{
    // ================================================
    // Lock Curve
    // ================================================

    [Header("Lock Curve")]
    [Tooltip("위로 들어올릴 Lock_Curve")]
    [SerializeField]
    private Transform lockCurve;


    // ================================================
    // 비활성화 대상
    // ================================================

    [Header("Disable Object")]

    [Tooltip("정답 시 완전히 비활성화할 Door_Curv 오브젝트")]
    [SerializeField]
    private GameObject doorCurveToDisable;


    // ================================================
    // Open Animation
    // ================================================

    [Header("Open Animation")]

    [Tooltip("자물쇠 고리가 위로 올라가는 거리")]
    [SerializeField]
    private float liftHeight = 0.2f;


    [Tooltip("자물쇠 고리가 올라가는 시간")]
    [SerializeField]
    private float liftDuration = 1f;


    // ================================================
    // State
    // ================================================

    [Header("State")]

    [SerializeField]
    private bool isOpened = false;


    public bool IsOpened => isOpened;


    // ================================================
    // 자물쇠 열기
    // ================================================

    public void OpenLock()
    {
        Debug.Log(
            "[OpenTheGateLock] OpenLock() 호출됨"
        );


        if (isOpened)
            return;


        if (lockCurve == null)
        {
            Debug.LogWarning(
                "[OpenTheGateLock] Lock_Curve가 연결되지 않았습니다."
            );

            return;
        }


        isOpened = true;


        // ============================================
        // ★ 제일 먼저 Door_Curv 완전 비활성화
        // ============================================

        DisableDoorCurve();


        // 그 다음 고리 열기
        StartCoroutine(
            OpenLockRoutine()
        );
    }


    // ================================================
    // Door_Curv 완전 비활성화
    // ================================================

    private void DisableDoorCurve()
    {
        if (doorCurveToDisable == null)
        {
            Debug.LogWarning(
                "[OpenTheGateLock] Door_Curv가 연결되지 않았습니다."
            );

            return;
        }


        Debug.Log(
            $"[OpenTheGateLock] 비활성화 대상 = " +
            $"{doorCurveToDisable.name}"
        );


        // --------------------------------------------
        // 해당 오브젝트 + 모든 자식 Collider 비활성화
        // --------------------------------------------

        Collider[] colliders =
            doorCurveToDisable.GetComponentsInChildren<Collider>(
                true
            );


        foreach (Collider col in colliders)
        {
            if (col == null)
                continue;


            col.enabled = false;


            Debug.Log(
                $"[OpenTheGateLock] Collider OFF : " +
                $"{col.gameObject.name}"
            );
        }


        // --------------------------------------------
        // GameObject 자체 비활성화
        // --------------------------------------------

        doorCurveToDisable.SetActive(false);


        Debug.Log(
            $"[OpenTheGateLock] Door_Curv SetActive(false) / " +
            $"activeSelf = {doorCurveToDisable.activeSelf}"
        );
    }


    // ================================================
    // 자물쇠 고리 올리기
    // ================================================

    private IEnumerator OpenLockRoutine()
    {
        Debug.Log(
            "[OpenTheGateLock] 자물쇠 열기 시작"
        );


        Vector3 startPosition =
            lockCurve.localPosition;


        Vector3 targetPosition =
            startPosition +
            Vector3.up * liftHeight;


        float elapsedTime = 0f;


        if (liftDuration <= 0f)
        {
            lockCurve.localPosition =
                targetPosition;


            Debug.Log(
                "[OpenTheGateLock] 자물쇠 열기 완료"
            );


            yield break;
        }


        while (elapsedTime < liftDuration)
        {
            elapsedTime +=
                Time.deltaTime;


            float t =
                Mathf.Clamp01(
                    elapsedTime /
                    liftDuration
                );


            t =
                Mathf.SmoothStep(
                    0f,
                    1f,
                    t
                );


            lockCurve.localPosition =
                Vector3.Lerp(
                    startPosition,
                    targetPosition,
                    t
                );


            yield return null;
        }


        lockCurve.localPosition =
            targetPosition;


        Debug.Log(
            "[OpenTheGateLock] 자물쇠 열기 완료"
        );
    }
}