using UnityEngine;
using System.Collections;

public class Coin_Insert : MonoBehaviour
{
    [Header("Player")]
    [SerializeField]
    private Player_Grab playerGrab;

    [Header("동전 방향")]
    [SerializeField]
    private Transform coinRotationPoint;

    [Header("Retro Game Manager")]
    [SerializeField]
    private RetroGameManager retroGameManager;

    [Header("실제 동전 투입구 Collider")]
    [SerializeField]
    private Collider insertCollider;

    [Header("동전 설정")]
    [SerializeField]
    private string coinName = "Coin";

    [Header("Collider 중앙에서 안쪽으로 들어갈 거리")]
    [SerializeField]
    private float insertDistance = 0f;

    [Header("동전 투입 후 처리")]
    [SerializeField]
    private float coinMoveDelay = 0.5f;

    [SerializeField]
    private Transform finalMovePoint;

    [SerializeField]
    private float finalMoveDuration = 3f;

    private bool isInserting = false;


    private void Start()
    {
        if (retroGameManager == null)
        {
            retroGameManager =
                FindFirstObjectByType<RetroGameManager>();
        }
    }

    private void Awake()
{
    Debug.Log(
        $"[Coin_Insert Awake] " +
        $"오브젝트={gameObject.name}, " +
        $"InstanceID={GetInstanceID()}, " +
        $"Collider={(insertCollider != null ? insertCollider.name : "NULL")}",
        gameObject
    );
}


    public void InsertCoin()
    {

            Debug.Log(
        $"[Coin_Insert 실행] " +
        $"오브젝트={gameObject.name}, " +
        $"InstanceID={GetInstanceID()}, " +
        $"Collider={(insertCollider != null ? insertCollider.name : "NULL")}",
        gameObject
    );

        if (isInserting)
            return;

        if (playerGrab == null)
        {
            Debug.LogError(
                "[Coin_Insert] Player_Grab 없음",
                this
            );
            return;
        }

        if (insertCollider == null)
        {
            Debug.LogError(
                "[Coin_Insert] Insert Collider 없음",
                this
            );
            return;
        }

        if (!playerGrab.hasKey(coinName))
        {
            Debug.Log(
                "[Coin_Insert] 현재 손에 동전이 없음",
                this
            );
            return;
        }


        // ★ Transform.position이 아니라
        // 실제 Collider 중앙 위치를 사용
        Vector3 colliderCenter =
            insertCollider.bounds.center;


        // 일단 테스트할 때 insertDistance = 0 권장
        Vector3 targetPosition =
            colliderCenter +
            insertCollider.transform.forward *
            insertDistance;


        Debug.Log(
            $"[Coin_Insert]\n" +
            $"실행 오브젝트 : {gameObject.name}\n" +
            $"Collider : {insertCollider.name}\n" +
            $"Transform 위치 : {insertCollider.transform.position}\n" +
            $"Collider 실제 중앙 : {insertCollider.bounds.center}\n" +
            $"최종 목적지 : {targetPosition}",
            this
        );


        isInserting = true;

        GameObject coin =
            playerGrab.PutOn(targetPosition);


        if (coin == null)
        {
            isInserting = false;
            return;
        }


        StartCoroutine(
            CompleteInsert(
                coin,
                targetPosition
            )
        );
    }


private IEnumerator CompleteInsert(
    GameObject coin,
    Vector3 targetPosition)
{
    // ============================================
    // 1. 손에서 동전 투입구까지 이동 완료 대기
    // ============================================

    yield return new WaitForSeconds(
        playerGrab.targetTime
    );


    if (coin == null)
    {
        isInserting = false;
        yield break;
    }


    // ============================================
    // 2. 동전 투입구 위치 정확하게 보정
    // ============================================

    coin.transform.position =
        targetPosition;

    if (coinRotationPoint != null)
    {
        coin.transform.rotation =
            coinRotationPoint.rotation;
    }


    Debug.Log(
        "[Coin_Insert] 동전 투입구 도착",
        coin
    );


    // ============================================
    // 3. RetroGameManager에 동전 투입 알림
    // ============================================

    if (retroGameManager != null)
    {
        retroGameManager.SetCoinInserted(true);
    }


    // ============================================
    // 4. 투입구에서 잠깐 대기
    // ============================================

    yield return new WaitForSeconds(
        coinMoveDelay
    );


    if (coin == null)
    {
        isInserting = false;
        yield break;
    }


    // ============================================
    // 5. 최종 이동 위치 확인
    // ============================================

    if (finalMovePoint == null)
    {
        Debug.LogError(
            "[Coin_Insert] Final Move Point가 없습니다.",
            this
        );

        isInserting = false;
        yield break;
    }


    // 다른 오브젝트의 자식으로 되어 있다면 해제
    coin.transform.SetParent(
        null,
        true
    );


    Vector3 startPosition =
        coin.transform.position;

    Quaternion verticalRotation =
        coinRotationPoint != null
            ? coinRotationPoint.rotation
            : coin.transform.rotation;

    Quaternion startRotation =
        verticalRotation;

    Quaternion endRotation =
        verticalRotation;


    Vector3 endPosition =
        finalMovePoint.position;



    // ============================================
    // 6. 3초 동안 최종 위치로 이동
    // ============================================

    float elapsed = 0f;


    while (elapsed < finalMoveDuration)
    {
        if (coin == null)
        {
            isInserting = false;
            yield break;
        }


        elapsed += Time.deltaTime;


        float t =
            Mathf.Clamp01(
                elapsed /
                Mathf.Max(
                    finalMoveDuration,
                    0.0001f
                )
            );


        // 시작과 끝이 조금 부드럽게 움직이도록
        float smoothT =
            Mathf.SmoothStep(
                0f,
                1f,
                t
            );


        coin.transform.position =
            Vector3.Lerp(
                startPosition,
                endPosition,
                smoothT
            );


        coin.transform.rotation =
            Quaternion.Slerp(
                startRotation,
                endRotation,
                smoothT
            );


        yield return null;
    }


    // ============================================
    // 7. 최종 위치 정확하게 보정
    // ============================================

    coin.transform.position =
        endPosition;

    coin.transform.rotation =
        endRotation;


    Debug.Log(
        "[Coin_Insert] 동전 최종 이동 완료",
        coin
    );


    // ============================================
    // 8. 최종 이동 후 동전 비활성화
    // ============================================

    coin.SetActive(false);


    isInserting = false;
}


    // Scene 창에서 실제 목표 위치 확인용
    private void OnDrawGizmos()
    {
        if (insertCollider == null)
            return;

        Vector3 center =
            insertCollider.bounds.center;

        Gizmos.DrawWireSphere(
            center,
            0.03f
        );

        Gizmos.DrawRay(
            center,
            insertCollider.transform.forward * 0.3f
        );
    }
}