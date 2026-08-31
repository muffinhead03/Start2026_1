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
    // Trains
    // =========================================================

    [Header("컬러 기차")]
    [SerializeField]
    private SpringTrainPathMover movingTrain;


    [Header("흑백 기차")]
    [SerializeField]
    private SpringTrainPathMover blackTrain;


    // =========================================================
    // 흑백 기차 출발 Trigger
    // =========================================================

    [Header("흑백 기차 출발 Point")]

    [Tooltip(
        "컬러 기차가 이 Point를 지나면 " +
        "흑백 기차가 움직이기 시작합니다."
    )]

    [Range(0, 100)]
    [SerializeField]
    private int blackTrainStartPointIndex =
        30;


    [Header("어느 컬러 기차 칸 기준인지")]
    [SerializeField]
    private SpringTrainCarSlot triggerCar =
        SpringTrainCarSlot.Front;


    // =========================================================
    // Spring
    // =========================================================

    [Header("태엽")]
    [SerializeField]
    private SpringPickupState springPickupState;


    [Header("기차 연출 완료 후 태엽 등장 대기")]
    [SerializeField]
    private float springRevealDelay =
        0.2f;


    // =========================================================
    // State
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
        // 게임 시작 시 두 기차를
        // 각자의 Path 시작 위치에 정확히 배치
        if (movingTrain != null)
        {
            movingTrain
                .SnapTrainToStart();
        }


        if (blackTrain != null)
        {
            blackTrain
                .SnapTrainToStart();
        }


        TryStartTrainSequence();
    }


    // =========================================================
    // Rail State
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
            movingTrain == null ||
            blackTrain == null
        )
        {
            Debug.LogWarning(
                "[SpringTrainSequence] " +
                "Inspector 연결을 확인하세요.",
                this
            );


            return;
        }


        if (
            !railStateManager
                .IsAllRailsCorrect
        )
        {
            return;
        }


        // -----------------------------------------------------
        // Route 사전 검증
        // -----------------------------------------------------

        if (!movingTrain.CanStartRoute)
        {
            Debug.LogError(
                "[SpringTrainSequence] " +
                "컬러 기차 Route 설정 오류",
                this
            );


            return;
        }


        if (!blackTrain.CanStartRoute)
        {
            Debug.LogError(
                "[SpringTrainSequence] " +
                "흑백 기차 Route 설정 오류",
                this
            );


            return;
        }


        if (
            movingTrain.Track == null ||
            blackTrainStartPointIndex >
            movingTrain.Track.LastPointIndex
        )
        {
            Debug.LogError(
                "[SpringTrainSequence] " +
                "흑백 기차 시작 Point Index가 " +
                "컬러 Path 범위를 벗어났습니다.",
                this
            );


            return;
        }


        isSequenceStarted =
            true;


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
            "[SpringTrainSequence] " +
            "레일 정답 → 기차 Sequence 시작"
        );


        // =====================================================
        // 1. 레일 정답 Rotation 보정
        // =====================================================

        if (railDataManager != null)
        {
            railDataManager
                .SnapAllRailsToCorrectRotation();
        }


        yield return null;


        // =====================================================
        // 2. 기차 시작 위치 초기화
        // =====================================================

        movingTrain
            .SnapTrainToStart();


        blackTrain
            .SnapTrainToStart();


        // =====================================================
        // 3. 컬러 기차 출발
        // =====================================================

        if (
            !movingTrain
                .StartRouteFromStart()
        )
        {
            Debug.LogError(
                "[SpringTrainSequence] " +
                "컬러 기차 출발 실패",
                this
            );


            isSequenceStarted =
                false;


            yield break;
        }


        // 컬러 기차가 출발한 이후
        // 레일 버튼 잠금
        railStateManager
            .LockInteraction();


        Debug.Log(
            "[SpringTrainSequence] " +
            "컬러 기차 출발"
        );


        // =====================================================
        // 4. 지정된 충돌 Point까지 대기
        // =====================================================

        while (
            !movingTrain.HasCarReachedPoint(
                blackTrainStartPointIndex,
                triggerCar
            )
        )
        {
            // Point에 도착하기 전에
            // Route가 끝나면 잘못된 설정
            if (
                movingTrain
                    .IsRouteComplete
            )
            {
                Debug.LogError(
                    "[SpringTrainSequence] " +
                    "흑백 기차 출발 Point에 도달하기 전에 " +
                    "컬러 Route가 끝났습니다.",
                    this
                );


                yield break;
            }


            yield return null;
        }


        Debug.Log(
            "[SpringTrainSequence] " +
            $"충돌 연출 Point 도달 : " +
            $"{blackTrainStartPointIndex}"
        );


        // =====================================================
        // 5. 흑백 기차 출발
        //
        // 컬러 기차는 멈추지 않고 계속 감.
        // =====================================================

        if (
            !blackTrain
                .StartRouteFromStart()
        )
        {
            Debug.LogError(
                "[SpringTrainSequence] " +
                "흑백 기차 출발 실패",
                this
            );


            yield break;
        }


        Debug.Log(
            "[SpringTrainSequence] " +
            "흑백 기차 출발"
        );


        // =====================================================
        // 6. 두 기차 Route 완료 대기
        // =====================================================

        while (
            !movingTrain.IsRouteComplete ||
            !blackTrain.IsRouteComplete
        )
        {
            yield return null;
        }


        Debug.Log(
            "[SpringTrainSequence] " +
            "두 기차 이동 완료"
        );


        // =====================================================
        // 7. 태엽 등장
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
        // 8. 완료
        // =====================================================

        IsSequenceComplete =
            true;


        OnSequenceCompleted?.Invoke();


        Debug.Log(
            "[SpringTrainSequence] " +
            "Sequence 완료"
        );
    }


    // =========================================================
    // Inspector
    // =========================================================

    private void OnValidate()
    {
        if (springRevealDelay < 0f)
        {
            springRevealDelay =
                0f;
        }


        blackTrainStartPointIndex =
            Mathf.Clamp(
                blackTrainStartPointIndex,
                0,
                100
            );
    }
}