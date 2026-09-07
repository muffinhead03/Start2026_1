using System;
using System.Collections;
using UnityEngine;


public class SpringTrainPathMover : MonoBehaviour
{
    // =========================================================
    // Path
    // =========================================================

    [Header("수동 기차 Path")]
    [SerializeField]
    private SpringTrainTrack track;


    // =========================================================
    // Formation
    // =========================================================

    [Header("기차 3칸 Formation")]
    [SerializeField]
    private SpringTrainFormationController formationController;


    // =========================================================
    // Movement
    // =========================================================

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


    // =========================================================
    // 기존 API 유지
    // =========================================================

    public bool IsRouteComplete
    {
        get;
        private set;
    }


    public SpringTrainTrack Track =>
        track;


    public Transform TrainCenter
    {
        get
        {
            if (formationController == null)
            {
                return null;
            }


            return formationController.TrainCenter;
        }
    }


    public event Action OnRouteCompleted;


    // =========================================================
    // 이동 가능 여부
    // =========================================================

    public bool CanStartRoute
    {
        get
        {
            if (
                track == null ||
                formationController == null ||
                !formationController.HasAllCars ||
                moveSpeed <= 0f
            )
            {
                return false;
            }


            return track.RebuildCache();
        }
    }


    // 새 이름도 사용 가능
    public bool CanMove =>
        CanStartRoute;


    // =========================================================
    // 시작 위치
    // =========================================================

    public bool SnapTrainToStart()
    {
        if (!CanStartRoute)
        {
            Debug.LogWarning(
                "[SpringTrainPathMover] " +
                "Track / Formation 설정을 확인하세요.",
                this
            );


            return false;
        }


        StopMovementImmediately();


        CurrentMiddleDistance = 0f;

        targetDistance = 0f;


        formationController.ApplyFormation(
            track,
            CurrentMiddleDistance
        );


        IsMoving = false;

        IsRouteComplete = false;


        return true;
    }


    // =========================================================
    // 기존 방식
    //
    // 처음부터 Track 마지막까지 이동
    // =========================================================

    public bool StartRouteFromStart()
    {
        if (!SnapTrainToStart())
        {
            return false;
        }


        return MoveToDistance(
            track.TotalLength
        );
    }


    // =========================================================
    // 현재 위치에서 특정 Point까지 이동
    // =========================================================

    public bool MoveToPoint(
        int pointIndex
    )
    {
        if (!CanStartRoute)
        {
            return false;
        }


        if (
            pointIndex < 0 ||
            pointIndex > track.LastPointIndex
        )
        {
            Debug.LogError(
                "[SpringTrainPathMover] " +
                $"Point Index 범위 오류 : {pointIndex}",
                this
            );


            return false;
        }


        float distance =
            track.GetDistanceAtPoint(
                pointIndex
            );


        return MoveToDistance(
            distance
        );
    }


    // =========================================================
    // 현재 위치에서 Track 마지막까지 이동
    // =========================================================

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


    // =========================================================
    // 현재 위치 → 지정 Distance
    // =========================================================

    public bool MoveToDistance(
        float distance
    )
    {
        if (!CanStartRoute)
        {
            return false;
        }


        distance =
            Mathf.Clamp(
                distance,
                0f,
                track.TotalLength
            );


        // 현재보다 뒤쪽으로 이동시키는 상황 방지
        if (
            distance <
            CurrentMiddleDistance - 0.001f
        )
        {
            Debug.LogWarning(
                "[SpringTrainPathMover] " +
                "현재 위치보다 이전 Distance로는 이동하지 않습니다.",
                this
            );


            return false;
        }


        if (moveCoroutine != null)
        {
            StopCoroutine(
                moveCoroutine
            );


            moveCoroutine = null;
        }


        targetDistance =
            distance;


        IsRouteComplete =
            false;


        // 이미 목표점에 있음
        if (
            Mathf.Abs(
                CurrentMiddleDistance -
                targetDistance
            ) <= 0.001f
        )
        {
            formationController.ApplyFormation(
                track,
                CurrentMiddleDistance
            );


            IsMoving = false;

            IsRouteComplete = true;


            OnRouteCompleted?.Invoke();


            return true;
        }


        moveCoroutine =
            StartCoroutine(
                MoveRoutine()
            );


        return true;
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


            // =================================================
            // Middle Distance 하나만 관리
            //
            // FormationController에서
            //
            // Front  = Middle + spacing
            // Middle = Middle
            // Rear   = Middle - spacing
            //
            // 으로 각각 독립 Pose 계산
            // =================================================

            formationController.ApplyFormation(
                track,
                CurrentMiddleDistance
            );


            yield return null;
        }


        CurrentMiddleDistance =
            targetDistance;


        formationController.ApplyFormation(
            track,
            CurrentMiddleDistance
        );


        IsMoving =
            false;


        IsRouteComplete =
            true;


        moveCoroutine =
            null;


        Debug.Log(
            "[SpringTrainPathMover] " +
            $"목표 Distance 도착 : {targetDistance}",
            this
        );


        OnRouteCompleted?.Invoke();
    }


    // =========================================================
    // 특정 Point를 Car가 통과했는지
    //
    // 기존 API 유지
    // =========================================================

    public bool HasCarReachedPoint(
        int pointIndex,
        SpringTrainCarSlot carSlot
    )
    {
        if (
            track == null ||
            formationController == null
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


        return carDistance >=
            pointDistance;
    }


    // =========================================================
    // Middle 기준 Point 도달
    // =========================================================

    public bool HasMiddleReachedPoint(
        int pointIndex
    )
    {
        return HasCarReachedPoint(
            pointIndex,
            SpringTrainCarSlot.Middle
        );
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


        IsMoving =
            false;
    }


    // =========================================================
    // Inspector
    // =========================================================

    private void OnValidate()
    {
        if (moveSpeed < 0f)
        {
            moveSpeed =
                0f;
        }
    }
}