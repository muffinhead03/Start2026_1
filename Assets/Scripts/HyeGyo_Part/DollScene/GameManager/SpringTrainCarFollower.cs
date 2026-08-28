using UnityEngine;


public enum SpringTrainMovementPlane
{
    XZ,
    XY
}


public class SpringTrainCarFollower : MonoBehaviour
{
    // =========================================================
    // 실제 움직일 기차 칸
    // =========================================================

    [Header("실제로 움직일 기차 칸")]
    [Tooltip(
        "비워두면 이 스크립트가 붙어 있는 Transform을 사용합니다."
    )]
    [SerializeField]
    private Transform carRoot;


    // =========================================================
    // 회전
    // =========================================================

    [Header("경로 방향으로 회전")]
    [SerializeField]
    private bool rotateAlongPath = true;


    [Header("이동 평면")]
    [SerializeField]
    private SpringTrainMovementPlane movementPlane =
        SpringTrainMovementPlane.XZ;


    [Header("모델 방향 보정")]
    [Tooltip(
        "기차 모델의 Forward 방향이 Unity +Z와 다를 경우 보정합니다."
    )]
    [SerializeField]
    private Vector3 rotationOffset;


    [Header("회전 속도")]
    [Tooltip(
        "0이면 즉시 회전합니다."
    )]
    [SerializeField]
    private float rotationSpeed = 720f;


    // =========================================================
    // 외부 접근
    // =========================================================

    public Transform CarRoot
    {
        get
        {
            if (carRoot != null)
            {
                return carRoot;
            }

            return transform;
        }
    }


    public Vector3 Position =>
        CarRoot.position;


    // =========================================================
    // Unity
    // =========================================================

    private void Awake()
    {
        if (carRoot == null)
        {
            carRoot =
                transform;
        }
    }


    // =========================================================
    // PathMover가 계산한 Pose 적용
    // =========================================================

    public void ApplyPathPose(
        Vector3 worldPosition,
        Vector3 pathDirection,
        bool snapRotation = false
    )
    {
        Transform root =
            CarRoot;


        if (root == null)
        {
            return;
        }


        // 위치
        root.position =
            worldPosition;


        if (!rotateAlongPath)
        {
            return;
        }


        if (
            pathDirection.sqrMagnitude <
            0.000001f
        )
        {
            return;
        }


        Quaternion targetRotation =
            CalculateRotation(
                pathDirection
            );


        if (
            snapRotation ||
            rotationSpeed <= 0f
        )
        {
            root.rotation =
                targetRotation;

            return;
        }


        root.rotation =
            Quaternion.RotateTowards(
                root.rotation,
                targetRotation,
                rotationSpeed *
                Time.deltaTime
            );
    }


    // =========================================================
    // 진행 방향 → Rotation
    // =========================================================

    private Quaternion CalculateRotation(
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
                return
                    CarRoot.rotation;
            }


            Quaternion rotation =
                Quaternion.LookRotation(
                    direction.normalized,
                    Vector3.up
                );


            return
                rotation *
                Quaternion.Euler(
                    rotationOffset
                );
        }


        // XY 평면
        direction.z =
            0f;


        if (
            direction.sqrMagnitude <
            0.000001f
        )
        {
            return
                CarRoot.rotation;
        }


        float angle =
            Mathf.Atan2(
                direction.y,
                direction.x
            ) *
            Mathf.Rad2Deg;


        return
            Quaternion.Euler(
                0f,
                0f,
                angle
            ) *
            Quaternion.Euler(
                rotationOffset
            );
    }


    // =========================================================
    // Inspector
    // =========================================================

    private void OnValidate()
    {
        if (rotationSpeed < 0f)
        {
            rotationSpeed =
                0f;
        }
    }
}