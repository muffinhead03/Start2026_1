using System.Collections;
using UnityEngine;

public class DollFixedCheckManager : MonoBehaviour
{
    // =========================================================
    // Doll Repair State
    // =========================================================

    [Header("Doll Repair State")]

    [Tooltip("인형 수리 상태를 GameManager로 전달하는 DollScene_ChangeDoll")]
    [SerializeField]
    private DollScene_ChangeDoll changeDoll;


    // =========================================================
    // Key
    // =========================================================

    [Header("Key")]

    [Tooltip("인형이 처음부터 들고 있는 열쇠")]
    [SerializeField]
    private GameObject keyObject;

    [Tooltip("바닥에 떨어졌다고 판단할 속도")]
    [SerializeField]
    private float settledVelocity = 0.08f;

    [Tooltip("착지 판정 최대 대기시간")]
    [SerializeField]
    private float maximumSettleWait = 5f;


    // =========================================================
    // State
    // =========================================================

    private bool repairSequenceStarted = false;


    // Key가 원래 사용하던 Layer
    private int[] originalKeyLayers;


    // =========================================================
    // Start
    // =========================================================

    private void Start()
    {
        if (changeDoll == null)
        {
            changeDoll =
                FindFirstObjectByType<DollScene_ChangeDoll>();
        }


        if (changeDoll == null)
        {
            Debug.LogError(
                "[DollFixedCheck] DollScene_ChangeDoll을 찾을 수 없습니다.",
                this
            );
        }


        PrepareLockedKey();


        Debug.Log(
            "[DollFixedCheck] 초기화 완료"
        );
    }


    // =========================================================
    // 인형 수리 완료 확인
    // =========================================================

    private void Update()
    {
        // 이미 Key Drop이 시작됐다면
        // 다시 실행하지 않음
        if (repairSequenceStarted)
        {
            return;
        }


        if (changeDoll == null)
        {
            return;
        }


        /*
         * DollScene_ChangeDoll
         *      ↓
         * DollScene_GameManager
         *      ↓
         * IsDollRepaired
         *
         * 최종 수리 완료 상태 확인
         */
        if (!changeDoll.IsDollRepairCompleted)
        {
            return;
        }


        repairSequenceStarted =
            true;


        Debug.Log(
            "[DollFixedCheck] 인형 전체 수리 완료 감지 → Key Drop 시작"
        );


        StartCoroutine(
            ReleaseKeySequence()
        );
    }


    // =========================================================
    // 초기 Key 잠금
    // =========================================================

    private void PrepareLockedKey()
    {
        if (keyObject == null)
        {
            Debug.LogWarning(
                "[DollFixedCheck] Key가 연결되지 않았습니다.",
                this
            );

            return;
        }


        // -----------------------------------------------------
        // Key 획득 기능 OFF
        // -----------------------------------------------------

        SetKeyGrabEnabled(
            false
        );


        // -----------------------------------------------------
        // Ray로 선택되지 않도록 Layer 임시 변경
        // -----------------------------------------------------

        SaveKeyLayers();


        int ignoreRaycastLayer =
            LayerMask.NameToLayer(
                "Ignore Raycast"
            );


        if (ignoreRaycastLayer >= 0)
        {
            SetKeyLayerRecursive(
                keyObject,
                ignoreRaycastLayer
            );
        }


        // -----------------------------------------------------
        // Rigidbody 물리 OFF
        // -----------------------------------------------------

        Rigidbody rb =
            keyObject.GetComponent<Rigidbody>();


        if (rb != null)
        {
            rb.linearVelocity =
                Vector3.zero;

            rb.angularVelocity =
                Vector3.zero;

            rb.useGravity =
                false;

            rb.isKinematic =
                true;
        }


        Debug.Log(
            "[DollFixedCheck] Key 잠금 완료"
        );
    }


    // =========================================================
    // 수리 완료 후 Key 처리
    // =========================================================

    private IEnumerator ReleaseKeySequence()
    {
        if (keyObject == null)
        {
            Debug.LogError(
                "[DollFixedCheck] Key Object가 없습니다.",
                this
            );

            yield break;
        }


        Debug.Log(
            "[DollFixedCheck] Key 낙하 준비"
        );


        // -----------------------------------------------------
        // 1. 떨어지는 동안에는 아직 획득 불가능
        // -----------------------------------------------------

        SetKeyGrabEnabled(
            false
        );


        // -----------------------------------------------------
        // 2. 인형 손에서 Key 분리
        // -----------------------------------------------------

        keyObject.transform.SetParent(
            null,
            true
        );


        // -----------------------------------------------------
        // 3. Collider 활성화
        // -----------------------------------------------------

        SetKeyColliders(
            true
        );


        // -----------------------------------------------------
        // 4. Rigidbody 준비
        // -----------------------------------------------------

        Rigidbody rb =
            keyObject.GetComponent<Rigidbody>();


        if (rb == null)
        {
            rb =
                keyObject.AddComponent<Rigidbody>();


            Debug.Log(
                "[DollFixedCheck] Key에 Rigidbody 자동 추가"
            );
        }


        rb.linearVelocity =
            Vector3.zero;

        rb.angularVelocity =
            Vector3.zero;


        // -----------------------------------------------------
        // 5. 중력 활성화
        //
        // 여기부터 Key가 실제로 아래로 떨어짐
        // -----------------------------------------------------

        rb.isKinematic =
            false;

        rb.useGravity =
            true;


        Debug.Log(
            "[DollFixedCheck] Key 중력 활성화 → 낙하 시작"
        );


        // -----------------------------------------------------
        // 6. 바닥에 안정될 때까지 대기
        // -----------------------------------------------------

        yield return StartCoroutine(
            WaitUntilKeySettled(
                rb
            )
        );


        // -----------------------------------------------------
        // 7. 원래 Raycast Layer 복구
        // -----------------------------------------------------

        RestoreKeyLayers();


        // -----------------------------------------------------
        // 8. 이제 획득 가능
        // -----------------------------------------------------

        SetKeyGrabEnabled(
            true
        );


        Debug.Log(
            "[DollFixedCheck] Key 착지 완료 → 획득 가능"
        );
    }


    // =========================================================
    // Key가 바닥에 떨어졌는지 확인
    // =========================================================

    private IEnumerator WaitUntilKeySettled(
        Rigidbody rb)
    {
        if (rb == null)
        {
            yield break;
        }


        /*
         * Gravity를 켠 직후에는 속도가 0에 가까우므로
         * 바로 착지했다고 판단하지 않도록 잠깐 대기
         */
        yield return new WaitForSeconds(
            0.3f
        );


        float stableTime =
            0f;

        float elapsed =
            0f;


        while (elapsed < maximumSettleWait)
        {
            if (rb == null)
            {
                yield break;
            }


            elapsed +=
                Time.deltaTime;


            /*
             * 속도가 충분히 낮은 상태가
             * 일정 시간 유지되면 착지로 판단
             */
            if (
                rb.linearVelocity.sqrMagnitude
                <
                settledVelocity *
                settledVelocity
            )
            {
                stableTime +=
                    Time.deltaTime;


                if (stableTime >= 0.3f)
                {
                    yield break;
                }
            }
            else
            {
                stableTime =
                    0f;
            }


            yield return null;
        }


        /*
         * 물리적으로 계속 흔들리거나
         * 어딘가에 걸린 경우에도
         * 영원히 획득 불가능 상태가 되지 않도록 함
         */
        Debug.LogWarning(
            "[DollFixedCheck] Key 착지 판정 시간 초과 → 강제로 획득 가능 상태로 전환"
        );
    }


    // =========================================================
    // Key Grabbable ON / OFF
    // =========================================================

    private void SetKeyGrabEnabled(
        bool enabled)
    {
        if (keyObject == null)
        {
            return;
        }


        // -----------------------------------------------------
        // Object_Grabbable
        // -----------------------------------------------------

        Object_Grabbable[] grabbables =
            keyObject.GetComponentsInChildren<Object_Grabbable>(
                true
            );


        foreach (
            Object_Grabbable grabbable
            in grabbables
        )
        {
            if (grabbable != null)
            {
                grabbable.enabled =
                    enabled;
            }
        }


        // -----------------------------------------------------
        // Event_On_Ray
        // -----------------------------------------------------

        Event_On_Ray[] rayEvents =
            keyObject.GetComponentsInChildren<Event_On_Ray>(
                true
            );


        foreach (
            Event_On_Ray rayEvent
            in rayEvents
        )
        {
            if (rayEvent != null)
            {
                rayEvent.enabled =
                    enabled;
            }
        }
    }


    // =========================================================
    // Key Collider
    // =========================================================

    private void SetKeyColliders(
        bool enabled)
    {
        if (keyObject == null)
        {
            return;
        }


        Collider[] colliders =
            keyObject.GetComponentsInChildren<Collider>(
                true
            );


        foreach (
            Collider col
            in colliders
        )
        {
            if (col != null)
            {
                col.enabled =
                    enabled;
            }
        }
    }


    // =========================================================
    // Key Layer 저장
    // =========================================================

    private void SaveKeyLayers()
    {
        if (keyObject == null)
        {
            return;
        }


        Transform[] transforms =
            keyObject.GetComponentsInChildren<Transform>(
                true
            );


        originalKeyLayers =
            new int[transforms.Length];


        for (int i = 0;
             i < transforms.Length;
             i++)
        {
            originalKeyLayers[i] =
                transforms[i].gameObject.layer;
        }
    }


    // =========================================================
    // Key Layer 복구
    // =========================================================

    private void RestoreKeyLayers()
    {
        if (keyObject == null ||
            originalKeyLayers == null)
        {
            return;
        }


        Transform[] transforms =
            keyObject.GetComponentsInChildren<Transform>(
                true
            );


        int count =
            Mathf.Min(
                transforms.Length,
                originalKeyLayers.Length
            );


        for (int i = 0;
             i < count;
             i++)
        {
            transforms[i].gameObject.layer =
                originalKeyLayers[i];
        }
    }


    // =========================================================
    // Key 전체 Layer 변경
    // =========================================================

    private void SetKeyLayerRecursive(
        GameObject target,
        int layer)
    {
        if (target == null ||
            layer < 0)
        {
            return;
        }


        target.layer =
            layer;


        foreach (
            Transform child
            in target.transform
        )
        {
            SetKeyLayerRecursive(
                child.gameObject,
                layer
            );
        }
    }
}