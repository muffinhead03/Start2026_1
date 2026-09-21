using UnityEngine;
using System.Collections;

public class Coin_Insert : MonoBehaviour
{
    // ============================================
    // Player
    // ============================================

    [Header("Player")]

    [SerializeField]
    private Player_Grab playerGrab;


    // ============================================
    // Coin Rotation
    // ============================================

    [Header("동전 방향")]

    [SerializeField]
    private Transform coinRotationPoint;


    // ============================================
    // Retro Game Manager
    // ============================================

    [Header("Retro Game Manager")]

    [SerializeField]
    private RetroGameManager retroGameManager;


    // ============================================
    // Insert Collider
    // ============================================

    [Header("실제 동전 투입구 Collider")]

    [SerializeField]
    private Collider insertCollider;


    // ============================================
    // Coin Setting
    // ============================================

    [Header("동전 설정")]

    [SerializeField]
    private string coinName = "Coin";


    // ============================================
    // Insert Position
    // ============================================

    [Header("Collider 중앙에서 안쪽으로 들어갈 거리")]

    [SerializeField]
    private float insertDistance = 0f;


    // ============================================
    // Coin Move
    // ============================================

    [Header("동전 투입 후 처리")]

    [SerializeField]
    private float coinMoveDelay = 0.5f;


    [SerializeField]
    private Transform finalMovePoint;


    [SerializeField]
    private float finalMoveDuration = 3f;


    // ============================================
    // Coin Insert Audio
    // ============================================

    [Header("동전 투입 사운드")]

    [Tooltip(
        "동전 투입 효과음을 재생할 Play_Audio.\n" +
        "AudioCollection의 Game_PuttingCoin을 연결"
    )]
    [SerializeField]
    private Play_Audio coinInsertAudio;


    [Tooltip("동전이 투입구에 들어갈 때 재생할 효과음")]
    [SerializeField]
    private AudioClip coinInsertClip;


    // ============================================
    // State
    // ============================================

    private bool isInserting = false;


    // ============================================
    // Awake
    // ============================================

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


    // ============================================
    // Start
    // ============================================

    private void Start()
    {
        if (retroGameManager == null)
        {
            retroGameManager =
                FindFirstObjectByType<RetroGameManager>();
        }
    }


    // ============================================
    // Insert Coin
    // ============================================

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
        {
            return;
        }


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


        // ========================================
        // 실제 Collider 중앙 위치
        // ========================================

        Vector3 colliderCenter =
            insertCollider.bounds.center;


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
            playerGrab.PutOn(
                targetPosition
            );


        if (coin == null)
        {
            isInserting = false;

            return;
        }
    // ============================================
    // Coin Insert이므로
    // 일반 Drop Sound는 이번 한 번 재생하지 않음
    // ============================================

    Coin_ReleaseSound releaseSound =
        coin.GetComponent<Coin_ReleaseSound>();


    if (releaseSound != null)
        {
            releaseSound.SuppressNextReleaseSound();
        }

        StartCoroutine(
            CompleteInsert(
                coin,
                targetPosition
            )
        );
    }


    // ============================================
    // Complete Insert
    // ============================================

    private IEnumerator CompleteInsert(
        GameObject coin,
        Vector3 targetPosition)
    {
        // ========================================
        // 1. 손 → 동전 투입구 이동 완료 대기
        // ========================================

        yield return new WaitForSeconds(
            playerGrab.targetTime
        );


        if (coin == null)
        {
            isInserting = false;

            yield break;
        }


        // ========================================
        // 2. 동전 투입구 위치 정확하게 보정
        // ========================================

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


        // ========================================
        // ★ 3. 동전 투입 사운드
        //
        // 실제 투입구에 도착한 순간 재생
        // 반드시 Play_Audio 사용
        // ========================================

        PlayCoinInsertSound();


        // ========================================
        // 4. RetroGameManager에 투입 알림
        // ========================================

        if (retroGameManager != null)
        {
            retroGameManager.SetCoinInserted(
                true
            );
        }


        // ========================================
        // 5. 투입구에서 잠깐 대기
        // ========================================

        yield return new WaitForSeconds(
            coinMoveDelay
        );


        if (coin == null)
        {
            isInserting = false;

            yield break;
        }


        // ========================================
        // 6. 최종 이동 위치 확인
        // ========================================

        if (finalMovePoint == null)
        {
            Debug.LogError(
                "[Coin_Insert] Final Move Point가 없습니다.",
                this
            );


            isInserting = false;

            yield break;
        }


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


        // ========================================
        // 7. 최종 위치까지 이동
        // ========================================

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


        // ========================================
        // 8. 최종 위치 정확하게 보정
        // ========================================

        coin.transform.position =
            endPosition;


        coin.transform.rotation =
            endRotation;


        Debug.Log(
            "[Coin_Insert] 동전 최종 이동 완료",
            coin
        );


        // ========================================
        // 9. 최종 이동 후 동전 비활성화
        // ========================================

        coin.SetActive(false);


        isInserting = false;
    }


    // ============================================
    // Coin Insert Sound
    // ============================================

    private void PlayCoinInsertSound()
    {
        if (coinInsertAudio == null)
        {
            Debug.LogWarning(
                "[Coin_Insert] " +
                "Coin Insert Play_Audio가 연결되지 않았습니다.",
                this
            );

            return;
        }


        if (coinInsertClip == null)
        {
            Debug.LogWarning(
                "[Coin_Insert] " +
                "Coin Insert AudioClip이 연결되지 않았습니다.",
                this
            );

            return;
        }


        // 팀 공용 Play_Audio를 통해 재생
        coinInsertAudio.PlayAudio(
            coinInsertClip
        );


        Debug.Log(
            "[Coin_Insert] 동전 투입 Sound 재생",
            this
        );
    }


    // ============================================
    // Scene Gizmo
    // ============================================

    private void OnDrawGizmos()
    {
        if (insertCollider == null)
        {
            return;
        }


        Vector3 center =
            insertCollider.bounds.center;


        Gizmos.DrawWireSphere(
            center,
            0.03f
        );


        Gizmos.DrawRay(
            center,
            insertCollider.transform.forward *
            0.3f
        );
    }
}