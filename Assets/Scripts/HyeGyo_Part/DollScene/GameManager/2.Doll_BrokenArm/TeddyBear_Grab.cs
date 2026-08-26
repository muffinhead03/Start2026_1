using UnityEngine;

public class TeddyBear_Grab : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private Player_Grab playerGrab;

    [Header("Bear Object")]
    [SerializeField] private Transform normalBearObject;

    [Header("Bear Logic")]
    [SerializeField] private SpeakingBearDoll speakingBear;

    [Header("State")]
    [SerializeField] private bool wasHeld = false;
    [SerializeField] private bool isThrown = false;

    [Header("충돌 판정")]
    [SerializeField] private float minImpactSpeed = 1.5f;


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

        // 자기 자신이 실제 정상 곰이라면 자동 등록
        if (normalBearObject == null)
        {
            normalBearObject = transform;
        }
    }


    private void Update()
    {
        if (normalBearObject == null)
            return;

        bool isCurrentlyHeld =
            IsHeldByPlayer();


        // ================================
        // 곰을 집었을 때
        // ================================

        if (isCurrentlyHeld && !wasHeld)
        {
            wasHeld = true;
            isThrown = false;

            Debug.Log(
                "[TeddyBear] 곰 인형을 들었습니다."
            );
        }


        // ================================
        // 손에서 빠짐 = 던짐
        // ================================

        if (!isCurrentlyHeld && wasHeld)
        {
            wasHeld = false;
            isThrown = true;

            Debug.Log(
                "[TeddyBear] 곰 인형이 던져졌습니다."
            );
        }
    }


    private bool IsHeldByPlayer()
    {
        if (playerGrab == null)
            return false;

        if (playerGrab.Hand == null)
            return false;

        if (normalBearObject == null)
            return false;


        return normalBearObject.parent ==
               playerGrab.Hand;
    }


    // =============================================
    // ★ 실제 Unity 충돌 이벤트
    // =============================================

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(
            $"[TeddyBear] 실제 충돌 발생 : " +
            $"{collision.gameObject.name}"
        );

        RegisterCollision(collision);
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


        if (impactSpeed < minImpactSpeed)
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