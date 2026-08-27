using UnityEngine;


public class SpringTrainFormationController : MonoBehaviour
{
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
    private float carSpacing = 0.8f;


    // =========================================================
    // 기차의 물리적인 앞 방향
    // =========================================================

    [Header("앞 칸이 시계방향 쪽에 위치")]
    [Tooltip(
        "현재 배치에서 Front Car가 Middle Car보다 " +
        "시계방향 쪽에 있으면 체크합니다."
    )]
    [SerializeField]
    private bool frontIsClockwiseSide = true;


    // =========================================================
    // 회전 방향 계산
    // =========================================================

    [Header("회전 방향 확인 거리")]
    [Tooltip(
        "현재 위치보다 조금 앞의 Path를 확인해서 " +
        "각 기차 칸이 바라볼 방향을 계산합니다."
    )]
    [SerializeField]
    private float tangentLookAhead = 0.05f;


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


            return middleCar.Position;
        }
    }


    // =========================================================
    // PathMover가 계산한 Middle 진행 거리 기준으로
    // Front / Middle / Rear 전체 배치
    // =========================================================

    public void ApplyFormation(
        SpringTrainTrack.PathSnapshot pathSnapshot,
        float centerPathDistance,
        bool snapRotation
    )
    {
        if (pathSnapshot == null)
        {
            return;
        }


        if (!HasAllCars)
        {
            return;
        }


        float frontSideSign =
            frontIsClockwiseSide
                ? 1f
                : -1f;


        // =====================================================
        // Middle
        // =====================================================

        ApplySingleCar(
            pathSnapshot,
            middleCar,
            centerPathDistance,
            snapRotation
        );


        // =====================================================
        // Front
        // =====================================================

        ApplySingleCar(
            pathSnapshot,
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
            pathSnapshot,
            rearCar,
            centerPathDistance -
            carSpacing *
            frontSideSign,
            snapRotation
        );
    }


    // =========================================================
    // 기차 한 칸 위치 + 방향 적용
    // =========================================================

    private void ApplySingleCar(
        SpringTrainTrack.PathSnapshot pathSnapshot,
        SpringTrainCarFollower car,
        float pathDistance,
        bool snapRotation
    )
    {
        if (
            pathSnapshot == null ||
            car == null
        )
        {
            return;
        }


        Vector3 worldPosition =
            pathSnapshot.EvaluatePosition(
                pathDistance
            );


        // =====================================================
        // 기차의 물리적인 앞 방향은 유지
        //
        // 반동으로 반대 방향으로 이동하더라도
        // 기차 모델이 갑자기 180도 뒤집히지 않는다.
        // =====================================================

        SpringTrainDirection visualDirection =
            frontIsClockwiseSide
                ? SpringTrainDirection.Clockwise
                : SpringTrainDirection.CounterClockwise;


        Vector3 pathDirection =
            pathSnapshot.EvaluateDirection(
                pathDistance,
                visualDirection,
                tangentLookAhead
            );


        car.ApplyPathPose(
            worldPosition,
            pathDirection,
            snapRotation
        );
    }


    // =========================================================
    // Inspector 검사
    // =========================================================

    private void OnValidate()
    {
        if (carSpacing < 0f)
        {
            carSpacing = 0f;
        }


        if (tangentLookAhead < 0.001f)
        {
            tangentLookAhead = 0.001f;
        }
    }
}