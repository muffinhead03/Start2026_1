using System;
using System.Collections;
using UnityEngine;


public class SpringTrainSequenceManager : MonoBehaviour
{
    // =========================================================
    // Rail
    // =========================================================

    [Header("선로 정답 Manager")]
    [SerializeField]
    private SpringRailStateManager railStateManager;


    [Header("선로 Data Manager")]
    [SerializeField]
    private SpringRailDataManager railDataManager;


    // =========================================================
    // Train
    // =========================================================

    [Header("움직이는 색깔 기차")]
    [SerializeField]
    private SpringTrainPathMover movingTrain;


    [Header("검은 기차")]
    [SerializeField]
    private SpringTrainPathMover blackTrain;


    [Header("색깔 기차 Physics")]
    [SerializeField]
    private SpringTrainPhysicsController movingTrainPhysics;


    [Header("검은 기차 Physics")]
    [SerializeField]
    private SpringTrainPhysicsController blackTrainPhysics;


    // =========================================================
    // Spring
    // =========================================================

    [Header("태엽")]
    [SerializeField]
    private SpringPickupState springPickupState;


    // =========================================================
    // 이동
    // =========================================================

    [Header("색깔 기차 진행 방향")]
    [SerializeField]
    private SpringTrainDirection movingDirection =
        SpringTrainDirection.Clockwise;


    [Header("충돌 탐색 안전 제한 시간")]
    [SerializeField]
    private float maximumTravelTime =
        30f;


    [Header("충돌 연출 완료 후 태엽 등장 대기")]
    [SerializeField]
    private float springRevealDelay =
        0.2f;


    // =========================================================
    // 상태
    // =========================================================

    private bool isSequenceStarted;


    public bool IsSequenceComplete
    {
        get;
        private set;
    }


    public event Action OnSequenceCompleted;


    // =========================================================
    // Unity
    // =========================================================

    private void OnEnable()
    {
        if (railStateManager != null)
        {
            railStateManager.OnRailStateChanged +=
                HandleRailStateChanged;
        }
    }


    private void OnDisable()
    {
        if (railStateManager != null)
        {
            railStateManager.OnRailStateChanged -=
                HandleRailStateChanged;
        }
    }


    private void Start()
    {
        TryStartTrainSequence();
    }


    // =========================================================
    // Rail 정답
    // =========================================================

    private void HandleRailStateChanged(
        bool isCorrect
    )
    {
        if (!isCorrect)
        {
            return;
        }


        TryStartTrainSequence();
    }


    // =========================================================
    // Sequence 시작
    // =========================================================

    private void TryStartTrainSequence()
    {
        if (
            isSequenceStarted ||
            IsSequenceComplete
        )
        {
            return;
        }


        if (
            railStateManager == null ||
            railDataManager == null ||
            movingTrain == null ||
            blackTrain == null ||
            movingTrainPhysics == null ||
            blackTrainPhysics == null
        )
        {
            Debug.LogWarning(
                "[SpringTrainSequence] Inspector 연결을 확인하세요.",
                this
            );


            return;
        }


        if (
            !railStateManager.IsAllRailsCorrect
        )
        {
            return;
        }


        isSequenceStarted =
            true;


        railStateManager.LockInteraction();


        StartCoroutine(
            PlaySequence()
        );
    }


    // =========================================================
    // 전체 연출
    // =========================================================

    private IEnumerator PlaySequence()
    {
        Debug.Log(
            "[SpringTrainSequence] 정답 → Sequence 시작"
        );


        // =====================================================
        // 1. 선로 정답 위치 정확히 보정
        // =====================================================

        railDataManager
            .SnapAllRailsToCorrectRotation();


        yield return null;


        // =====================================================
        // 2. 현재 기차 위치를 완성 Path에 등록
        // =====================================================

        if (
            !movingTrain.SnapTrainToCurrentPath() ||
            !blackTrain.SnapTrainToCurrentPath()
        )
        {
            Debug.LogError(
                "[SpringTrainSequence] 기차 Path 등록 실패",
                this
            );


            isSequenceStarted =
                false;


            yield break;
        }


        // =====================================================
        // 3. Physics 준비
        // =====================================================

        movingTrainPhysics
            .PrepareForPathMovement();


        blackTrainPhysics
            .PrepareForStationaryCollision();


        yield return null;


        // =====================================================
        // 4. 색깔 기차 출발
        // =====================================================

        Debug.Log(
            "[SpringTrainSequence] 색깔 기차 출발"
        );


        yield return
            movingTrain
                .MoveUntilPhysicalCollision(
                    movingDirection,
                    maximumTravelTime
                );


        // =====================================================
        // 5. 실제 Collision 확인
        // =====================================================

        if (!movingTrain.DidCollide)
        {
            Debug.LogError(
                "[SpringTrainSequence] 실제 기차 충돌이 발생하지 않았습니다.",
                this
            );


            movingTrainPhysics
                .PrepareIdleLocked();


            blackTrainPhysics
                .PrepareIdleLocked();


            isSequenceStarted =
                false;


            yield break;
        }


        Debug.Log(
            "[SpringTrainSequence] 실제 기차 충돌 확인"
        );


        // =====================================================
        // 6. 두 기차 모두 10 units 이동 완료 대기
        // =====================================================

        while (
            !movingTrainPhysics.IsReactionFinished ||
            !blackTrainPhysics.IsReactionFinished
        )
        {
            yield return null;
        }


        Debug.Log(
            "[SpringTrainSequence] 두 기차 충돌 연출 완료"
        );


        // =====================================================
        // 7. 태엽
        // =====================================================

        if (springRevealDelay > 0f)
        {
            yield return new WaitForSeconds(
                springRevealDelay
            );
        }


        if (springPickupState != null)
        {
            springPickupState
                .RevealSpring();
        }


        // =====================================================
        // 완료
        // =====================================================

        IsSequenceComplete =
            true;


        OnSequenceCompleted?.Invoke();


        Debug.Log(
            "[SpringTrainSequence] Sequence 완료"
        );
    }


    private void OnValidate()
    {
        if (maximumTravelTime < 0f)
        {
            maximumTravelTime = 0f;
        }


        if (springRevealDelay < 0f)
        {
            springRevealDelay = 0f;
        }
    }
}