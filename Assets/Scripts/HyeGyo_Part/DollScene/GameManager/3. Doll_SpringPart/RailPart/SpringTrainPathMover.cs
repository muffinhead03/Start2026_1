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
    // 기차 3칸
    // =========================================================

    [Header("앞 칸")]
    [SerializeField]
    private SpringTrainCarFollower frontCar;


    [Header("중간 칸 - 기차 전체 기준점")]
    [SerializeField]
    private SpringTrainCarFollower middleCar;


    [Header("뒤 칸")]
    [SerializeField]
    private SpringTrainCarFollower rearCar;


    // =========================================================
    // 칸 간격
    // =========================================================

    [Header("기차 칸 중심 사이 거리")]
    [SerializeField]
    private float carSpacing =
        0.8f;


    [Header("앞 칸이 시계방향 쪽에 위치")]
    [Tooltip(
        "현재 배치에서 Front Car가 Middle Car보다 " +
        "시계방향 쪽에 있으면 체크합니다."
    )]
    [SerializeField]
    private bool frontIsClockwiseSide =
        true;


    // =========================================================
    // 이동
    // =========================================================

    [Header("전체 기차 이동 속도")]
    [SerializeField]
    private float moveSpeed =
        2f;


    [Header("선로 Center 도착 오차")]
    [SerializeField]
    private float arriveDistance =
        0.005f;


    [Header("회전 방향 확인 거리")]
    [Tooltip(
        "현재 위치보다 조금 앞의 경로를 확인해서 기차 방향을 계산합니다."
    )]
    [SerializeField]
    private float tangentLookAhead =
        0.05f;


    // =========================================================
    // 충돌
    // =========================================================

    [Header("기차 충돌 반경")]
    [Tooltip(
        "두 기차의 Middle Car 사이 거리로 충돌을 판단합니다."
    )]
    [SerializeField]
    private float collisionRadius =
        0.5f;


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


    public Transform TrainCenter
    {
        get
        {
            if (middleCar == null)
            {
                return null;
            }


            return
                middleCar.CarRoot;
        }
    }


    // =========================================================
    // Runtime Path
    // =========================================================

    private SpringTrainTrack.PathSnapshot
        pathSnapshot;


    // 중간칸의 현재 Path 거리
    //
    // 이 값 하나만 움직인다.
    // 앞/뒤칸은 이 값 ± carSpacing
    private float centerPathDistance;


    // =========================================================
    // Unity
    // =========================================================

    private void Start()
    {
        RefreshCurrentRailIndex();
    }


    // =========================================================
    // 현재 위치 기준 Path 다시 생성
    //
    // Rail이 퍼즐로 회전한 뒤 호출되므로
    // 회전된 HelpPoint 위치를 다시 읽음
    // =========================================================

    public int RefreshCurrentRailIndex()
    {
        if (
            track == null ||
            middleCar == null
        )
        {
            CurrentRailIndex =
                -1;


            return -1;
        }


        pathSnapshot =
            track.BuildPathSnapshot();


        if (pathSnapshot == null)
        {
            CurrentRailIndex =
                -1;


            return -1;
        }


        Vector3 middlePosition =
            middleCar.Position;


        // 현재 Middle Car와 가장 가까운
        // Path 거리
        centerPathDistance =
            pathSnapshot.GetNearestDistance(
                middlePosition
            );


        // 현재 가장 가까운 Rail
        CurrentRailIndex =
            track.GetNearestSectionIndex(
                middlePosition
            );


        return
            CurrentRailIndex;
    }


    // =========================================================
    // 여러 Rail 이동
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


        IsMoving =
            true;


        DidCollide =
            false;


        for (
            int i = 0;
            i < stepCount;
            i++
        )
        {
            yield return
                MoveToNextSection(
                    direction,
                    null,
                    false
                );


            if (DidCollide)
            {
                break;
            }
        }


        IsMoving =
            false;
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


        if (
            maximumRailSteps <= 0
        )
        {
            yield break;
        }


        if (!PrepareMovement())
        {
            yield break;
        }


        IsMoving =
            true;


        DidCollide =
            false;


        for (
            int i = 0;
            i < maximumRailSteps;
            i++
        )
        {
            yield return
                MoveToNextSection(
                    direction,
                    otherTrain,
                    true
                );


            if (DidCollide)
            {
                break;
            }
        }


        IsMoving =
            false;
    }


    // =========================================================
    // 이동 준비
    // =========================================================

    private bool PrepareMovement()
    {
        if (
            track == null ||
            frontCar == null ||
            middleCar == null ||
            rearCar == null
        )
        {
            Debug.LogWarning(
                "[SpringTrainPathMover] Track 또는 기차 3칸 연결이 안 되어 있습니다."
            );


            return false;
        }


        int railIndex =
            RefreshCurrentRailIndex();


        if (railIndex < 0)
        {
            Debug.LogWarning(
                "[SpringTrainPathMover] 현재 선로를 찾지 못했습니다."
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
                otherTrain != null &&
                IsCollidingWith(
                    otherTrain
                )
            )
            {
                HandleCollisionStop();


                yield break;
            }


            // =================================================
            // Middle 기준 거리 하나만 이동
            // =================================================

            centerPathDistance =
                Mathf.MoveTowards(
                    centerPathDistance,
                    targetDistance,
                    moveSpeed *
                    Time.deltaTime
                );


            // =================================================
            // 3칸 전부 같은 경로에 배치
            // =================================================

            ApplyCarPositions(
                false
            );


            // =================================================
            // 이동 후 충돌 검사
            // =================================================

            if (
                stopOnCollision &&
                otherTrain != null &&
                IsCollidingWith(
                    otherTrain
                )
            )
            {
                HandleCollisionStop();


                yield break;
            }


            yield return null;
        }


        // =====================================================
        // 정확히 Section Center에 도착
        // =====================================================

        centerPathDistance =
            targetDistance;


        ApplyCarPositions(
            false
        );


        CurrentRailIndex =
            nextIndex;


        // =====================================================
        // 도착 직후 충돌
        // =====================================================

        if (
            stopOnCollision &&
            otherTrain != null &&
            IsCollidingWith(
                otherTrain
            )
        )
        {
            HandleCollisionStop();
        }
    }


    // =========================================================
    // 다음 Rail Center의 Unwrapped 거리 구하기
    //
    // centerPathDistance는 계속 증가/감소 가능
    // Snapshot 내부에서 실제 위치 계산 시 원형 Wrap
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
    // 기차 3칸 위치 적용
    // =========================================================

    private void ApplyCarPositions(
        bool snapRotation
    )
    {
        if (pathSnapshot == null)
        {
            return;
        }


        float frontSideSign =
            frontIsClockwiseSide
                ? 1f
                : -1f;


        // =====================================================
        // Middle은 기준 거리 그대로
        // =====================================================

        ApplySingleCar(
            middleCar,
            centerPathDistance,
            snapRotation
        );


        // =====================================================
        // Front
        // =====================================================

        ApplySingleCar(
            frontCar,
            centerPathDistance +
            carSpacing *
            frontSideSign,
            snapRotation
        );


        // =====================================================
        // Rear
        // =====================================================

        ApplySingleCar(
            rearCar,
            centerPathDistance -
            carSpacing *
            frontSideSign,
            snapRotation
        );
    }


    // =========================================================
    // 한 칸 위치 + 방향 계산
    // =========================================================

    private void ApplySingleCar(
        SpringTrainCarFollower car,
        float pathDistance,
        bool snapRotation
    )
    {
        if (car == null)
        {
            return;
        }


        Vector3 position =
            pathSnapshot.EvaluatePosition(
                pathDistance
            );


        // 기차의 물리적인 앞 방향
        //
        // 반동으로 뒤로 움직여도
        // 기차 자체가 갑자기 180도 뒤집히지 않게 함
        SpringTrainDirection visualDirection =
            frontIsClockwiseSide
                ? SpringTrainDirection.Clockwise
                : SpringTrainDirection.CounterClockwise;


        Vector3 direction =
            pathSnapshot.EvaluateDirection(
                pathDistance,
                visualDirection,
                tangentLookAhead
            );


        car.ApplyPathPose(
            position,
            direction,
            snapRotation
        );
    }


    // =========================================================
    // 충돌
    // =========================================================

    public bool IsCollidingWith(
        SpringTrainPathMover otherTrain
    )
    {
        if (
            otherTrain == null ||
            TrainCenter == null ||
            otherTrain.TrainCenter == null
        )
        {
            return false;
        }


        float collisionDistance =
            collisionRadius +
            otherTrain.collisionRadius;


        float currentDistance =
            Vector3.Distance(
                TrainCenter.position,
                otherTrain
                    .TrainCenter
                    .position
            );


        return
            currentDistance <=
            collisionDistance;
    }


    // =========================================================
    // 충돌 시 현재 상태 갱신
    // =========================================================

    private void HandleCollisionStop()
    {
        DidCollide =
            true;


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
    // Inspector
    // =========================================================

    private void OnValidate()
    {
        if (moveSpeed < 0f)
        {
            moveSpeed =
                0f;
        }


        if (carSpacing < 0f)
        {
            carSpacing =
                0f;
        }


        if (arriveDistance < 0f)
        {
            arriveDistance =
                0f;
        }


        if (tangentLookAhead < 0.001f)
        {
            tangentLookAhead =
                0.001f;
        }


        if (collisionRadius < 0f)
        {
            collisionRadius =
                0f;
        }
    }
}