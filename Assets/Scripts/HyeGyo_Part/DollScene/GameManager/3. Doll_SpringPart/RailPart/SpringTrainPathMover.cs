using System.Collections;
using UnityEngine;


public class SpringTrainPathMover : MonoBehaviour
{
    // =========================================================
    // 선로
    // =========================================================

    [Header("전체 선로")]
    [SerializeField]
    private SpringTrainTrack track;


    // =========================================================
    // 기차 3칸 배치 Controller
    // =========================================================

    [Header("기차 3칸 배치 Controller")]
    [SerializeField]
    private SpringTrainFormationController formationController;


    // =========================================================
    // 충돌 Detector
    // =========================================================

    [Header("기차 충돌 Detector")]
    [SerializeField]
    private SpringTrainCollisionDetector collisionDetector;


    // =========================================================
    // 이동
    // =========================================================

    [Header("전체 기차 이동 속도")]
    [SerializeField]
    private float moveSpeed = 2f;


    [Header("선로 Center 도착 오차")]
    [SerializeField]
    private float arriveDistance = 0.005f;


    // =========================================================
    // 상태
    // =========================================================

    public int CurrentRailIndex
    {
        get;
        private set;
    } = -1;


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


    // =========================================================
    // 외부 접근
    // =========================================================

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


    public SpringTrainCollisionDetector CollisionDetector =>
        collisionDetector;


    // =========================================================
    // Runtime Path
    // =========================================================

    private SpringTrainTrack.PathSnapshot pathSnapshot;


    // =========================================================
    // 중간 칸의 Path 진행 거리
    //
    // 실제 이동에서는 이 값 하나만 움직인다.
    //
    // Front / Rear 위치는
    // SpringTrainFormationController가 계산한다.
    // =========================================================

    private float centerPathDistance;


    // =========================================================
    // Unity
    // =========================================================

    private void Start()
    {
        RefreshCurrentRailIndex();
    }


    // =========================================================
    // 현재 기차 위치 기준으로
    // Path Snapshot과 현재 Rail을 다시 계산
    // =========================================================

    public int RefreshCurrentRailIndex()
    {
        if (
            track == null ||
            formationController == null ||
            formationController.TrainCenter == null
        )
        {
            CurrentRailIndex = -1;

            return -1;
        }


        // 현재 회전된 Rail / HelpPoint의
        // World Position 기준으로 Path 새로 생성
        pathSnapshot =
            track.BuildPathSnapshot();


        if (pathSnapshot == null)
        {
            CurrentRailIndex = -1;

            return -1;
        }


        Vector3 middlePosition =
            formationController.MiddlePosition;


        // 현재 Middle Car에 가장 가까운
        // Path상의 거리
        centerPathDistance =
            pathSnapshot.GetNearestDistance(
                middlePosition
            );


        // 현재 가장 가까운 Rail
        CurrentRailIndex =
            track.GetNearestSectionIndex(
                middlePosition
            );


        return CurrentRailIndex;
    }


    // =========================================================
    // 지정된 Rail 칸 수만큼 이동
    //
    // 충돌 후 반동 등의 연출에 사용
    // =========================================================

    public IEnumerator MoveRailSteps(
        int stepCount,
        SpringTrainDirection direction
    )
    {
        if (IsMoving)
        {
            yield break;
        }


        if (stepCount <= 0)
        {
            yield break;
        }


        if (!PrepareMovement())
        {
            yield break;
        }


        IsMoving = true;

        DidCollide = false;


        for (
            int i = 0;
            i < stepCount;
            i++
        )
        {
            yield return MoveToNextSection(
                direction,
                null,
                false
            );


            if (DidCollide)
            {
                break;
            }
        }


        IsMoving = false;
    }


    // =========================================================
    // 다른 기차와 충돌할 때까지 이동
    // =========================================================

    public IEnumerator MoveUntilCollision(
        SpringTrainDirection direction,
        SpringTrainPathMover otherTrain,
        int maximumRailSteps
    )
    {
        if (IsMoving)
        {
            yield break;
        }


        if (otherTrain == null)
        {
            yield break;
        }


        if (maximumRailSteps <= 0)
        {
            yield break;
        }


        if (collisionDetector == null)
        {
            Debug.LogWarning(
                "[SpringTrainPathMover] CollisionDetector가 연결되지 않았습니다.",
                this
            );

            yield break;
        }


        if (otherTrain.CollisionDetector == null)
        {
            Debug.LogWarning(
                "[SpringTrainPathMover] 상대 기차의 CollisionDetector가 연결되지 않았습니다.",
                otherTrain
            );

            yield break;
        }


        if (!PrepareMovement())
        {
            yield break;
        }


        IsMoving = true;

        DidCollide = false;


        for (
            int i = 0;
            i < maximumRailSteps;
            i++
        )
        {
            yield return MoveToNextSection(
                direction,
                otherTrain,
                true
            );


            if (DidCollide)
            {
                break;
            }
        }


        IsMoving = false;
    }


    // =========================================================
    // 이동 준비
    // =========================================================

    private bool PrepareMovement()
    {
        if (track == null)
        {
            Debug.LogWarning(
                "[SpringTrainPathMover] Track이 연결되지 않았습니다.",
                this
            );

            return false;
        }


        if (formationController == null)
        {
            Debug.LogWarning(
                "[SpringTrainPathMover] FormationController가 연결되지 않았습니다.",
                this
            );

            return false;
        }


        if (!formationController.HasAllCars)
        {
            Debug.LogWarning(
                "[SpringTrainPathMover] Front / Middle / Rear 연결을 확인하세요.",
                this
            );

            return false;
        }


        int railIndex =
            RefreshCurrentRailIndex();


        if (railIndex < 0)
        {
            Debug.LogWarning(
                "[SpringTrainPathMover] 현재 선로를 찾지 못했습니다.",
                this
            );

            return false;
        }


        return true;
    }


    // =========================================================
    // 다음 Section Center까지 이동
    // =========================================================

    private IEnumerator MoveToNextSection(
        SpringTrainDirection direction,
        SpringTrainPathMover otherTrain,
        bool stopOnCollision
    )
    {
        if (
            pathSnapshot == null ||
            CurrentRailIndex < 0
        )
        {
            yield break;
        }


        int nextIndex =
            track.GetNextIndex(
                CurrentRailIndex,
                direction
            );


        if (nextIndex < 0)
        {
            yield break;
        }


        float targetDistance =
            GetTargetPathDistance(
                nextIndex,
                direction
            );


        while (
            Mathf.Abs(
                targetDistance -
                centerPathDistance
            ) >
            arriveDistance
        )
        {
            // =================================================
            // 이동 전 충돌 검사
            // =================================================

            if (
                stopOnCollision &&
                IsCollidingWith(otherTrain)
            )
            {
                HandleCollisionStop();

                yield break;
            }


            // =================================================
            // Middle 기준 Path 거리 이동
            // =================================================

            centerPathDistance =
                Mathf.MoveTowards(
                    centerPathDistance,
                    targetDistance,
                    moveSpeed *
                    Time.deltaTime
                );


            // =================================================
            // Front / Middle / Rear 실제 배치
            // =================================================

            ApplyFormation(
                false
            );


            // =================================================
            // 이동 후 충돌 검사
            // =================================================

            if (
                stopOnCollision &&
                IsCollidingWith(otherTrain)
            )
            {
                HandleCollisionStop();

                yield break;
            }


            yield return null;
        }


        // =====================================================
        // 정확히 다음 Rail Center에 도착
        // =====================================================

        centerPathDistance =
            targetDistance;


        ApplyFormation(
            false
        );


        CurrentRailIndex =
            nextIndex;


        // =====================================================
        // 도착 직후 충돌 검사
        // =====================================================

        if (
            stopOnCollision &&
            IsCollidingWith(otherTrain)
        )
        {
            HandleCollisionStop();
        }
    }


    // =========================================================
    // 다음 Rail Center의
    // Unwrapped Path 거리 계산
    //
    // centerPathDistance는 계속
    // 증가 또는 감소할 수 있다.
    //
    // 실제 위치 계산 시
    // PathSnapshot 내부에서 Wrap 처리한다.
    // =========================================================

    private float GetTargetPathDistance(
        int targetSectionIndex,
        SpringTrainDirection direction
    )
    {
        float totalLength =
            pathSnapshot.TotalLength;


        float currentWrapped =
            pathSnapshot.WrapDistance(
                centerPathDistance
            );


        float targetWrapped =
            pathSnapshot.GetSectionDistance(
                targetSectionIndex
            );


        if (
            direction ==
            SpringTrainDirection.Clockwise
        )
        {
            float delta =
                Mathf.Repeat(
                    targetWrapped -
                    currentWrapped,
                    totalLength
                );


            return
                centerPathDistance +
                delta;
        }


        float reverseDelta =
            Mathf.Repeat(
                currentWrapped -
                targetWrapped,
                totalLength
            );


        return
            centerPathDistance -
            reverseDelta;
    }


    // =========================================================
    // 3칸 배치 적용
    // =========================================================

    private void ApplyFormation(
        bool snapRotation
    )
    {
        if (
            formationController == null ||
            pathSnapshot == null
        )
        {
            return;
        }


        formationController.ApplyFormation(
            pathSnapshot,
            centerPathDistance,
            snapRotation
        );
    }


    // =========================================================
    // 충돌 검사
    // =========================================================

    private bool IsCollidingWith(
        SpringTrainPathMover otherTrain
    )
    {
        if (
            otherTrain == null ||
            collisionDetector == null ||
            otherTrain.CollisionDetector == null
        )
        {
            return false;
        }


        return collisionDetector.IsCollidingWith(
            otherTrain.CollisionDetector
        );
    }


    // =========================================================
    // 충돌 시 상태 갱신
    // =========================================================

    private void HandleCollisionStop()
    {
        DidCollide = true;


        if (
            track != null &&
            TrainCenter != null
        )
        {
            CurrentRailIndex =
                track.GetNearestSectionIndex(
                    TrainCenter.position
                );
        }
    }


    // =========================================================
    // Inspector 검사
    // =========================================================

    private void OnValidate()
    {
        if (moveSpeed < 0f)
        {
            moveSpeed = 0f;
        }


        if (arriveDistance < 0f)
        {
            arriveDistance = 0f;
        }
    }

    // =========================================================
// 완성된 선로 Path에 기차를 정확하게 맞춤
//
// 퍼즐 진행 중에는 호출하지 않는다.
// 전체 선로 정답 이후 SequenceManager가 호출한다.
// =========================================================

public bool SnapTrainToCurrentPath()
{
    if (
        track == null ||
        formationController == null
    )
    {
        return false;
    }


    pathSnapshot =
        track.BuildPathSnapshot();


    if (pathSnapshot == null)
    {
        return false;
    }


    Vector3 middlePosition =
        formationController.MiddlePosition;


    centerPathDistance =
        pathSnapshot.GetNearestDistance(
            middlePosition
        );


    CurrentRailIndex =
        track.GetNearestSectionIndex(
            middlePosition
        );


    formationController.ApplyFormation(
        pathSnapshot,
        centerPathDistance,
        true
    );


    Debug.Log(
        "[SpringTrainPathMover] 기차를 완성된 Path에 맞춤",
        this
    );


    return true;
}

// =========================================================
// 일정 시간 동안 Path를 따라 이동하면서
// 다른 기차와 충돌 검사
//
// duration은 최대 이동 시간.
// 그 전에 충돌하면 즉시 정지.
// =========================================================

public IEnumerator MoveUntilCollisionForDuration(
    SpringTrainDirection direction,
    SpringTrainPathMover otherTrain,
    float duration
)
{
    if (IsMoving)
    {
        yield break;
    }


    if (
        otherTrain == null ||
        duration <= 0f
    )
    {
        yield break;
    }


    if (!PrepareMovement())
    {
        yield break;
    }


    if (
        collisionDetector == null ||
        otherTrain.CollisionDetector == null
    )
    {
        Debug.LogWarning(
            "[SpringTrainPathMover] CollisionDetector 연결을 확인하세요.",
            this
        );

        yield break;
    }


    IsMoving =
        true;


    DidCollide =
        false;


    float elapsed =
        0f;


    float moveSign =
        direction ==
        SpringTrainDirection.Clockwise
            ? 1f
            : -1f;


    while (elapsed < duration)
    {
        // 이동 전 충돌 검사
        if (IsCollidingWith(otherTrain))
        {
            HandleCollisionStop();

            break;
        }


        float deltaTime =
            Time.deltaTime;


        centerPathDistance +=
            moveSign *
            moveSpeed *
            deltaTime;


        ApplyFormation(
            false
        );


        // 이동 후 충돌 검사
        if (IsCollidingWith(otherTrain))
        {
            HandleCollisionStop();

            break;
        }


        elapsed +=
            deltaTime;


        yield return null;
    }


    IsMoving =
        false;


    if (!DidCollide)
    {
        Debug.LogWarning(
            "[SpringTrainPathMover] 설정된 시간 안에 충돌하지 않았습니다.",
            this
        );
    }
}
}