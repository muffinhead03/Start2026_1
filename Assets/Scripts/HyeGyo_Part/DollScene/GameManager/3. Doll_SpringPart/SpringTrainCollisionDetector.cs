using UnityEngine;


public class SpringTrainCollisionDetector : MonoBehaviour
{
    // =========================================================
    // Formation
    // =========================================================

    [Header("기차 Formation")]
    [SerializeField]
    private SpringTrainFormationController formationController;


    // =========================================================
    // Collision
    // =========================================================

    [Header("기차 칸 충돌 반경")]
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
    // =========================================================

    public bool IsCollidingWith(
        SpringTrainCollisionDetector other
    )
    {
        if (
            other == null ||
            formationController == null ||
            other.formationController == null
        )
        {
            return false;
        }


        SpringTrainCarFollower[] myCars =
        {
            formationController.FrontCar,
            formationController.MiddleCar,
            formationController.RearCar
        };


        SpringTrainCarFollower[] otherCars =
        {
            other.formationController.FrontCar,
            other.formationController.MiddleCar,
            other.formationController.RearCar
        };


        // 3 x 3
        for (int i = 0; i < myCars.Length; i++)
        {
            if (myCars[i] == null)
            {
                continue;
            }


            for (int j = 0; j < otherCars.Length; j++)
            {
                if (otherCars[j] == null)
                {
                    continue;
                }


                if (
                    IsCarPairColliding(
                        myCars[i],
                        otherCars[j],
                        collisionRadius,
                        other.collisionRadius
                    )
                )
                {
                    return true;
                }
            }
        }


        return false;
    }


    // =========================================================
    // 기차 한 칸끼리 거리 검사
    // =========================================================

    private bool IsCarPairColliding(
        SpringTrainCarFollower first,
        SpringTrainCarFollower second,
        float firstRadius,
        float secondRadius
    )
    {
        Vector3 firstPosition =
            first.WorldPosition;


        Vector3 secondPosition =
            second.WorldPosition;


        float totalRadius =
            firstRadius +
            secondRadius;


        return
            (
                firstPosition -
                secondPosition
            ).sqrMagnitude
            <=
            totalRadius *
            totalRadius;
    }


    // =========================================================
    // Inspector
    // =========================================================

    private void OnValidate()
    {
        if (collisionRadius < 0f)
        {
            collisionRadius = 0f;
        }
    }
}