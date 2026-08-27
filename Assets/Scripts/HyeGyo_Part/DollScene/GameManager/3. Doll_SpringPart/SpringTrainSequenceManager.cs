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
    // 기차
    // =========================================================

    [Header("정답 후 움직이기 시작하는 기차")]
    [SerializeField]
    private SpringTrainPathMover movingTrain;


    [Header("충돌 대상 기차")]
    [SerializeField]
    private SpringTrainPathMover blackTrain;


    // =========================================================
    // 태엽
    // =========================================================

    [Header("태엽")]
    [SerializeField]
    private SpringPickupState springPickupState;


    // =========================================================
    // 기본 진행 방향
    // =========================================================

    [Header("움직이는 기차의 방향")]
    [SerializeField]
    private SpringTrainDirection movingDirection =
        SpringTrainDirection.Clockwise;


    [Header("충돌 탐색 최대 칸 수")]
    [SerializeField]
    private int maximumTravelSteps = 12;


    // =========================================================
    // 충돌 후
    // =========================================================

    [Header("충돌 후 움직이던 기차 반동 칸")]
    [SerializeField]
    private int movingTrainReactionSteps = 1;


    [Header("충돌 후 검은 기차 밀려나는 칸")]
    [SerializeField]
    private int blackTrainReactionSteps = 2;


    [Header("충돌 후 잠깐 대기")]
    [SerializeField]
    private float collisionPause = 0.15f;


    [Header("태엽 등장 전 대기")]
    [SerializeField]
    private float springRevealDelay = 0.2f;


    // =========================================================
    // 상태
    // =========================================================

    private bool isSequenceStarted =
        false;


    public bool IsSequenceComplete
    {
        get;
        private set;
    }


    public event Action OnSequenceCompleted;


    // =========================================================
    // Event
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
    // 선로 정답 변경
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
    // 정답일 때만 기차 출발
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
            return;
        }


        if (!railStateManager.IsAllRailsCorrect)
        {
            return;
        }


        if (movingTrain == null ||
            blackTrain == null)
        {
            Debug.LogWarning(
                "[SpringTrainSequence] 기차 연결이 안 되어 있습니다."
            );

            return;
        }


        // 기차 연출 중 선로 입력 잠금
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
        isSequenceStarted =
            true;


        movingTrain.RefreshCurrentRailIndex();

        blackTrain.RefreshCurrentRailIndex();


        Debug.Log(
            "[SpringTrainSequence] 정답 완료 → 기차 출발"
        );


        // =====================================================
        // 1.
        // 색깔 기차가 검은 기차와 충돌할 때까지 이동
        // =====================================================

        yield return movingTrain.MoveUntilCollision(
            movingDirection,
            blackTrain,
            maximumTravelSteps
        );


        if (!movingTrain.DidCollide)
        {
            Debug.LogWarning(
                "[SpringTrainSequence] 한 바퀴 안에 기차 충돌이 발생하지 않았습니다. " +
                "Train Center / Collision Radius / Track 순서를 확인하세요."
            );


            isSequenceStarted =
                false;


            yield break;
        }


        Debug.Log(
            "[SpringTrainSequence] 기차 충돌"
        );


        if (collisionPause > 0f)
        {
            yield return new WaitForSeconds(
                collisionPause
            );
        }


        // =====================================================
        // 2.
        // 충돌 후 두 기차가 서로 벌어짐
        //
        // 색깔 기차:
        // 원래 진행 방향의 반대로 1칸
        //
        // 검은 기차:
        // 색깔 기차가 진행하던 방향으로 2칸
        //
        //
        // 예:
        //
        // 충돌 전
        //
        // [색깔] ---> [검은]
        //
        //
        // 충돌 후
        //
        // <--- [색깔]     [검은] --->
        // =====================================================

        SpringTrainDirection movingReactionDirection =
            GetOppositeDirection(
                movingDirection
            );


        SpringTrainDirection blackReactionDirection =
            movingDirection;


        bool movingTrainDone =
            false;


        bool blackTrainDone =
            false;


        // 색깔 기차 반동
        StartCoroutine(
            MoveReaction(
                movingTrain,
                movingTrainReactionSteps,
                movingReactionDirection,
                () =>
                    movingTrainDone = true
            )
        );


        // 검은 기차 밀려남
        StartCoroutine(
            MoveReaction(
                blackTrain,
                blackTrainReactionSteps,
                blackReactionDirection,
                () =>
                    blackTrainDone = true
            )
        );


        // 두 기차가 모두 멈출 때까지 대기
        while (
            !movingTrainDone ||
            !blackTrainDone
        )
        {
            yield return null;
        }


        // =====================================================
        // 3.
        // 태엽 공개
        // =====================================================

        if (springRevealDelay > 0f)
        {
            yield return new WaitForSeconds(
                springRevealDelay
            );
        }


        if (springPickupState != null)
        {
            springPickupState.RevealSpring();
        }


        // =====================================================
        // Sequence 완료
        // =====================================================

        IsSequenceComplete =
            true;


        Debug.Log(
            "[SpringTrainSequence] 기차 연출 완료"
        );


        OnSequenceCompleted?.Invoke();
    }


    // =========================================================
    // 충돌 반동 이동
    // =========================================================

    private IEnumerator MoveReaction(
        SpringTrainPathMover train,
        int steps,
        SpringTrainDirection direction,
        Action onComplete
    )
    {
        if (train != null)
        {
            yield return train.MoveRailSteps(
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
}