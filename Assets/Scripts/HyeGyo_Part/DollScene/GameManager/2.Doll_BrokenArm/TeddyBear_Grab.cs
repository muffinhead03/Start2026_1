using UnityEngine;

public class TeddyBear_Grab : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private Player_Grab playerGrab;

    [Header("Bear")]
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
    }


    private void Update()
    {
        /*
         * 부모가 Player_Grab의 Hand인지 확인하는 방식.
         *
         * 기존 Player_Grab은 물건을 집으면
         * Hand의 자식으로 넣고,
         *
         * Release()하면
         * SetParent(null) 처리함.
         */

        bool isCurrentlyHeld = IsHeldByPlayer();


        // 방금 집힌 상태
        if (isCurrentlyHeld && !wasHeld)
        {
            wasHeld = true;
            isThrown = false;

            Debug.Log("[TeddyBear] 곰 인형을 들었습니다.");
        }


        // 이전 프레임까지 들고 있었는데
        // 현재 손에서 빠졌다면 던져진 것으로 판정
        if (!isCurrentlyHeld && wasHeld)
        {
            wasHeld = false;
            isThrown = true;

            Debug.Log("[TeddyBear] 곰 인형이 던져졌습니다.");
        }
    }


    private bool IsHeldByPlayer()
    {
        if (playerGrab == null)
            return false;

        if (playerGrab.Hand == null)
            return false;


        return transform.parent == playerGrab.Hand;
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (!isThrown)
            return;


        // 너무 약한 충돌은 무시
        if (collision.relativeVelocity.magnitude < minImpactSpeed)
            return;


        /*
         * 한 번 던졌을 때
         * 최초 충돌 하나만 인정.
         */
        isThrown = false;


        Debug.Log(
            $"[TeddyBear] 던진 후 충돌 감지 / " +
            $"속도 : {collision.relativeVelocity.magnitude}"
        );


        if (speakingBear != null)
        {
            speakingBear.RegisterThrowHit();
        }
    }
}