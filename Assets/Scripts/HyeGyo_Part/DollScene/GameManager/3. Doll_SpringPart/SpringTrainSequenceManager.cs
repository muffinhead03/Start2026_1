using System;
using System.Collections;
using UnityEngine;


public class SpringTrainSequenceManager : MonoBehaviour
{
    // =========================================================
    // 선로 정답 Manager
    // =========================================================

    [Header("선로 정답 Manager")]
    [SerializeField]
    private SpringRailStateManager railStateManager;


    // =========================================================
    // 선로 Data Manager
    // =========================================================

    [Header("선로 데이터 Manager")]
    [SerializeField]
    private SpringRailDataManager railDataManager;


    // =========================================================
    // 기차
    // =========================================================

    [Header("정답 후 움직이기 시작하는 색깔 기차")]
    [SerializeField]
    private SpringTrainPathMover movingTrain;


    [Header("충돌 대상 검은 기차")]
    [SerializeField]
    private SpringTrainPathMover blackTrain;


    // =========================================================
    // 태엽
    // =========================================================

    [Header("태엽")]
    [SerializeField]
    private SpringPickupState springPickupState;


    // =========================================================
    // 기차 진행
    // =========================================================

    [Header("색깔 기차 진행 방향")]
    [SerializeField]
    private SpringTrainDirection movingDirection =
        SpringTrainDirection.Clockwise;


    [Header("충돌까지 최대 주행 시간")]
    [Tooltip(
        "색깔 기차가 검은 기차와 충돌할 때까지 " +
        "최대 몇 초 동안 주행할지 설정합니다."
    )]
    [SerializeField]
    private float travelDuration = 10f;


    // =========================================================
    // 충돌 후 반동
    // =========================================================

    [Header("충돌 후 색깔 기차 반동 칸")]
    [SerializeField]
    private int movingTrainReactionSteps = 2;


    [Header("충돌 후 검은 기차 밀려나는 칸")]
    [SerializeField]
    private int blackTrainReactionSteps = 3;


    [Header("충돌 후 잠깐 대기")]
    [SerializeField]
    private float collisionPause = 0.15f;


    [Header("태엽 등장 전 대기")]
    [SerializeField]
    private float springRevealDelay = 0.2f;


    // =========================================================
    // 상태
    // =========================================================

    private bool isSequenceStarted = false;


    public bool IsSequenceComplete
    {
        get;
        private set;
    }


    public event Action OnSequenceCompleted;


    // =========================================================
    // Event 연결
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


    // =========================================================
    // 시작 시 이미 정답인지 확인
    // =========================================================

    private void Start()
    {
        TryStartTrainSequence();
    }


    // =========================================================
    // 선로 정답 상태 변경
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
    // 전체 선로가 정답일 때만 Sequence 시작
    // =========================================================

    private void TryStartTrainSequence()
    {
        if (isSequenceStarted)
        {
            return;
        }


        if (IsSequenceComplete)
        {
            return;
        }


        if (railStateManager == null)
        {
            Debug.LogWarning(
                "[SpringTrainSequence] RailStateManager가 연결되지 않았습니다.",
                this
            );

            return;
        }


        if (railDataManager == null)
        {
            Debug.LogWarning(
                "[SpringTrainSequence] RailDataManager가 연결되지 않았습니다.",
                this
            );

            return;
        }


        if (
            movingTrain == null ||
            blackTrain == null
        )
        {
            Debug.LogWarning(
                "[SpringTrainSequence] 색깔 기차 또는 검은 기차가 연결되지 않았습니다.",
                this
            );

            return;
        }


        if (springPickupState == null)
        {
            Debug.LogWarning(
                "[SpringTrainSequence] SpringPickupState가 연결되지 않았습니다.",
                this
            );

            return;
        }


        // 아직 모든 선로가 정답이 아님
        if (!railStateManager.IsAllRailsCorrect)
        {
            return;
        }


        // 중복 실행 방지
        isSequenceStarted = true;


        // 기차 연출이 시작되면
        // 더 이상 선로를 조작할 수 없도록 잠금
        railStateManager.LockInteraction();


        StartCoroutine(
            PlayTrainSequence()
        );
    }


    // =========================================================
    // 전체 기차 연출
    // =========================================================

    private IEnumerator PlayTrainSequence()
    {
        Debug.Log(
            "[SpringTrainSequence] 전체 선로 정답 → 기차 Sequence 시작"
        );


        // =====================================================
        // 1.
        // 모든 선로를 정확한 Correct Rotation으로 보정
        //
        // RotationTolerance 때문에
        // 약간의 오차가 있어도 정답으로 판정될 수 있으므로
        // 기차가 출발하기 직전에 정확하게 Snap한다.
        // =====================================================

        railDataManager.SnapAllRailsToCorrectRotation();


        Debug.Log(
            "[SpringTrainSequence] 선로 정답 위치 보정 완료"
        );


        // Transform / 자식 HelpPoint의
        // World Position이 갱신될 시간을 한 프레임 준다.
        yield return null;


        // =====================================================
        // 2.
        // 완성된 선로 Path에
        // 색깔 기차와 검은 기차를 정확히 맞춤
        //
        // 퍼즐을 푸는 동안에는 기차가 현재 위치에 고정되어 있다가
        // 이 시점에만 Path 시스템에 맞춰진다.
        // =====================================================

        bool movingTrainSnapped =
            movingTrain.SnapTrainToCurrentPath();


        bool blackTrainSnapped =
            blackTrain.SnapTrainToCurrentPath();


        if (
            !movingTrainSnapped ||
            !blackTrainSnapped
        )
        {
            Debug.LogError(
                "[SpringTrainSequence] 기차를 Path에 맞추지 못했습니다. " +
                "Track / FormationController / TrainCenter 설정을 확인하세요.",
                this
            );


            isSequenceStarted = false;

            yield break;
        }


        Debug.Log(
            "[SpringTrainSequence] 두 기차 Path 보정 완료"
        );


        // 한 프레임 대기
        yield return null;


        // =====================================================
        // 3.
        // 색깔 기차 출발
        //
        // 최대 travelDuration초 동안
        // 완성된 Path를 따라 움직인다.
        //
        // 검은 기차와 먼저 충돌하면
        // 즉시 이동을 멈춘다.
        // =====================================================

        Debug.Log(
            "[SpringTrainSequence] 색깔 기차 출발"
        );


        yield return
            movingTrain.MoveUntilCollisionForDuration(
                movingDirection,
                blackTrain,
                travelDuration
            );


        // =====================================================
        // 4.
        // 충돌 확인
        // =====================================================

        if (!movingTrain.DidCollide)
        {
            Debug.LogError(
                "[SpringTrainSequence] " +
                travelDuration +
                "초 안에 검은 기차와 충돌하지 못했습니다. " +
                "Moving Direction / Move Speed / Collision Radius / " +
                "기차 초기 위치를 확인하세요.",
                this
            );


            isSequenceStarted = false;

            yield break;
        }


        Debug.Log(
            "[SpringTrainSequence] 기차 충돌"
        );


        // =====================================================
        // 5.
        // 충돌 후 잠깐 정지
        // =====================================================

        if (collisionPause > 0f)
        {
            yield return new WaitForSeconds(
                collisionPause
            );
        }


        // =====================================================
        // 6.
        // 충돌 후 반동 방향
        //
        // 색깔 기차:
        // 원래 진행 방향의 반대 방향
        //
        // 검은 기차:
        // 색깔 기차가 진행하던 방향
        //
        //
        // 예:
        //
        //       충돌 전
        //
        // [색깔] -----> [검은]
        //
        //
        //       충돌 후
        //
        // <---- [색깔]     [검은] ----->
        // =====================================================

        SpringTrainDirection
            movingReactionDirection =
                GetOppositeDirection(
                    movingDirection
                );


        SpringTrainDirection
            blackReactionDirection =
                movingDirection;


        bool movingTrainDone = false;

        bool blackTrainDone = false;


        // =====================================================
        // 색깔 기차 반동
        // =====================================================

        StartCoroutine(
            MoveReaction(
                movingTrain,
                movingTrainReactionSteps,
                movingReactionDirection,
                () =>
                    movingTrainDone = true
            )
        );


        // =====================================================
        // 검은 기차 밀려남
        // =====================================================

        StartCoroutine(
            MoveReaction(
                blackTrain,
                blackTrainReactionSteps,
                blackReactionDirection,
                () =>
                    blackTrainDone = true
            )
        );


        // =====================================================
        // 두 기차가 모두 이동을 끝낼 때까지 대기
        // =====================================================

        while (
            !movingTrainDone ||
            !blackTrainDone
        )
        {
            yield return null;
        }


        Debug.Log(
            "[SpringTrainSequence] 충돌 후 반동 연출 완료"
        );


        // =====================================================
        // 7.
        // 태엽 등장 전 잠깐 대기
        // =====================================================

        if (springRevealDelay > 0f)
        {
            yield return new WaitForSeconds(
                springRevealDelay
            );
        }


        // =====================================================
        // 8.
        // 태엽 공개
        // =====================================================

        springPickupState.RevealSpring();


        Debug.Log(
            "[SpringTrainSequence] 태엽 공개"
        );


        // =====================================================
        // 9.
        // Sequence 완료
        // =====================================================

        IsSequenceComplete = true;


        Debug.Log(
            "[SpringTrainSequence] 기차 Sequence 완료"
        );


        OnSequenceCompleted?.Invoke();
    }


    // =========================================================
    // 충돌 후 기차 Rail 단위 이동
    // =========================================================

    private IEnumerator MoveReaction(
        SpringTrainPathMover train,
        int steps,
        SpringTrainDirection direction,
        Action onComplete
    )
    {
        if (
            train != null &&
            steps > 0
        )
        {
            yield return
                train.MoveRailSteps(
                    steps,
                    direction
                );
        }


        onComplete?.Invoke();
    }


    // =========================================================
    // 반대 방향
    // =========================================================

    private SpringTrainDirection GetOppositeDirection(
        SpringTrainDirection direction
    )
    {
        if (
            direction ==
            SpringTrainDirection.Clockwise
        )
        {
            return
                SpringTrainDirection.CounterClockwise;
        }


        return
            SpringTrainDirection.Clockwise;
    }


    // =========================================================
    // Inspector 검사
    // =========================================================

    private void OnValidate()
    {
        if (travelDuration < 0f)
        {
            travelDuration = 0f;
        }


        if (movingTrainReactionSteps < 0)
        {
            movingTrainReactionSteps = 0;
        }


        if (blackTrainReactionSteps < 0)
        {
            blackTrainReactionSteps = 0;
        }


        if (collisionPause < 0f)
        {
            collisionPause = 0f;
        }


        if (springRevealDelay < 0f)
        {
            springRevealDelay = 0f;
        }
    }
}