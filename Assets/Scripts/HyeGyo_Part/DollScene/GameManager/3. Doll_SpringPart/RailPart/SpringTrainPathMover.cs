using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum SpringTrainMovementPlane
{
    XZ,
    XY
}


public class SpringTrainPathMover : MonoBehaviour
{
    // =========================================================
    // 기차
    // =========================================================

    [Header("기차 Root")]
    [SerializeField]
    private Transform trainRoot;


    [Header("기차 중심점")]
    [Tooltip(
        "기차가 여러 선로에 걸쳐 있을 때 " +
        "이 Transform 위치를 기준으로 현재 선로를 판단합니다."
    )]
    [SerializeField]
    private Transform trainCenter;


    // =========================================================
    // 선로
    // =========================================================

    [Header("전체 선로")]
    [SerializeField]
    private SpringTrainTrack track;


    // =========================================================
    // 이동
    // =========================================================

    [Header("이동 속도")]
    [SerializeField]
    private float moveSpeed = 2f;


    [Header("도착 오차")]
    [SerializeField]
    private float arriveDistance = 0.01f;


    // =========================================================
    // 충돌
    // =========================================================

    [Header("기차 충돌 반경")]
    [Tooltip(
        "두 기차 중심 사이의 거리가 " +
        "Collision Radius의 합보다 작으면 충돌입니다."
    )]
    [SerializeField]
    private float collisionRadius = 0.5f;


    // =========================================================
    // 기차 방향 회전
    // =========================================================

    [Header("기차가 경로 방향을 바라봄")]
    [SerializeField]
    private bool rotateAlongPath = true;


    [SerializeField]
    private SpringTrainMovementPlane movementPlane =
        SpringTrainMovementPlane.XZ;


    [Header("모델 방향 보정")]
    [SerializeField]
    private Vector3 rotationOffset;


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


    public Transform TrainCenter =>
        trainCenter;


    // =========================================================
    // Unity
    // =========================================================

    private void Awake()
    {
        if (trainRoot == null)
        {
            trainRoot =
                transform;
        }


        if (trainCenter == null)
        {
            trainCenter =
                trainRoot;
        }
    }


    private void Start()
    {
        RefreshCurrentRailIndex();
    }


    // =========================================================
    // 현재 기차가 위치한 선로 갱신
    // =========================================================

    public int RefreshCurrentRailIndex()
    {
        if (track == null ||
            trainCenter == null)
        {
            CurrentRailIndex =
                -1;

            return -1;
        }


        CurrentRailIndex =
            track.GetNearestSectionIndex(
                trainCenter.position
            );


        return CurrentRailIndex;
    }


    // =========================================================
    // 여러 칸 이동
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


        IsMoving =
            true;


        DidCollide =
            false;


        RefreshCurrentRailIndex();


        for (int i = 0; i < stepCount; i++)
        {
            yield return MoveOneStepInternal(
                direction,
                null,
                false
            );
        }


        RefreshCurrentRailIndex();


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


        IsMoving =
            true;


        DidCollide =
            false;


        RefreshCurrentRailIndex();


        for (
            int i = 0;
            i < maximumRailSteps;
            i++
        )
        {
            yield return MoveOneStepInternal(
                direction,
                otherTrain,
                true
            );


            if (DidCollide)
            {
                break;
            }
        }


        RefreshCurrentRailIndex();


        IsMoving =
            false;
    }


    // =========================================================
    // 한 칸 이동
    // =========================================================

    private IEnumerator MoveOneStepInternal(
        SpringTrainDirection direction,
        SpringTrainPathMover otherTrain,
        bool stopOnCollision
    )
    {
        if (track == null)
        {
            yield break;
        }


        if (CurrentRailIndex < 0)
        {
            RefreshCurrentRailIndex();
        }


        List<Vector3> path =
            track.BuildStepPath(
                CurrentRailIndex,
                direction
            );


        for (int i = 0; i < path.Count; i++)
        {
            Vector3 targetPosition =
                path[i];


            while (
                Vector3.Distance(
                    trainCenter.position,
                    targetPosition
                ) > arriveDistance
            )
            {
                // ---------------------------------------------
                // 다른 기차와 충돌
                // ---------------------------------------------

                if (
                    stopOnCollision &&
                    otherTrain != null &&
                    IsCollidingWith(
                        otherTrain
                    )
                )
                {
                    DidCollide =
                        true;


                    RefreshCurrentRailIndex();


                    yield break;
                }


                MoveCenterTowards(
                    targetPosition
                );


                yield return null;
            }


            SnapCenterTo(
                targetPosition
            );


            if (
                stopOnCollision &&
                otherTrain != null &&
                IsCollidingWith(
                    otherTrain
                )
            )
            {
                DidCollide =
                    true;


                RefreshCurrentRailIndex();


                yield break;
            }
        }


        CurrentRailIndex =
            track.GetNextIndex(
                CurrentRailIndex,
                direction
            );
    }


    // =========================================================
    // 충돌 검사
    // =========================================================

    public bool IsCollidingWith(
        SpringTrainPathMover otherTrain
    )
    {
        if (otherTrain == null ||
            otherTrain.trainCenter == null ||
            trainCenter == null)
        {
            return false;
        }


        float collisionDistance =
            collisionRadius +
            otherTrain.collisionRadius;


        float currentDistance =
            Vector3.Distance(
                trainCenter.position,
                otherTrain.trainCenter.position
            );


        return
            currentDistance <=
            collisionDistance;
    }


    // =========================================================
    // Center 기준 이동
    // =========================================================

    private void MoveCenterTowards(
        Vector3 targetPosition
    )
    {
        Vector3 currentCenter =
            trainCenter.position;


        Vector3 nextCenter =
            Vector3.MoveTowards(
                currentCenter,
                targetPosition,
                moveSpeed * Time.deltaTime
            );


        Vector3 moveDirection =
            nextCenter -
            currentCenter;


        trainRoot.position +=
            moveDirection;


        if (
            rotateAlongPath &&
            moveDirection.sqrMagnitude >
            0.000001f
        )
        {
            RotateTrain(
                moveDirection
            );


            // 회전 후 Center가 살짝 이동하는
            // Pivot 구조에도 대응
            trainRoot.position +=
                nextCenter -
                trainCenter.position;
        }
    }


    // =========================================================
    // Center 정확한 위치 보정
    // =========================================================

    private void SnapCenterTo(
        Vector3 targetPosition
    )
    {
        trainRoot.position +=
            targetPosition -
            trainCenter.position;
    }


    // =========================================================
    // 진행 방향으로 기차 회전
    // =========================================================

    private void RotateTrain(
        Vector3 direction
    )
    {
        if (
            movementPlane ==
            SpringTrainMovementPlane.XZ
        )
        {
            direction.y =
                0f;


            if (
                direction.sqrMagnitude <
                0.000001f
            )
            {
                return;
            }


            Quaternion targetRotation =
                Quaternion.LookRotation(
                    direction.normalized,
                    Vector3.up
                );


            trainRoot.rotation =
                targetRotation *
                Quaternion.Euler(
                    rotationOffset
                );
        }
        else
        {
            direction.z =
                0f;


            float angle =
                Mathf.Atan2(
                    direction.y,
                    direction.x
                ) *
                Mathf.Rad2Deg;


            trainRoot.rotation =
                Quaternion.Euler(
                    0f,
                    0f,
                    angle
                ) *
                Quaternion.Euler(
                    rotationOffset
                );
        }
    }
}