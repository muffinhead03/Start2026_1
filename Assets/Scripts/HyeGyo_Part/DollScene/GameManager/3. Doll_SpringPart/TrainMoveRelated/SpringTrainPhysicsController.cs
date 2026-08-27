using UnityEngine;


[RequireComponent(typeof(Rigidbody))]
public class SpringTrainPhysicsController : MonoBehaviour
{
    // =========================================================
    // 역할
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

    [Header("충돌 후 최소 이동 거리")]
    [SerializeField]
    private float reactionDistance =
        10f;


    [Header("충돌 후 이동 속도")]
    [SerializeField]
    private float reactionSpeed =
        4f;


    [Header("충돌 순간 추가 Impulse")]
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


    private bool collisionHandled;


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


    private void Start()
    {
        IgnorePlayerCollisions();
    }


    // =========================================================
    // 정답 전 완전 고정
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


        body.constraints =
            RigidbodyConstraints.None;


        body.linearVelocity =
            Vector3.zero;


        body.angularVelocity =
            Vector3.zero;
    }


    // =========================================================
    // 움직이는 색깔 기차
    //
    // Kinematic이라 Player/선로가 밀 수 없음
    // =========================================================

    public void PrepareForPathMovement()
    {
        collisionHandled =
            false;


        state =
            TrainPhysicsState.PathMoving;


        body.useGravity =
            false;


        body.isKinematic =
            true;


        body.constraints =
            RigidbodyConstraints.None;


        body.linearVelocity =
            Vector3.zero;


        body.angularVelocity =
            Vector3.zero;
    }


    // =========================================================
    // 충돌 대상 검은 기차
    //
    // 실제 Collision Event를 받기 위해
    // Dynamic 상태로 두지만 FreezeAll한다.
    //
    // 따라서 움직이지는 않는다.
    // =========================================================

    public void PrepareForStationaryCollision()
    {
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
        if (collisionHandled)
        {
            return;
        }


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


        // 두 Root 모두 ToyTrain Tag여야 함
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


        // -----------------------------------------
        // 어느 쪽이 달려오던 기차인지 결정
        // -----------------------------------------

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
            return;
        }


        // =====================================================
        // 실제 충돌 발생
        // =====================================================

        movingTrain.collisionHandled =
            true;


        stationaryTrain.collisionHandled =
            true;


        Debug.Log(
            "[SpringTrainPhysics] 실제 기차 충돌 발생",
            this
        );


        // -----------------------------------------
        // Path 이동 즉시 중단
        // -----------------------------------------

        movingTrain.pathMover
            ?.StopMovementForCollision();


        stationaryTrain.pathMover
            ?.StopMovementForCollision();


        // -----------------------------------------
        // 충돌 순간의 실제 이동 방향
        // -----------------------------------------

        Vector3 impactDirection =
            Vector3.zero;


        if (
            movingTrain.pathMover != null
        )
        {
            impactDirection =
                movingTrain
                    .pathMover
                    .CurrentTravelWorldDirection;
        }


        // XZ 평면으로 제한
        impactDirection.y =
            0f;


        if (
            impactDirection.sqrMagnitude <
            0.000001f
        )
        {
            // 혹시 Path 방향을 얻지 못하면
            // Collision Contact Normal 사용
            if (collision.contactCount > 0)
            {
                impactDirection =
                    -collision
                        .GetContact(0)
                        .normal;


                impactDirection.y =
                    0f;
            }
        }


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


        // -----------------------------------------
        // 충돌 후 서로의 Collider는 무시
        //
        // 첫 실제 충돌만 사용하고
        // 이후 서로 다시 걸리지 않게 한다.
        // -----------------------------------------

        IgnoreTrainCollision(
            movingTrain,
            stationaryTrain
        );


        // -----------------------------------------
        // 색깔 기차 = 반대 방향으로 반동
        // -----------------------------------------

        movingTrain.BeginReaction(
            -impactDirection
        );


        // -----------------------------------------
        // 검은 기차 = 충돌 진행 방향으로 밀림
        // -----------------------------------------

        stationaryTrain.BeginReaction(
            impactDirection
        );
    }


    // =========================================================
    // 충돌 후 Physics Reaction
    // =========================================================

    private void BeginReaction(
        Vector3 direction
    )
    {
        reactionDirection =
            direction.normalized;


        reactionStartPosition =
            body.position;


        state =
            TrainPhysicsState.Reacting;


        body.constraints =
            RigidbodyConstraints.None;


        body.isKinematic =
            false;


        body.useGravity =
            false;


        // 넘어지지 않게
        body.constraints =
            RigidbodyConstraints.FreezePositionY |
            RigidbodyConstraints.FreezeRotationX |
            RigidbodyConstraints.FreezeRotationZ;


        body.linearVelocity =
            reactionDirection *
            reactionSpeed;


        body.angularVelocity =
            Vector3.zero;


        body.AddForce(
            reactionDirection *
            impactImpulse,
            ForceMode.Impulse
        );
    }


    // =========================================================
    // 최소 reactionDistance만큼 이동 보장
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


        Vector3 current =
            body.position;


        Vector3 delta =
            current -
            reactionStartPosition;


        delta.y =
            0f;


        float movedDistance =
            delta.magnitude;


        if (
            movedDistance >=
            reactionDistance
        )
        {
            StopReaction();

            return;
        }


        // 물리 마찰 때문에 중간에 멈추지 않도록
        // 최소 속도를 계속 유지
        body.linearVelocity =
            reactionDirection *
            reactionSpeed;
    }


    // =========================================================
    // Reaction 종료
    // =========================================================

    private void StopReaction()
    {
        body.linearVelocity =
            Vector3.zero;


        body.angularVelocity =
            Vector3.zero;


        body.isKinematic =
            true;


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
    // Player와 물리 충돌 완전 무시
    //
    // ToyTrain Tag와 Player Tag를 이용한다.
    // =========================================================

    private void IgnorePlayerCollisions()
    {
        GameObject[] players;


        try
        {
            players =
                GameObject.FindGameObjectsWithTag(
                    playerTag
                );
        }
        catch
        {
            Debug.LogWarning(
                "[SpringTrainPhysics] Player Tag를 찾을 수 없습니다.",
                this
            );


            return;
        }


        Collider[] trainColliders =
            GetComponentsInChildren<Collider>(
                true
            );


        for (int p = 0; p < players.Length; p++)
        {
            Collider[] playerColliders =
                players[p]
                    .GetComponentsInChildren<Collider>(
                        true
                    );


            for (
                int i = 0;
                i < trainColliders.Length;
                i++
            )
            {
                for (
                    int j = 0;
                    j < playerColliders.Length;
                    j++
                )
                {
                    if (
                        trainColliders[i] == null ||
                        playerColliders[j] == null
                    )
                    {
                        continue;
                    }


                    Physics.IgnoreCollision(
                        trainColliders[i],
                        playerColliders[j],
                        true
                    );
                }
            }
        }
    }


    // =========================================================
    // 기차끼리 첫 충돌 후에는 서로 무시
    // =========================================================

    private static void IgnoreTrainCollision(
        SpringTrainPhysicsController first,
        SpringTrainPhysicsController second
    )
    {
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
            for (
                int j = 0;
                j < secondColliders.Length;
                j++
            )
            {
                if (
                    firstColliders[i] == null ||
                    secondColliders[j] == null
                )
                {
                    continue;
                }


                Physics.IgnoreCollision(
                    firstColliders[i],
                    secondColliders[j],
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
            reactionDistance = 0f;
        }


        if (reactionSpeed < 0f)
        {
            reactionSpeed = 0f;
        }


        if (impactImpulse < 0f)
        {
            impactImpulse = 0f;
        }
    }
}