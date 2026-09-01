using UnityEngine;


[RequireComponent(typeof(Rigidbody))]
public class SpringTrainPhysicsController : MonoBehaviour
{
    // =========================================================
    // 기차 상태
    // =========================================================

    private enum TrainPhysicsState
    {
        Idle,
        PathMoving,
        StationaryCollisionReceiver,
        Reacting,
        Finished
    }


    // =========================================================
    // Path
    // =========================================================

    [Header("이 기차의 Path Mover")]
    [SerializeField]
    private SpringTrainPathMover pathMover;


    // =========================================================
    // Tags
    // =========================================================

    [Header("장난감 기차 Tag")]
    [SerializeField]
    private string trainTag =
        "ToyTrain";


    [Header("플레이어 Tag")]
    [SerializeField]
    private string playerTag =
        "Player";


    // =========================================================
    // 충돌 후 이동
    // =========================================================

    [Header("충돌 후 이동 거리")]
    [SerializeField]
    private float reactionDistance =
        10f;


    [Header("충돌 후 이동 속도")]
    [SerializeField]
    private float reactionSpeed =
        4f;


    [Header("충돌 순간 추가 힘")]
    [SerializeField]
    private float impactImpulse =
        2f;


    // =========================================================
    // Runtime
    // =========================================================

    private Rigidbody body;


    private TrainPhysicsState state =
        TrainPhysicsState.Idle;


    private Vector3 reactionDirection;


    private Vector3 reactionStartPosition;


    private bool collisionHandled =
        false;


    // =========================================================
    // 외부 상태
    // =========================================================

    public bool IsReacting =>
        state ==
        TrainPhysicsState.Reacting;


    public bool IsReactionFinished =>
        state ==
        TrainPhysicsState.Finished;


    // =========================================================
    // Unity
    // =========================================================

    private void Awake()
    {
        body =
            GetComponent<Rigidbody>();


        PrepareIdleLocked();
    }


    // =========================================================
    // 퍼즐 진행 중
    //
    // 기차는 완전히 고정.
    //
    // Player가 밀어도 움직이지 않지만
    // Collider는 살아 있기 때문에
    // Player는 기차를 통과하지 못함.
    // =========================================================

    public void PrepareIdleLocked()
    {
        if (body == null)
        {
            body =
                GetComponent<Rigidbody>();
        }


        collisionHandled =
            false;


        state =
            TrainPhysicsState.Idle;


        body.useGravity =
            false;


        body.isKinematic =
            true;


        body.linearVelocity =
            Vector3.zero;


        body.angularVelocity =
            Vector3.zero;


        body.constraints =
            RigidbodyConstraints.None;
    }


    // =========================================================
    // 색깔 기차 Path 이동 준비
    //
    // Kinematic 상태 유지.
    //
    // 따라서:
    // - Player가 밀 수 없음
    // - 선로가 밀 수 없음
    // - PathMover가 Transform 이동 가능
    // =========================================================

    public void PrepareForPathMovement()
    {
        if (body == null)
        {
            body =
                GetComponent<Rigidbody>();
        }


        collisionHandled =
            false;


        state =
            TrainPhysicsState.PathMoving;


        body.useGravity =
            false;


        body.isKinematic =
            true;


        body.linearVelocity =
            Vector3.zero;


        body.angularVelocity =
            Vector3.zero;


        body.constraints =
            RigidbodyConstraints.None;
    }


    // =========================================================
    // 검은 기차 충돌 대기
    //
    // 색깔 기차는 Kinematic.
    //
    // Unity의 실제 Collision Event를 받으려면
    // 반대쪽 검은 기차 Rigidbody는 Dynamic이어야 함.
    //
    // 하지만 FreezeAll을 걸어서
    // 충돌 전에는 절대 움직이지 않게 함.
    // =========================================================

    public void PrepareForStationaryCollision()
    {
        if (body == null)
        {
            body =
                GetComponent<Rigidbody>();
        }


        collisionHandled =
            false;


        state =
            TrainPhysicsState.StationaryCollisionReceiver;


        body.useGravity =
            false;


        body.isKinematic =
            false;


        body.linearVelocity =
            Vector3.zero;


        body.angularVelocity =
            Vector3.zero;


        body.constraints =
            RigidbodyConstraints.FreezeAll;
    }


    // =========================================================
    // 실제 Unity Collision
    // =========================================================

    private void OnCollisionEnter(
        Collision collision
    )
    {
        if (collision == null)
        {
            return;
        }


        // =====================================================
        // 1. Player 충돌
        //
        // 중요:
        // Collider 충돌 자체는 유지한다.
        //
        // 즉 Player는 기차에 막히지만
        // 기차 충돌 연출에는 절대 사용하지 않는다.
        // =====================================================

        if (
            HasTagInParents(
                collision.collider.transform,
                playerTag
            )
        )
        {
            return;
        }


        // 이미 기차 충돌 처리가 끝났으면 무시
        if (collisionHandled)
        {
            return;
        }


        // =====================================================
        // 2. 상대가 실제 기차인지 확인
        // =====================================================

        SpringTrainPhysicsController other =
            collision.collider
                .GetComponentInParent<
                    SpringTrainPhysicsController
                >();


        if (
            other == null ||
            other == this
        )
        {
            return;
        }


        // =====================================================
        // 3. ToyTrain Tag끼리만 충돌 연출
        // =====================================================

        if (
            !CompareTag(trainTag) ||
            !other.CompareTag(trainTag)
        )
        {
            return;
        }


        if (other.collisionHandled)
        {
            return;
        }


        // =====================================================
        // 4. 어느 기차가 움직이고 있었는지 결정
        // =====================================================

        SpringTrainPhysicsController movingTrain;

        SpringTrainPhysicsController stationaryTrain;


        if (
            state ==
            TrainPhysicsState.PathMoving
        )
        {
            movingTrain =
                this;


            stationaryTrain =
                other;
        }
        else if (
            other.state ==
            TrainPhysicsState.PathMoving
        )
        {
            movingTrain =
                other;


            stationaryTrain =
                this;
        }
        else
        {
            // 둘 다 PathMoving 상태가 아니라면
            // 우리가 원하는 기차 충돌이 아님
            return;
        }


        // =====================================================
        // 5. 중복 충돌 차단
        // =====================================================

        movingTrain.collisionHandled =
            true;


        stationaryTrain.collisionHandled =
            true;


        Debug.Log(
            "[SpringTrainPhysics] " +
            "ToyTrain ↔ ToyTrain 실제 충돌 발생",
            this
        );


        // =====================================================
        // 6. Path 이동 즉시 종료
        // =====================================================

        if (movingTrain.pathMover != null)
        {
            movingTrain.pathMover
                .StopMovementForCollision();
        }


        if (stationaryTrain.pathMover != null)
        {
            stationaryTrain.pathMover
                .StopMovementForCollision();
        }


        // =====================================================
        // 7. 충돌 진행 방향 계산
        // =====================================================

        Vector3 impactDirection =
            Vector3.zero;


        // 가장 먼저 PathMover가 알고 있는
        // 실제 기차 이동 방향을 사용
        if (movingTrain.pathMover != null)
        {
            impactDirection =
                movingTrain
                    .pathMover
                    .CurrentTravelWorldDirection;
        }


        // XZ 평면만 사용
        impactDirection.y =
            0f;


        // =====================================================
        // Path 방향을 얻지 못했다면
        // 실제 Collision Contact Normal 사용
        // =====================================================

        if (
            impactDirection.sqrMagnitude <
            0.000001f
        )
        {
            if (collision.contactCount > 0)
            {
                ContactPoint contact =
                    collision.GetContact(0);


                impactDirection =
                    -contact.normal;


                impactDirection.y =
                    0f;
            }
        }


        // =====================================================
        // 그래도 방향이 없다면
        // 기차 Forward 사용
        // =====================================================

        if (
            impactDirection.sqrMagnitude <
            0.000001f
        )
        {
            impactDirection =
                movingTrain.transform.forward;


            impactDirection.y =
                0f;
        }


        impactDirection.Normalize();


        // =====================================================
        // 8. 첫 충돌 이후
        // 두 기차끼리의 Collider는 무시
        //
        // 그래야 Reaction 중 다시 충돌해서
        // 충돌 연출이 꼬이지 않는다.
        // =====================================================

        IgnoreCollisionBetweenTrains(
            movingTrain,
            stationaryTrain
        );


        // =====================================================
        // 9. 충돌 반동
        //
        // 움직이던 색깔 기차:
        // 충돌 진행 방향의 반대
        //
        // 검은 기차:
        // 충돌 진행 방향
        // =====================================================

        movingTrain.BeginReaction(
            -impactDirection
        );


        stationaryTrain.BeginReaction(
            impactDirection
        );
    }


    // =========================================================
    // 충돌 후 이동 시작
    // =========================================================

    private void BeginReaction(
        Vector3 direction
    )
    {
        if (body == null)
        {
            body =
                GetComponent<Rigidbody>();
        }


        // =====================================================
        // XZ 평면으로 고정
        // =====================================================

        direction.y =
            0f;


        if (
            direction.sqrMagnitude <
            0.000001f
        )
        {
            return;
        }


        reactionDirection =
            direction.normalized;


        reactionStartPosition =
            body.position;


        state =
            TrainPhysicsState.Reacting;


        // =====================================================
        // 이제부터 실제 Rigidbody 이동
        // =====================================================

        body.isKinematic =
            false;


        body.useGravity =
            false;


        // Y 위치는 고정
        // X/Z 회전도 고정
        //
        // 즉 기차가 넘어지지 않음.
        body.constraints =
            RigidbodyConstraints.FreezePositionY |
            RigidbodyConstraints.FreezeRotationX |
            RigidbodyConstraints.FreezeRotationZ;


        body.linearVelocity =
            Vector3.zero;


        body.angularVelocity =
            Vector3.zero;


        // =====================================================
        // 초기 충돌 힘
        // =====================================================

        body.AddForce(
            reactionDirection *
            impactImpulse,
            ForceMode.Impulse
        );


        Debug.Log(
            "[SpringTrainPhysics] Reaction 시작 / Direction = " +
            reactionDirection,
            this
        );
    }


    // =========================================================
    // 충돌 후 reactionDistance 만큼 이동
    // =========================================================

    private void FixedUpdate()
    {
        if (
            state !=
            TrainPhysicsState.Reacting
        )
        {
            return;
        }


        if (body == null)
        {
            return;
        }


        Vector3 delta =
            body.position -
            reactionStartPosition;


        // Y는 거리 계산에서 제외
        delta.y =
            0f;


        float movedDistance =
            delta.magnitude;


        // =====================================================
        // 지정 거리 이상 이동하면 종료
        // =====================================================

        if (
            movedDistance >=
            reactionDistance
        )
        {
            StopReaction();

            return;
        }


        // =====================================================
        // 마찰 때문에 중간에 멈추지 않도록
        // 일정한 속도 유지
        // =====================================================

        body.linearVelocity =
            reactionDirection *
            reactionSpeed;
    }


    // =========================================================
    // Reaction 종료
    // =========================================================

    private void StopReaction()
    {
        if (body == null)
        {
            return;
        }


        body.linearVelocity =
            Vector3.zero;


        body.angularVelocity =
            Vector3.zero;


        // 다시 완전 고정
        body.isKinematic =
            true;


        body.useGravity =
            false;


        body.constraints =
            RigidbodyConstraints.None;


        state =
            TrainPhysicsState.Finished;


        Debug.Log(
            "[SpringTrainPhysics] 충돌 후 이동 완료 : " +
            reactionDistance +
            " units",
            this
        );
    }


    // =========================================================
    // Player Tag 부모까지 검사
    //
    // Player Collider가 Player Root의 자식이어도
    // 정상적으로 Player로 판정하기 위함.
    // =========================================================

    private bool HasTagInParents(
        Transform target,
        string targetTag
    )
    {
        if (
            target == null ||
            string.IsNullOrEmpty(targetTag)
        )
        {
            return false;
        }


        Transform current =
            target;


        while (current != null)
        {
            if (
                current.CompareTag(
                    targetTag
                )
            )
            {
                return true;
            }


            current =
                current.parent;
        }


        return false;
    }


    // =========================================================
    // 실제 기차 두 대 사이 Collider 무시
    //
    // 이 함수는 첫 기차 충돌 이후에만 호출됨.
    //
    // Player와는 절대 IgnoreCollision하지 않음.
    // =========================================================

    private static void IgnoreCollisionBetweenTrains(
        SpringTrainPhysicsController first,
        SpringTrainPhysicsController second
    )
    {
        if (
            first == null ||
            second == null
        )
        {
            return;
        }


        Collider[] firstColliders =
            first.GetComponentsInChildren<Collider>(
                true
            );


        Collider[] secondColliders =
            second.GetComponentsInChildren<Collider>(
                true
            );


        for (
            int i = 0;
            i < firstColliders.Length;
            i++
        )
        {
            Collider firstCollider =
                firstColliders[i];


            if (firstCollider == null)
            {
                continue;
            }


            for (
                int j = 0;
                j < secondColliders.Length;
                j++
            )
            {
                Collider secondCollider =
                    secondColliders[j];


                if (secondCollider == null)
                {
                    continue;
                }


                Physics.IgnoreCollision(
                    firstCollider,
                    secondCollider,
                    true
                );
            }
        }
    }


    // =========================================================
    // Inspector
    // =========================================================

    private void OnValidate()
    {
        if (reactionDistance < 0f)
        {
            reactionDistance =
                0f;
        }


        if (reactionSpeed < 0f)
        {
            reactionSpeed =
                0f;
        }


        if (impactImpulse < 0f)
        {
            impactImpulse =
                0f;
        }
    }
}