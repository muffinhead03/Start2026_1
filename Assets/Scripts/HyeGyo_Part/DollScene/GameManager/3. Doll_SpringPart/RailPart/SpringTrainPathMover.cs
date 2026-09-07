using System;
using System.Collections;
using UnityEngine;


public class SpringTrainPathMover : MonoBehaviour
{
    // =========================================================
    // Inspector
    // =========================================================

    [Header("수동 기차 Path")]
    [SerializeField]
    private SpringTrainTrack track;


    [Header("기차 3칸 Formation")]
    [SerializeField]
    private SpringTrainFormationController formationController;


    [Header("이동 속도")]
    [SerializeField]
    private float moveSpeed = 2f;


    // =========================================================
    // Runtime
    // =========================================================

    private Coroutine moveCoroutine;
    private float targetDistance;


    public float CurrentMiddleDistance
    {
        get;
        private set;
    }


    public bool IsMoving
    {
        get;
        private set;
    }


    // 기존 외부 코드 호환용.
    // 현재 SequenceManager는 IsMoving을 사용합니다.
    public bool IsRouteComplete
    {
        get;
        private set;
    }


    public SpringTrainTrack Track =>
        track;


    public Transform TrainCenter =>
        formationController != null
            ? formationController.TrainCenter
            : null;


    public event Action OnRouteCompleted;


    // =========================================================
    // 준비 여부
    // =========================================================

    public bool CanStartRoute =>
        ValidateSetup();


    // 기존 API 호환용
    public bool CanMove =>
        CanStartRoute;


    private bool ValidateSetup()
    {
        if (track == null)
        {
            Debug.LogError(
                "[SpringTrainPathMover] Track이 연결되지 않았습니다.",
                this
            );

            return false;
        }


        if (formationController == null)
        {
            Debug.LogError(
                "[SpringTrainPathMover] FormationController가 연결되지 않았습니다.",
                this
            );

            return false;
        }


        if (!formationController.HasAllCars)
        {
            Debug.LogError(
                "[SpringTrainPathMover] Front / Middle / Rear Car 연결을 확인하세요.",
                this
            );

            return false;
        }


        if (moveSpeed <= 0f)
        {
            Debug.LogError(
                "[SpringTrainPathMover] MoveSpeed는 0보다 커야 합니다.",
                this
            );

            return false;
        }


        if (!track.RebuildCache())
        {
            Debug.LogError(
                "[SpringTrainPathMover] Track Point 설정을 확인하세요.",
                this
            );

            return false;
        }


        return true;
    }


    // =========================================================
    // 즉시 배치
    // =========================================================

    public bool SnapTrainToPoint(
        int pointIndex
    )
    {
        if (!ValidateSetup())
        {
            return false;
        }


        if (!IsValidPointIndex(pointIndex))
        {
            LogPointRangeError(
                "Snap",
                pointIndex
            );

            return false;
        }


        StopMovementImmediately();


        CurrentMiddleDistance =
            track.GetDistanceAtPoint(
                pointIndex
            );


        targetDistance =
            CurrentMiddleDistance;


        ApplyCurrentFormation();


        IsRouteComplete = false;


        return true;
    }


    // =========================================================
    // Point까지 이동
    // =========================================================

    public bool MoveToPoint(
        int pointIndex
    )
    {
        if (!ValidateSetup())
        {
            return false;
        }


        if (!IsValidPointIndex(pointIndex))
        {
            LogPointRangeError(
                "Move",
                pointIndex
            );

            return false;
        }


        float nextTargetDistance =
            track.GetDistanceAtPoint(
                pointIndex
            );


        if (
            nextTargetDistance <
            CurrentMiddleDistance - 0.001f
        )
        {
            Debug.LogError(
                "[SpringTrainPathMover] " +
                $"현재 위치보다 이전 Point로 이동할 수 없습니다. " +
                $"CurrentDistance:{CurrentMiddleDistance:F3}, " +
                $"TargetPoint:{pointIndex}, " +
                $"TargetDistance:{nextTargetDistance:F3}",
                this
            );

            return false;
        }


        StartMoveToDistance(
            nextTargetDistance
        );


        return true;
    }


    private void StartMoveToDistance(
        float distance
    )
    {
        if (moveCoroutine != null)
        {
            StopCoroutine(
                moveCoroutine
            );

            moveCoroutine = null;
        }


        targetDistance =
            distance;


        IsRouteComplete = false;


        if (
            Mathf.Abs(
                CurrentMiddleDistance -
                targetDistance
            ) <= 0.001f
        )
        {
            CurrentMiddleDistance =
                targetDistance;


            ApplyCurrentFormation();


            IsMoving = false;
            IsRouteComplete = true;


            OnRouteCompleted?.Invoke();

            return;
        }


        moveCoroutine =
            StartCoroutine(
                MoveRoutine()
            );
    }


    // =========================================================
    // 실제 이동
    // =========================================================

    private IEnumerator MoveRoutine()
    {
        IsMoving = true;


        while (
            CurrentMiddleDistance <
            targetDistance
        )
        {
            CurrentMiddleDistance =
                Mathf.MoveTowards(
                    CurrentMiddleDistance,
                    targetDistance,
                    moveSpeed * Time.deltaTime
                );


            ApplyCurrentFormation();


            yield return null;
        }


        CurrentMiddleDistance =
            targetDistance;


        ApplyCurrentFormation();


        IsMoving = false;
        IsRouteComplete = true;
        moveCoroutine = null;


        OnRouteCompleted?.Invoke();
    }


    private void ApplyCurrentFormation()
    {
        formationController.ApplyFormation(
            track,
            CurrentMiddleDistance
        );
    }


    // =========================================================
    // Point 통과 여부
    // =========================================================

    public bool HasMiddleReachedPoint(
        int pointIndex
    )
    {
        if (
            track == null ||
            !IsValidPointIndex(pointIndex)
        )
        {
            return false;
        }


        float pointDistance =
            track.GetDistanceAtPoint(
                pointIndex
            );


        return
            CurrentMiddleDistance >=
            pointDistance;
    }


    // =========================================================
    // 즉시 정지
    // =========================================================

    public void StopMovementImmediately()
    {
        if (moveCoroutine != null)
        {
            StopCoroutine(
                moveCoroutine
            );

            moveCoroutine = null;
        }


        IsMoving = false;
    }


    // =========================================================
    // 기존 API 호환용
    // =========================================================

    public bool SnapTrainToStart()
    {
        return SnapTrainToPoint(0);
    }


    public bool StartRouteFromStart()
    {
        if (!SnapTrainToStart())
        {
            return false;
        }


        return MoveToPoint(
            track.LastPointIndex
        );
    }


    public bool MoveToEnd()
    {
        if (track == null)
        {
            return false;
        }


        return MoveToPoint(
            track.LastPointIndex
        );
    }


    public bool HasCarReachedPoint(
        int pointIndex,
        SpringTrainCarSlot carSlot
    )
    {
        if (
            track == null ||
            formationController == null ||
            !IsValidPointIndex(pointIndex)
        )
        {
            return false;
        }


        float pointDistance =
            track.GetDistanceAtPoint(
                pointIndex
            );


        float carDistance =
            CurrentMiddleDistance +
            formationController.GetDistanceOffset(
                carSlot
            );


        return
            carDistance >=
            pointDistance;
    }


    // =========================================================
    // Utility
    // =========================================================

    private bool IsValidPointIndex(
        int pointIndex
    )
    {
        return
            track != null &&
            pointIndex >= 0 &&
            pointIndex <= track.LastPointIndex;
    }


    private void LogPointRangeError(
        string actionName,
        int pointIndex
    )
    {
        Debug.LogError(
            "[SpringTrainPathMover] " +
            $"{actionName} Point Index 오류 : {pointIndex} / " +
            $"사용 가능 0 ~ {track.LastPointIndex}",
            this
        );
    }


    // =========================================================
    // Inspector
    // =========================================================

    private void OnValidate()
    {
        if (moveSpeed < 0f)
        {
            moveSpeed = 0f;
        }
    }
}
