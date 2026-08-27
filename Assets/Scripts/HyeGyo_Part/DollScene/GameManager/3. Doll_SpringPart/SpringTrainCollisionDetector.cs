using UnityEngine;


public class SpringTrainCollisionDetector : MonoBehaviour
{
    // =========================================================
    // 기차 구성
    // =========================================================

    [Header("기차 3칸 배치 Controller")]
    [SerializeField]
    private SpringTrainFormationController formationController;


    // =========================================================
    // 충돌
    // =========================================================

    [Header("기차 충돌 반경")]
    [Tooltip(
        "두 기차의 각 칸 중심 사이 거리가 " +
        "두 기차 Collision Radius의 합 이하가 되면 " +
        "충돌로 판단합니다."
    )]
    [SerializeField]
    private float collisionRadius = 0.5f;


    // =========================================================
    // 외부 접근
    // =========================================================

    public float CollisionRadius =>
        collisionRadius;


    public SpringTrainFormationController FormationController =>
        formationController;


    // =========================================================
    // 다른 기차와 충돌 검사
    //
    // Front / Middle / Rear
    // 총 3 x 3 = 9가지 조합을 검사
    // =========================================================

    public bool IsCollidingWith(
        SpringTrainCollisionDetector otherDetector
    )
    {
        if (otherDetector == null)
        {
            return false;
        }


        if (
            formationController == null ||
            otherDetector.formationController == null
        )
        {
            return false;
        }


        float collisionDistance =
            collisionRadius +
            otherDetector.collisionRadius;


        float collisionDistanceSqr =
            collisionDistance *
            collisionDistance;


        SpringTrainFormationController otherFormation =
            otherDetector.formationController;


        // =====================================================
        // 내 Front
        // =====================================================

        if (
            IsCarPairColliding(
                formationController.FrontCar,
                otherFormation.FrontCar,
                collisionDistanceSqr
            )
        )
        {
            return true;
        }


        if (
            IsCarPairColliding(
                formationController.FrontCar,
                otherFormation.MiddleCar,
                collisionDistanceSqr
            )
        )
        {
            return true;
        }


        if (
            IsCarPairColliding(
                formationController.FrontCar,
                otherFormation.RearCar,
                collisionDistanceSqr
            )
        )
        {
            return true;
        }


        // =====================================================
        // 내 Middle
        // =====================================================

        if (
            IsCarPairColliding(
                formationController.MiddleCar,
                otherFormation.FrontCar,
                collisionDistanceSqr
            )
        )
        {
            return true;
        }


        if (
            IsCarPairColliding(
                formationController.MiddleCar,
                otherFormation.MiddleCar,
                collisionDistanceSqr
            )
        )
        {
            return true;
        }


        if (
            IsCarPairColliding(
                formationController.MiddleCar,
                otherFormation.RearCar,
                collisionDistanceSqr
            )
        )
        {
            return true;
        }


        // =====================================================
        // 내 Rear
        // =====================================================

        if (
            IsCarPairColliding(
                formationController.RearCar,
                otherFormation.FrontCar,
                collisionDistanceSqr
            )
        )
        {
            return true;
        }


        if (
            IsCarPairColliding(
                formationController.RearCar,
                otherFormation.MiddleCar,
                collisionDistanceSqr
            )
        )
        {
            return true;
        }


        if (
            IsCarPairColliding(
                formationController.RearCar,
                otherFormation.RearCar,
                collisionDistanceSqr
            )
        )
        {
            return true;
        }


        return false;
    }


    // =========================================================
    // 기차 칸 두 개 거리 검사
    // =========================================================

    private bool IsCarPairColliding(
        SpringTrainCarFollower firstCar,
        SpringTrainCarFollower secondCar,
        float collisionDistanceSqr
    )
    {
        if (
            firstCar == null ||
            secondCar == null
        )
        {
            return false;
        }


        Vector3 difference =
            firstCar.Position -
            secondCar.Position;


        return
            difference.sqrMagnitude <=
            collisionDistanceSqr;
    }


    // =========================================================
    // Inspector 검사
    // =========================================================

    private void OnValidate()
    {
        if (collisionRadius < 0f)
        {
            collisionRadius = 0f;
        }
    }
}