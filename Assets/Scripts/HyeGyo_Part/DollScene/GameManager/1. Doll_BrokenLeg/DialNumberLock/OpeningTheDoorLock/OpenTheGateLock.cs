using System.Collections;
using UnityEngine;

public class OpenTheGateLock : MonoBehaviour
{
    // ================================================
    // Lock Curve
    // ================================================

    [Header("Lock Curve")]

    [Tooltip("위로 들어올릴 자물쇠 고리")]
    [SerializeField]
    private Transform lockCurve;


    // ================================================
    // Disable Objects
    // ================================================

    [Header("Disable Objects")]

    [Tooltip("문 쪽에서 자물쇠를 고정하고 있는 Door_Curv")]
    [SerializeField]
    private GameObject doorCurveToDisable;


    [Tooltip("Lock_Curve 아래에서 자물쇠를 고정하고 있는 GameObject")]
    [SerializeField]
    private GameObject lockHolderToDisable;


    // ================================================
    // Lock Drop
    // ================================================

    [Header("Lock Drop")]

    [Tooltip("떨어질 자물쇠 본체 Rigidbody. 사용하지 않으면 비워도 됨")]
    [SerializeField]
    private Rigidbody lockRigidbody;


    [Tooltip("고리가 열린 뒤 Rigidbody를 물리 상태로 전환할지")]
    [SerializeField]
    private bool dropLockAfterOpen = true;


    [Tooltip("고리가 열린 뒤 낙하까지 기다리는 시간")]
    [SerializeField]
    private float dropDelay = 0.15f;


    // ================================================
    // Door
    // ================================================

    [Header("Door")]

    [Tooltip("잠금 해제할 문")]
    [SerializeField]
    private Door_OpenClose door;


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
    // Puzzle 정답에서 호출
    // ================================================

    public void OpenLock()
    {
        Debug.Log("[OpenTheGateLock] OpenLock() 호출됨");


        if (isOpened)
            return;


        isOpened = true;


        // --------------------------------------------
        // 1. 문 쪽 고정물 강제 비활성화
        // --------------------------------------------

        DisableObject(
            doorCurveToDisable,
            "Door_Curv"
        );


        // --------------------------------------------
        // 2. Lock_Curve 아래 고정물도 강제 비활성화
        // --------------------------------------------

        DisableObject(
            lockHolderToDisable,
            "Lock Holder"
        );


        // --------------------------------------------
        // 3. 문 잠금 해제
        // 문을 바로 열지는 않음.
        // 이후 E → ToggleDoor()가 가능해짐.
        // --------------------------------------------

        if (door != null)
        {
            door.UnlockDoor();

            Debug.Log(
                "[OpenTheGateLock] Door 잠금 해제 완료"
            );
        }
        else
        {
            Debug.LogWarning(
                "[OpenTheGateLock] Door_OpenClose가 연결되지 않았습니다."
            );
        }


        // --------------------------------------------
        // 4. 고리 애니메이션
        // --------------------------------------------

        if (lockCurve != null)
        {
            StartCoroutine(
                OpenLockRoutine()
            );
        }
        else
        {
            Debug.LogWarning(
                "[OpenTheGateLock] Lock_Curve가 연결되지 않았습니다."
            );


            // 고리가 없어도 낙하 처리는 실행
            StartCoroutine(
                DropLockRoutine()
            );
        }
    }


    // ================================================
    // GameObject 강제 비활성화
    // ================================================

    private void DisableObject(
        GameObject target,
        string targetName)
    {
        if (target == null)
        {
            Debug.LogWarning(
                $"[OpenTheGateLock] {targetName} 연결 안 됨"
            );

            return;
        }


        // 자식까지 Collider 전부 끄기
        Collider[] colliders =
            target.GetComponentsInChildren<Collider>(
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


        // 오브젝트 자체 완전 비활성화
        target.SetActive(false);


        Debug.Log(
            $"[OpenTheGateLock] {targetName} OFF / " +
            $"Object = {target.name} / " +
            $"activeSelf = {target.activeSelf}"
        );
    }


    // ================================================
    // 자물쇠 고리 들어올리기
    // ================================================

    private IEnumerator OpenLockRoutine()
    {
        Debug.Log(
            "[OpenTheGateLock] 자물쇠 고리 열기 시작"
        );


        Vector3 startPosition =
            lockCurve.localPosition;


        Vector3 targetPosition =
            startPosition +
            Vector3.up * liftHeight;


        float elapsedTime = 0f;


        while (elapsedTime < liftDuration)
        {
            elapsedTime += Time.deltaTime;


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
            "[OpenTheGateLock] 자물쇠 고리 열기 완료"
        );


        // 고리 열린 다음 자물쇠 낙하
        yield return StartCoroutine(
            DropLockRoutine()
        );
    }


    // ================================================
    // 자물쇠 낙하
    // ================================================

    private IEnumerator DropLockRoutine()
    {
        if (!dropLockAfterOpen)
            yield break;


        if (dropDelay > 0f)
        {
            yield return new WaitForSeconds(
                dropDelay
            );
        }


        if (lockRigidbody == null)
        {
            Debug.LogWarning(
                "[OpenTheGateLock] Lock Rigidbody가 없습니다. " +
                "고정 오브젝트만 비활성화했습니다."
            );

            yield break;
        }


        lockRigidbody.isKinematic = false;
        lockRigidbody.useGravity = true;


        Debug.Log(
            "[OpenTheGateLock] 자물쇠 Rigidbody 해제 → 낙하"
        );
    }
}