using System.Collections;
using UnityEngine;


public class SpringTrainPathMover : MonoBehaviour
{
    // =========================================================
    // Track
    // =========================================================

    [Header("전체 선로")]
    [SerializeField]
    private SpringTrainTrack track;


    // =========================================================
    // Formation
    // =========================================================

    [Header("기차 Formation")]
    [SerializeField]
    private SpringTrainFormationController formationController;


    // =========================================================
    // 이동
    // =========================================================

    [Header("기차 이동 속도")]
    [SerializeField]
    private float moveSpeed = 2f;


    // =========================================================
    // Runtime
    // =========================================================

    private SpringTrainTrack.PathSnapshot pathSnapshot;


    private float middlePathDistance;


    private bool stopRequested;


    // =========================================================
    // 상태
    // =========================================================

    public bool IsMoving
    {
        get;
        private set;
    }


    public bool DidCollide
    {
        get;
        private set;
    }


    public Vector3 CurrentTravelWorldDirection
    {
        get;
        private set;
    }


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


    // =========================================================
    // 현재 기차 위치를 Path 시작점으로 등록
    //
    // 여기서만 GetNearestDistance를 사용한다.
    // 이동 중에는 다시 계산하지 않는다.
    // =========================================================

    public bool SnapTrainToCurrentPath()
    {
        if (
            track == null ||
            formationController == null ||
            !formationController.HasAllCars
        )
        {
            Debug.LogWarning(
                "[SpringTrainPathMover] Track / Formation 연결 확인",
                this
            );


            return false;
        }


        pathSnapshot =
            track.BuildPathSnapshot();


        if (pathSnapshot == null)
        {
            return false;
        }


        Vector3 currentMiddlePosition =
            formationController.MiddlePosition;


        middlePathDistance =
            pathSnapshot.GetNearestDistance(
                currentMiddlePosition
            );


        formationController.ApplyFormation(
            pathSnapshot,
            middlePathDistance,
            true
        );


        Debug.Log(
            "[SpringTrainPathMover] 현재 기차 위치를 Path에 등록 완료",
            this
        );


        return true;
    }


    // =========================================================
    // 실제 물리 충돌이 발생할 때까지 이동
    // =========================================================

    public IEnumerator MoveUntilPhysicalCollision(
        SpringTrainDirection direction,
        float maximumTravelTime
    )
    {
        if (IsMoving)
        {
            yield break;
        }


        if (pathSnapshot == null)
        {
            if (!SnapTrainToCurrentPath())
            {
                yield break;
            }
        }


        IsMoving =
            true;


        DidCollide =
            false;


        stopRequested =
            false;


        float elapsed =
            0f;


        float sign =
            direction ==
            SpringTrainDirection.Clockwise
                ? 1f
                : -1f;


        while (
            !stopRequested &&
            elapsed < maximumTravelTime
        )
        {
            // Physics Timing에 맞춤
            yield return new WaitForFixedUpdate();


            if (stopRequested)
            {
                break;
            }


            float moveDistance =
                moveSpeed *
                Time.fixedDeltaTime;


            middlePathDistance +=
                sign *
                moveDistance;


            // -----------------------------------------
            // 지정 Point 사이 직선 위치 계산
            // -----------------------------------------

            formationController.ApplyFormation(
                pathSnapshot,
                middlePathDistance,
                false
            );


            CurrentTravelWorldDirection =
                pathSnapshot.EvaluateDirection(
                    middlePathDistance,
                    direction
                );


            // Transform으로 움직이는 Compound Collider를
            // Physics에 즉시 반영
            Physics.SyncTransforms();


            elapsed +=
                Time.fixedDeltaTime;
        }


        IsMoving =
            false;


        if (!DidCollide)
        {
            Debug.LogWarning(
                "[SpringTrainPathMover] 제한 시간 안에 물리 충돌이 발생하지 않았습니다.",
                this
            );
        }
    }


    // =========================================================
    // PhysicsController가 호출
    // =========================================================

    public void StopMovementForCollision()
    {
        DidCollide =
            true;


        stopRequested =
            true;
    }


    // =========================================================
    // 강제 정지
    // =========================================================

    public void StopMovementImmediately()
    {
        stopRequested =
            true;


        IsMoving =
            false;
    }


    private void OnValidate()
    {
        if (moveSpeed < 0f)
        {
            moveSpeed = 0f;
        }
    }
}