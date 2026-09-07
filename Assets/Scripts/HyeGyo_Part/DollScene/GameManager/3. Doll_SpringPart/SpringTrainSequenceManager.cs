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
    // Collision A
    // =========================================================

    [Header("컬러 기차 Collision A Point")]

    [Tooltip(
        "기존 blackTrainStartPointIndex 값을 그대로 사용합니다.\n" +
        "이제는 컬러 기차 Middle이 충돌 위치에 도달하는 Point입니다."
    )]

    [Range(0, 100)]
    [SerializeField]
    private int blackTrainStartPointIndex =
        30;


    [Header("흑백 기차 Collision A Point")]

    [Tooltip(
        "흑백 기차 Middle이 충돌 위치에 도달하는 Point입니다."
    )]

    [Range(0, 100)]
    [SerializeField]
    private int blackCollisionPointIndex =
        10;


    // =========================================================
    // 기존 Inspector 데이터 유지
    //
    // 더 이상 실제 판정에는 사용하지 않음
    // =========================================================

    [Header("기존 Car Trigger 설정 - 호환용")]
    [SerializeField]
    private SpringTrainCarSlot triggerCar =
        SpringTrainCarSlot.Front;


    // =========================================================
    // Spring
    // =========================================================

    [Header("태엽")]
    [SerializeField]
    private SpringPickupState springPickupState;


    [Header("기차 정지 후 태엽 등장 대기")]
    [SerializeField]
    private float springRevealDelay =
        0f;


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


        // 아직 정답이 아님
        if (!railStateManager.IsAllRailsCorrect)
        {
            return;
        }


        if (!movingTrain.CanStartRoute)
        {
            Debug.LogError(
                "[SpringTrainSequence] " +
                "컬러 기차 Path 설정 오류",
                this
            );


            return;
        }


        if (!blackTrain.CanStartRoute)
        {
            Debug.LogError(
                "[SpringTrainSequence] " +
                "흑백 기차 Path 설정 오류",
                this
            );


            return;
        }


        if (!ValidatePoints())
        {
            return;
        }


        // =====================================================
        // 여기서 바로 잠금
        //
        // 정답 이후에는 인형을 눌러도
        // 선로가 더 이상 움직이지 않음
        // =====================================================

        railStateManager
            .LockInteraction();


        isSequenceStarted =
            true;


        StartCoroutine(
            PlaySequence()
        );
    }


    // =========================================================
    // Point 검사
    // =========================================================

    private bool ValidatePoints()
    {
        if (
            movingTrain.Track == null ||
            blackTrain.Track == null
        )
        {
            return false;
        }


        // 컬러 Collision A
        if (
            blackTrainStartPointIndex < 0 ||
            blackTrainStartPointIndex >=
            movingTrain.Track.LastPointIndex
        )
        {
            Debug.LogError(
                "[SpringTrainSequence] " +
                "컬러 기차 Collision A Point를 확인하세요.",
                this
            );


            return false;
        }


        // 흑백 Collision A
        if (
            blackCollisionPointIndex < 0 ||
            blackCollisionPointIndex >=
            blackTrain.Track.LastPointIndex
        )
        {
            Debug.LogError(
                "[SpringTrainSequence] " +
                "흑백 기차 Collision A Point를 확인하세요.",
                this
            );


            return false;
        }


        return true;
    }


    // =========================================================
    // Sequence
    // =========================================================

    private IEnumerator PlaySequence()
    {
        Debug.Log(
            "[SpringTrainSequence] " +
            "정답 → Train Sequence 시작"
        );


        // =====================================================
        // 1. 정답 상태 Rotation 정확히 고정
        // =====================================================

        if (railDataManager != null)
        {
            railDataManager
                .SnapAllRailsToCorrectRotation();
        }


        yield return null;


        // =====================================================
        // 2. 기차 시작 위치
        // =====================================================

        if (
            !movingTrain.SnapTrainToStart() ||
            !blackTrain.SnapTrainToStart()
        )
        {
            Debug.LogError(
                "[SpringTrainSequence] " +
                "기차 시작 위치 초기화 실패",
                this
            );


            yield break;
        }


        // =====================================================
        // 3. 두 기차 동시에 Collision A로 이동
        // =====================================================

        bool colorStarted =
            movingTrain.MoveToPoint(
                blackTrainStartPointIndex
            );


        bool blackStarted =
            blackTrain.MoveToPoint(
                blackCollisionPointIndex
            );


        if (
            !colorStarted ||
            !blackStarted
        )
        {
            Debug.LogError(
                "[SpringTrainSequence] " +
                "Collision A 이동 시작 실패",
                this
            );


            yield break;
        }


        Debug.Log(
            "[SpringTrainSequence] " +
            "두 기차 Collision A 이동"
        );


        // =====================================================
        // 4. 두 기차 Middle이 A에 도착
        //
        // 이것을 실제 충돌 판정으로 사용
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
            "Collision 판정 완료"
        );


        // =====================================================
        // 5. Collision 이후
        //
        // 각 기차가 자신의 Track 마지막 Point까지 이동
        // =====================================================

        bool colorExitStarted =
            movingTrain.MoveToEnd();


        bool blackExitStarted =
            blackTrain.MoveToEnd();


        if (
            !colorExitStarted ||
            !blackExitStarted
        )
        {
            Debug.LogError(
                "[SpringTrainSequence] " +
                "충돌 후 이동 시작 실패",
                this
            );


            yield break;
        }


        Debug.Log(
            "[SpringTrainSequence] " +
            "충돌 후 기차 이동 시작"
        );


        // =====================================================
        // 6. 원하는 전체 기차 애니메이션 종료 대기
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
            "두 기차 최종 위치 도착 / 정지"
        );


        // =====================================================
        // 7. Spring Reveal
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


        Debug.Log(
            "[SpringTrainSequence] " +
            "Spring 공개"
        );


        // =====================================================
        // 8. Sequence 완료
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
        blackTrainStartPointIndex =
            Mathf.Max(
                0,
                blackTrainStartPointIndex
            );


        blackCollisionPointIndex =
            Mathf.Max(
                0,
                blackCollisionPointIndex
            );


        if (springRevealDelay < 0f)
        {
            springRevealDelay =
                0f;
        }
    }
}