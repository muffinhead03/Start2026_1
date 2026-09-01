using UnityEngine;


public class SpringTrainFormationController : MonoBehaviour
{
    // =========================================================
    // 기차 3칸
    // =========================================================

    [Header("앞 칸")]
    [SerializeField]
    private SpringTrainCarFollower frontCar;


    [Header("가운데 칸")]
    [SerializeField]
    private SpringTrainCarFollower middleCar;


    [Header("뒤 칸")]
    [SerializeField]
    private SpringTrainCarFollower rearCar;


    // =========================================================
    // 간격
    // =========================================================

    [Header("기차 칸 사이 Path 거리")]
    [SerializeField]
    private float carSpacing = 0.8f;


    [Header("Front가 시계방향 앞쪽인가")]
    [SerializeField]
    private bool frontIsClockwiseSide = true;


    // =========================================================
    // 외부 접근
    // =========================================================

    public SpringTrainCarFollower FrontCar =>
        frontCar;


    public SpringTrainCarFollower MiddleCar =>
        middleCar;


    public SpringTrainCarFollower RearCar =>
        rearCar;


    public bool HasAllCars =>
        frontCar != null &&
        middleCar != null &&
        rearCar != null;


    public Transform TrainCenter
    {
        get
        {
            if (middleCar == null)
            {
                return null;
            }


            return middleCar.CarRoot;
        }
    }


    public Vector3 MiddlePosition
    {
        get
        {
            if (middleCar == null)
            {
                return transform.position;
            }


            return middleCar.WorldPosition;
        }
    }


    // =========================================================
    // 세 칸을 동일 Path 위에 배치
    // =========================================================

    public void ApplyFormation(
        SpringTrainTrack.PathSnapshot path,
        float middleDistance,
        bool snapRotation
    )
    {
        if (
            path == null ||
            !HasAllCars
        )
        {
            return;
        }


        float sign =
            frontIsClockwiseSide
                ? 1f
                : -1f;


        float frontDistance =
            middleDistance +
            carSpacing *
            sign;


        float rearDistance =
            middleDistance -
            carSpacing *
            sign;


        ApplySingleCar(
            frontCar,
            path,
            frontDistance,
            snapRotation
        );


        ApplySingleCar(
            middleCar,
            path,
            middleDistance,
            snapRotation
        );


        ApplySingleCar(
            rearCar,
            path,
            rearDistance,
            snapRotation
        );
    }


    // =========================================================
    // 한 칸
    // =========================================================

    private void ApplySingleCar(
        SpringTrainCarFollower car,
        SpringTrainTrack.PathSnapshot path,
        float distance,
        bool snapRotation
    )
    {
        Vector3 position =
            path.EvaluatePosition(
                distance
            );


        SpringTrainDirection directionType =
            frontIsClockwiseSide
                ? SpringTrainDirection.Clockwise
                : SpringTrainDirection.CounterClockwise;


        Vector3 direction =
            path.EvaluateDirection(
                distance,
                directionType
            );


        car.ApplyPathPose(
            position,
            direction,
            snapRotation
        );
    }


    private void OnValidate()
    {
        if (carSpacing < 0f)
        {
            carSpacing = 0f;
        }
    }
}