using UnityEngine;


public enum SpringTrainCarSlot
{
    Front,
    Middle,
    Rear
}


public class SpringTrainFormationController : MonoBehaviour
{
    // =========================================================
    // Cars
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
    // Spacing
    // =========================================================

    [Header("기차 칸 사이 Path 거리")]
    [SerializeField]
    private float carSpacing =
        0.8f;


    // =========================================================
    // Model Rotation 보정
    // =========================================================

    [Header("모델 Rotation 보정")]

    [Tooltip(
        "Point Rotation과 실제 기차 모델 방향이 다를 때 사용합니다.\n" +
        "예: 모델이 반대로 보이면 Y = 180"
    )]

    [SerializeField]
    private Vector3 modelRotationOffset;


    // =========================================================
    // 외부 접근
    // =========================================================

    public SpringTrainCarFollower FrontCar =>
        frontCar;


    public SpringTrainCarFollower MiddleCar =>
        middleCar;


    public SpringTrainCarFollower RearCar =>
        rearCar;


    public float CarSpacing =>
        carSpacing;


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
            if (
                middleCar == null ||
                middleCar.CarRoot == null
            )
            {
                return transform.position;
            }


            return middleCar
                .CarRoot
                .position;
        }
    }


    // =========================================================
    // Formation
    // =========================================================

    public void ApplyFormation(
        SpringTrainTrack path,
        float middleDistance
    )
    {
        if (
            path == null ||
            !HasAllCars
        )
        {
            return;
        }


        ApplySingleCar(
            frontCar,
            path,
            middleDistance +
            carSpacing
        );


        ApplySingleCar(
            middleCar,
            path,
            middleDistance
        );


        ApplySingleCar(
            rearCar,
            path,
            middleDistance -
            carSpacing
        );
    }


    // =========================================================
    // 각 Car의 Distance Offset
    // =========================================================

    public float GetDistanceOffset(
        SpringTrainCarSlot slot
    )
    {
        switch (slot)
        {
            case SpringTrainCarSlot.Front:

                return carSpacing;


            case SpringTrainCarSlot.Rear:

                return -carSpacing;
        }


        return 0f;
    }


    // =========================================================
    // Single Car
    // =========================================================

    private void ApplySingleCar(
        SpringTrainCarFollower car,
        SpringTrainTrack path,
        float distance
    )
    {
        if (
            car == null ||
            car.CarRoot == null
        )
        {
            return;
        }


        Pose pose =
            path.EvaluatePose(
                distance
            );


        Quaternion rotation =
            pose.rotation *
            Quaternion.Euler(
                modelRotationOffset
            );


        car.CarRoot.SetPositionAndRotation(
            pose.position,
            rotation
        );
    }


    // =========================================================
    // Inspector
    // =========================================================

    private void OnValidate()
    {
        if (carSpacing < 0f)
        {
            carSpacing =
                0f;
        }
    }
}