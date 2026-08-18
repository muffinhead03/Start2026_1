using System.Collections;
using UnityEngine;

public class OpenTheGateLock : MonoBehaviour
{
    [Header("Lock Curve")]
    [Tooltip("위로 들어올릴 Lock_Curve 오브젝트")]
    [SerializeField]
    private Transform lockCurve;


    [Header("Door Curve")]
    [Tooltip("자물쇠가 열리기 직전에 비활성화할 DoorCurve 오브젝트")]
    [SerializeField]
    private GameObject doorCurve;


    [Header("Open Animation")]
    [Tooltip("자물쇠 고리가 위로 올라가는 거리")]
    [SerializeField]
    private float liftHeight = 0.2f;

    [Tooltip("자물쇠 고리가 올라가는 시간")]
    [SerializeField]
    private float liftDuration = 1f;


    private bool isOpened = false;


    // ================================================
    // 자물쇠 열기
    // ================================================

    public void OpenLock()
    {
        Debug.Log("[OpenTheGateLock] OpenLock() 호출됨");

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

        StartCoroutine(OpenLockRoutine());
    }


    // ================================================
    // 자물쇠 고리 들어올리기
    // ================================================

    private IEnumerator OpenLockRoutine()
    {
        Debug.Log("[OpenTheGateLock] 자물쇠 열기 시작");


        // ================================================
        // 1. DoorCurve 비활성화
        // ================================================

        if (doorCurve != null)
        {
            doorCurve.SetActive(false);

            Debug.Log(
                "[OpenTheGateLock] DoorCurve 비활성화"
            );
        }
        else
        {
            Debug.LogWarning(
                "[OpenTheGateLock] DoorCurve가 연결되지 않았습니다."
            );
        }


        // ================================================
        // 2. Lock_Curve 들어올리기
        // ================================================

        Vector3 startPosition = lockCurve.localPosition;

        Vector3 targetPosition =
            startPosition + Vector3.up * liftHeight;

        float elapsedTime = 0f;


        while (elapsedTime < liftDuration)
        {
            elapsedTime += Time.deltaTime;

            float t =
                Mathf.Clamp01(
                    elapsedTime / liftDuration
                );

            t = Mathf.SmoothStep(
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


        // 최종 위치 정확하게 맞추기
        lockCurve.localPosition = targetPosition;


        Debug.Log(
            "[OpenTheGateLock] 자물쇠 열기 완료"
        );
    }
}