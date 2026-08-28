using System.Collections;
using UnityEngine;

public class TeddyBear_Grab : MonoBehaviour
{
    [Header("Player")]
    [SerializeField]
    private Player_Grab playerGrab;


    [Header("Bear Object")]
    [SerializeField]
    private Transform normalBearObject;


    [Header("Bear Logic")]
    [SerializeField]
    private SpeakingBearDoll speakingBear;


    // =============================================
    // Throw Setting
    // =============================================

    [Header("Throw Setting")]

    [Tooltip("던질 때 사용할 Rigidbody")]
    [SerializeField]
    private Rigidbody bearRigidbody;


    [Tooltip("던지는 방향 기준. 보통 Player Camera")]
    [SerializeField]
    private Transform throwDirection;


    [Tooltip("던지는 힘")]
    [SerializeField]
    private float throwForce = 3f;


    [Tooltip(
        "살짝 위쪽으로 보정할 값.\n" +
        "높이 뜨는 게 싫으면 0으로 설정"
    )]
    [SerializeField]
    private float upwardBias = 0f;


    [Tooltip("던질 때 기존 속도를 초기화할지")]
    [SerializeField]
    private bool resetVelocityBeforeThrow = true;


    // =============================================
    // State
    // =============================================

    [Header("State")]

    [SerializeField]
    private bool wasHeld = false;


    [SerializeField]
    private bool isThrown = false;


    // =============================================
    // Collision
    // =============================================

    [Header("충돌 판정")]

    [SerializeField]
    private float minImpactSpeed = 1.5f;


    private Coroutine throwCoroutine;


    // =============================================
    // Start
    // =============================================

    private void Start()
    {
        if (playerGrab == null)
        {
            playerGrab =
                FindFirstObjectByType<Player_Grab>();
        }


        if (speakingBear == null)
        {
            speakingBear =
                GetComponent<SpeakingBearDoll>();
        }


        if (normalBearObject == null)
        {
            normalBearObject =
                transform;
        }


        if (bearRigidbody == null)
        {
            bearRigidbody =
                GetComponent<Rigidbody>();
        }


        // 방향 기준을 연결하지 않았다면
        // Main Camera 자동 사용
        if (throwDirection == null &&
            Camera.main != null)
        {
            throwDirection =
                Camera.main.transform;
        }
    }


    // =============================================
    // Update
    // =============================================

    private void Update()
    {
        if (normalBearObject == null)
            return;


        bool isCurrentlyHeld =
            IsHeldByPlayer();


        // =========================================
        // 곰을 집음
        // =========================================

        if (isCurrentlyHeld &&
            !wasHeld)
        {
            wasHeld = true;

            isThrown = false;


            Debug.Log(
                "[TeddyBear] 곰 인형을 들었습니다."
            );
        }


        // =========================================
        // 손에서 빠짐
        //
        // 기존 Player_Grab에서 E를 눌러
        // 손에서 놓은 순간 여기로 들어옴
        // =========================================

        if (!isCurrentlyHeld &&
            wasHeld)
        {
            wasHeld = false;

            isThrown = true;


            Debug.Log(
                "[TeddyBear] 곰 인형 던지기 감지"
            );


            // Player_Grab의 기존 물리 처리 이후에
            // 우리가 원하는 속도로 던지기
            if (throwCoroutine != null)
            {
                StopCoroutine(
                    throwCoroutine
                );
            }


            throwCoroutine =
                StartCoroutine(
                    ThrowRoutine()
                );
        }
    }


    // =============================================
    // 현재 플레이어가 들고 있는지
    // =============================================

    private bool IsHeldByPlayer()
    {
        if (playerGrab == null)
            return false;


        if (playerGrab.Hand == null)
            return false;


        if (normalBearObject == null)
            return false;


        return
            normalBearObject.parent ==
            playerGrab.Hand;
    }


    // =============================================
    // 바라보는 방향으로 던지기
    // =============================================

    private IEnumerator ThrowRoutine()
    {
        /*
         * Player_Grab에서 E 입력으로
         * 부모 해제 / Rigidbody 변경 / 기존 AddForce 등을
         * 먼저 처리하게 한 뒤 우리가 속도를 설정.
         */
        yield return new WaitForFixedUpdate();


        if (bearRigidbody == null)
        {
            Debug.LogWarning(
                "[TeddyBear] Rigidbody가 없습니다."
            );

            yield break;
        }


        if (throwDirection == null)
        {
            Debug.LogWarning(
                "[TeddyBear] Throw Direction이 없습니다."
            );

            yield break;
        }


        // 물리 활성화
        bearRigidbody.isKinematic = false;

        bearRigidbody.useGravity = true;


        // =========================================
        // 기존 속도 제거
        // =========================================

        if (resetVelocityBeforeThrow)
        {
            bearRigidbody.linearVelocity =
                Vector3.zero;

            bearRigidbody.angularVelocity =
                Vector3.zero;
        }


        // =========================================
        // 바라보는 방향
        // =========================================

        Vector3 direction =
            throwDirection.forward;


        // 위쪽 보정
        direction +=
            Vector3.up *
            upwardBias;


        direction.Normalize();


        // =========================================
        // 힘 적용
        // =========================================

        bearRigidbody.AddForce(
            direction * throwForce,
            ForceMode.VelocityChange
        );


        Debug.Log(
            $"[TeddyBear] 던지기 / " +
            $"Direction = {direction} / " +
            $"Force = {throwForce}"
        );


        throwCoroutine = null;
    }


    // =============================================
    // 실제 Unity 충돌 이벤트
    // =============================================

    private void OnCollisionEnter(
        Collision collision)
    {
        Debug.Log(
            $"[TeddyBear] 실제 충돌 발생 : " +
            $"{collision.gameObject.name}"
        );


        RegisterCollision(
            collision
        );
    }


    // =============================================
    // 던진 뒤 충돌 판정
    // =============================================

    public void RegisterCollision(
        Collision collision)
    {
        if (!isThrown)
        {
            Debug.Log(
                "[TeddyBear] 던진 상태가 아니므로 충돌 무시"
            );

            return;
        }


        float impactSpeed =
            collision.relativeVelocity.magnitude;


        Debug.Log(
            $"[TeddyBear] 충돌 속도 : {impactSpeed}"
        );


        if (impactSpeed <
            minImpactSpeed)
        {
            Debug.Log(
                "[TeddyBear] 충돌 속도가 너무 낮아서 무시"
            );

            return;
        }


        // 한 번 던질 때 최초 충돌만 인정
        isThrown = false;


        Debug.Log(
            "[TeddyBear] 유효한 던지기 충돌!"
        );


        if (speakingBear != null)
        {
            speakingBear.RegisterThrowHit();
        }
        else
        {
            Debug.LogError(
                "[TeddyBear] SpeakingBearDoll이 없습니다."
            );
        }
    }
}