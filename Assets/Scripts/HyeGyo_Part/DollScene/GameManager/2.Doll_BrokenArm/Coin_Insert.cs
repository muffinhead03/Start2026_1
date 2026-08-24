using UnityEngine;
using System.Collections;

public class Coin_Insert : MonoBehaviour
{
    [Header("Player")]
    [SerializeField]
    private Player_Grab playerGrab;

    [Header("Retro Game Manager")]
    [SerializeField]
    private RetroGameManager retroGameManager;

    [Header("동전 투입 위치")]
    [SerializeField]
    private Transform insertPoint;

    [Header("동전 설정")]
    [SerializeField]
    private string coinName = "Coin";

    [Header("동전이 들어가는 거리")]
    [SerializeField]
    private float insertDistance = 0.3f;

    private bool isInserting = false;


    private void Start()
    {
        // RetroGameManager가 Inspector에 연결되지 않았다면 자동 검색
        if (retroGameManager == null)
        {
            retroGameManager =
                FindFirstObjectByType<RetroGameManager>();
        }

        if (retroGameManager == null)
        {
            Debug.LogWarning(
                "[Coin_Insert] RetroGameManager를 찾을 수 없습니다.",
                this
            );
        }
    }


    // ============================================
    // Event_On_Ray의 OnClick에서 호출
    // ============================================

    public void InsertCoin()
    {
        // 중복 클릭 방지
        if (isInserting)
        {
            return;
        }


        if (playerGrab == null)
        {
            Debug.LogWarning(
                "[Coin_Insert] Player_Grab이 연결되지 않았습니다.",
                this
            );

            return;
        }


        if (insertPoint == null)
        {
            Debug.LogWarning(
                "[Coin_Insert] InsertPoint가 연결되지 않았습니다.",
                this
            );

            return;
        }


        // 현재 손에 Coin이 있는지 확인
        if (!playerGrab.hasKey(coinName))
        {
            Debug.Log(
                "[Coin_Insert] 현재 손에 동전이 없습니다.",
                this
            );

            return;
        }


        isInserting = true;


        // InsertPoint의 로컬 Z축(+Z) 방향으로 0.3m 안쪽
        Vector3 targetPosition =
            insertPoint.position +
            insertPoint.forward * insertDistance;


        /*
         * Player_Grab의 기존 PutOn 사용
         *
         * PutOn 내부에서 기존 MoveToTargetPosition을 사용하기 때문에
         * 물건을 집을 때 사용하는 targetTime과 동일한 시간으로 이동합니다.
         */
        GameObject coin =
            playerGrab.PutOn(targetPosition);


        if (coin == null)
        {
            isInserting = false;
            return;
        }


        // 이동이 끝난 후 처리
        StartCoroutine(
            CompleteInsert(
                coin,
                targetPosition
            )
        );
    }


    // ============================================
    // 동전 투입 완료
    // ============================================

    private IEnumerator CompleteInsert(
        GameObject coin,
        Vector3 targetPosition)
    {
        // Player_Grab의 이동시간과 동일하게 대기
        yield return new WaitForSeconds(
            playerGrab.targetTime
        );


        if (coin == null)
        {
            isInserting = false;
            yield break;
        }


        // 최종 위치 정확하게 보정
        coin.transform.position =
            targetPosition;


        // InsertPoint 방향으로 동전 방향 맞추기
        coin.transform.rotation =
            insertPoint.rotation;


        // 기계의 InsertPoint 아래에 넣기
        coin.transform.SetParent(
            insertPoint,
            true
        );


        // ========================================
        // RetroGameManager에게 동전 투입 알림
        // ========================================

        if (retroGameManager != null)
        {
            retroGameManager.SetCoinInserted(true);

            Debug.Log(
                "[Coin_Insert] 동전 투입 완료 → RetroGameManager 전달",
                this
            );
        }
        else
        {
            Debug.LogWarning(
                "[Coin_Insert] RetroGameManager가 없어 동전 상태를 전달하지 못했습니다.",
                this
            );
        }


        isInserting = false;
    }
}