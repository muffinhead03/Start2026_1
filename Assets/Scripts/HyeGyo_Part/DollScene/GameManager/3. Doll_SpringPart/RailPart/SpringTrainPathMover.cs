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
    private float moveSpeed =
        2f;


    // =========================================================
    // Runtime
    // =========================================================

    private Coroutine moveCoroutine;


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
    // 준비 여부
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


    // =========================================================
    // 시작 위치
    // =========================================================

    public bool SnapTrainToStart()
    {
        if (
            track == null ||
            formationController == null ||
            !formationController.HasAllCars
        )
        {
            Debug.LogWarning(
                "[SpringTrainPathMover] " +
                "Track / Formation 연결을 확인하세요.",
                this
            );


            return false;
        }


        if (!track.RebuildCache())
        {
            return false;
        }


        CurrentMiddleDistance =
            0f;


        formationController.ApplyFormation(
            track,
            CurrentMiddleDistance
        );


        IsMoving =
            false;


        IsRouteComplete =
            false;


        return true;
    }


    // =========================================================
    // 처음부터 이동 시작
    // =========================================================

    public bool StartRouteFromStart()
    {
        if (!CanStartRoute)
        {
            Debug.LogWarning(
                "[SpringTrainPathMover] " +
                "Route를 시작할 수 없습니다.",
                this
            );


            return false;
        }


        if (moveCoroutine != null)
        {
            StopCoroutine(
                moveCoroutine
            );


            moveCoroutine =
                null;
        }


        CurrentMiddleDistance =
            0f;


        formationController.ApplyFormation(
            track,
            CurrentMiddleDistance
        );


        IsRouteComplete =
            false;


        moveCoroutine =
            StartCoroutine(
                MoveRoute()
            );


        return true;
    }


    // =========================================================
    // Route 이동
    // =========================================================

    private IEnumerator MoveRoute()
    {
        IsMoving =
            true;


        float targetDistance =
            track.TotalLength;


        Debug.Log(
            "[SpringTrainPathMover] Route 시작",
            this
        );


        while (
            CurrentMiddleDistance <
            targetDistance
        )
        {
            float moveDistance =
                moveSpeed *
                Time.deltaTime;


            CurrentMiddleDistance =
                Mathf.Min(
                    CurrentMiddleDistance +
                    moveDistance,
                    targetDistance
                );


            formationController.ApplyFormation(
                track,
                CurrentMiddleDistance
            );


            yield return null;
        }


        formationController.ApplyFormation(
            track,
            targetDistance
        );


        IsMoving =
            false;


        IsRouteComplete =
            true;


        moveCoroutine =
            null;


        Debug.Log(
            "[SpringTrainPathMover] Route 완료",
            this
        );


        OnRouteCompleted?.Invoke();
    }


    // =========================================================
    // 지정 Point를 Car가 통과했는가
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
            formationController
                .GetDistanceOffset(
                    carSlot
                );


        return carDistance >=
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


            moveCoroutine =
                null;
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