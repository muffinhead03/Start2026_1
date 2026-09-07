using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;


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
    // Color Train Points
    // =========================================================

    [Header("컬러 기차 - 시작 Point")]
    [Tooltip("정답 상태가 되면 컬러 기차 Middle을 이 Point에 맞춘 뒤 출발합니다.")]
    [Range(0, 100)]
    [SerializeField]
    private int colorStartPointIndex = 0;


    [Header("컬러 기차 - A Point (흑백 기차 출발 Trigger)")]
    [Tooltip(
        "컬러 기차 Middle이 이 Point를 지나면 흑백 기차가 출발합니다.\n" +
        "Color Start < A < Color End 순서여야 합니다."
    )]
    [Range(0, 100)]
    [SerializeField]
    [FormerlySerializedAs("blackStartTriggerPointIndex")]
    [FormerlySerializedAs("blackTrainStartPointIndex")]
    private int colorTriggerPointIndex = 10;


    [Header("컬러 기차 - 마지막 Point")]
    [Tooltip("컬러 기차가 최종적으로 멈출 Point입니다.")]
    [Range(0, 100)]
    [SerializeField]
    private int colorEndPointIndex = 30;


    // =========================================================
    // Black Train Points
    // =========================================================

    [Header("흑백 기차 - 시작 Point")]
    [Tooltip("정답 상태가 되면 흑백 기차 Middle을 이 Point에 맞춰 대기시킵니다.")]
    [Range(0, 100)]
    [SerializeField]
    private int blackStartPointIndex = 0;


    [Header("흑백 기차 - 마지막 Point")]
    [Tooltip("컬러 기차가 A Point를 지나면 흑백 기차가 이 Point까지 이동합니다.")]
    [Range(0, 100)]
    [SerializeField]
    private int blackEndPointIndex = 30;


    // =========================================================
    // Spring
    // =========================================================

    [Header("태엽")]
    [SerializeField]
    private SpringPickupState springPickupState;


    [Header("기차 정지 후 태엽 등장 대기")]
    [SerializeField]
    private float springRevealDelay = 0f;


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
        // 게임 시작 시 기차 Transform은 건드리지 않습니다.
        // 이미 정답 상태로 시작하는 경우만 Sequence 시작을 시도합니다.
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
                "[SpringTrainSequence] Inspector 연결을 확인하세요.",
                this
            );

            return;
        }


        if (!railStateManager.IsAllRailsCorrect)
        {
            return;
        }


        if (!movingTrain.CanStartRoute)
        {
            Debug.LogError(
                "[SpringTrainSequence] 컬러 기차 Path 설정 오류",
                this
            );

            return;
        }


        if (!blackTrain.CanStartRoute)
        {
            Debug.LogError(
                "[SpringTrainSequence] 흑백 기차 Path 설정 오류",
                this
            );

            return;
        }


        if (!ValidatePoints())
        {
            return;
        }


        // 정답이 된 즉시 레일 조작 잠금
        railStateManager.LockInteraction();


        isSequenceStarted = true;


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
            Debug.LogError(
                "[SpringTrainSequence] Train Track 연결을 확인하세요.",
                this
            );

            return false;
        }


        int colorLast =
            movingTrain.Track.LastPointIndex;

        int blackLast =
            blackTrain.Track.LastPointIndex;


        // -----------------------------------------------------
        // 범위 검사
        // -----------------------------------------------------

        if (!IsPointInRange(colorStartPointIndex, colorLast))
        {
            LogPointError(
                "컬러 Start",
                colorStartPointIndex,
                colorLast
            );

            return false;
        }


        if (!IsPointInRange(colorTriggerPointIndex, colorLast))
        {
            LogPointError(
                "컬러 A Trigger",
                colorTriggerPointIndex,
                colorLast
            );

            return false;
        }


        if (!IsPointInRange(colorEndPointIndex, colorLast))
        {
            LogPointError(
                "컬러 End",
                colorEndPointIndex,
                colorLast
            );

            return false;
        }


        if (!IsPointInRange(blackStartPointIndex, blackLast))
        {
            LogPointError(
                "흑백 Start",
                blackStartPointIndex,
                blackLast
            );

            return false;
        }


        if (!IsPointInRange(blackEndPointIndex, blackLast))
        {
            LogPointError(
                "흑백 End",
                blackEndPointIndex,
                blackLast
            );

            return false;
        }


        // -----------------------------------------------------
        // 이동 순서 검사
        // 현재 PathMover는 작은 Index -> 큰 Index 방향 이동
        // -----------------------------------------------------

        if (!(
            colorStartPointIndex <
            colorTriggerPointIndex &&
            colorTriggerPointIndex <
            colorEndPointIndex
        ))
        {
            Debug.LogError(
                "[SpringTrainSequence] 컬러 Point 순서 오류. " +
                "Color Start < A Trigger < Color End 순서로 지정하세요.",
                this
            );

            return false;
        }


        if (!(
            blackStartPointIndex <
            blackEndPointIndex
        ))
        {
            Debug.LogError(
                "[SpringTrainSequence] 흑백 Point 순서 오류. " +
                "Black Start < Black End 순서로 지정하세요.",
                this
            );

            return false;
        }


        return true;
    }


    private bool IsPointInRange(
        int pointIndex,
        int lastPointIndex
    )
    {
        return
            pointIndex >= 0 &&
            pointIndex <= lastPointIndex;
    }


    private void LogPointError(
        string pointName,
        int pointIndex,
        int lastPointIndex
    )
    {
        Debug.LogError(
            "[SpringTrainSequence] " +
            $"{pointName} Point Index 오류 : {pointIndex} / " +
            $"사용 가능 0 ~ {lastPointIndex}",
            this
        );
    }


    // =========================================================
    // Sequence
    // =========================================================

    private IEnumerator PlaySequence()
    {
        Debug.Log(
            "[SpringTrainSequence] 정답 → Train Sequence 시작"
        );


        // =====================================================
        // 1. 정답 선로 Rotation 고정
        // =====================================================

        if (railDataManager != null)
        {
            railDataManager
                .SnapAllRailsToCorrectRotation();
        }


        yield return null;


        // =====================================================
        // 2. 두 기차를 각각 지정된 Start Point에 배치
        // =====================================================

        if (
            !movingTrain.SnapTrainToPoint(
                colorStartPointIndex
            ) ||
            !blackTrain.SnapTrainToPoint(
                blackStartPointIndex
            )
        )
        {
            FailSequence(
                "기차 Start Point 배치 실패"
            );

            yield break;
        }


        Debug.Log(
            "[SpringTrainSequence] " +
            $"Start 배치 완료 / " +
            $"Color:{colorStartPointIndex}, " +
            $"Black:{blackStartPointIndex}"
        );


        // =====================================================
        // 3. 컬러 기차는 처음부터 End까지 계속 이동
        // =====================================================

        if (!movingTrain.MoveToPoint(colorEndPointIndex))
        {
            FailSequence(
                "컬러 기차 출발 실패"
            );

            yield break;
        }


        Debug.Log(
            "[SpringTrainSequence] " +
            $"컬러 기차 출발 → End {colorEndPointIndex}"
        );


        // =====================================================
        // 4. 컬러 Middle이 A Point를 지나면 흑백 기차 출발
        // =====================================================

        while (
            !movingTrain.HasMiddleReachedPoint(
                colorTriggerPointIndex
            )
        )
        {
            if (!movingTrain.IsMoving)
            {
                FailSequence(
                    "컬러 기차가 A Trigger에 도달하지 못하고 정지했습니다."
                );

                yield break;
            }


            yield return null;
        }


        Debug.Log(
            "[SpringTrainSequence] " +
            $"컬러 기차 A 통과 : Point {colorTriggerPointIndex}"
        );


        // =====================================================
        // 5. 흑백 기차는 Start -> End까지 한 번에 이동
        // =====================================================

        if (!blackTrain.MoveToPoint(blackEndPointIndex))
        {
            FailSequence(
                "흑백 기차 출발 실패"
            );

            yield break;
        }


        Debug.Log(
            "[SpringTrainSequence] " +
            $"흑백 기차 출발 → End {blackEndPointIndex}"
        );


        // =====================================================
        // 6. 두 기차가 모두 End에 도착할 때까지 대기
        // =====================================================

        while (
            movingTrain.IsMoving ||
            blackTrain.IsMoving
        )
        {
            yield return null;
        }


        Debug.Log(
            "[SpringTrainSequence] " +
            $"두 기차 최종 정지 / " +
            $"Color:{colorEndPointIndex}, " +
            $"Black:{blackEndPointIndex}"
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


        // =====================================================
        // 8. Sequence 완료
        // =====================================================

        IsSequenceComplete = true;


        OnSequenceCompleted?.Invoke();


        Debug.Log(
            "[SpringTrainSequence] Sequence 완료 → Spring 공개"
        );
    }


    // =========================================================
    // Sequence 실패
    // =========================================================

    private void FailSequence(
        string message
    )
    {
        movingTrain?.StopMovementImmediately();
        blackTrain?.StopMovementImmediately();


        isSequenceStarted = false;


        Debug.LogError(
            "[SpringTrainSequence] " + message,
            this
        );
    }


    // =========================================================
    // Inspector
    // =========================================================

    private void OnValidate()
    {
        colorStartPointIndex =
            Mathf.Max(0, colorStartPointIndex);

        colorTriggerPointIndex =
            Mathf.Max(0, colorTriggerPointIndex);

        colorEndPointIndex =
            Mathf.Max(0, colorEndPointIndex);

        blackStartPointIndex =
            Mathf.Max(0, blackStartPointIndex);

        blackEndPointIndex =
            Mathf.Max(0, blackEndPointIndex);


        if (springRevealDelay < 0f)
        {
            springRevealDelay = 0f;
        }
    }
}