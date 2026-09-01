using UnityEngine;


public enum SpringTrainMovementPlane
{
    XZ,
    XY
}


public class SpringTrainCarFollower : MonoBehaviour
{
    // =========================================================
    // 실제 기차 모델
    // =========================================================

    [Header("실제로 움직일 기차 Transform")]
    [SerializeField]
    private Transform carRoot;


    // =========================================================
    // 이동 평면
    // =========================================================

    [Header("이동 평면")]
    [SerializeField]
    private SpringTrainMovementPlane movementPlane =
        SpringTrainMovementPlane.XZ;


    // =========================================================
    // 모델 방향 보정
    // =========================================================

    [Header("모델 Rotation 보정")]
    [SerializeField]
    private Vector3 rotationOffset;


    [Header("회전 속도")]
    [SerializeField]
    private float rotationSpeed = 8f;


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


    public Vector3 WorldPosition =>
        CarRoot.position;


    // =========================================================
    // Path 위치 적용
    // =========================================================

    public void ApplyPathPose(
        Vector3 worldPosition,
        Vector3 pathDirection,
        bool snapRotation
    )
    {
        Transform target =
            CarRoot;


        // Point A -> Point B 사이의
        // 정확한 World Position
        target.position =
            worldPosition;


        if (
            pathDirection.sqrMagnitude <
            0.000001f
        )
        {
            return;
        }


        Quaternion targetRotation;


        if (
            movementPlane ==
            SpringTrainMovementPlane.XZ
        )
        {
            Vector3 direction =
                new Vector3(
                    pathDirection.x,
                    0f,
                    pathDirection.z
                );


            if (
                direction.sqrMagnitude <
                0.000001f
            )
            {
                return;
            }


            targetRotation =
                Quaternion.LookRotation(
                    direction.normalized,
                    Vector3.up
                );
        }
        else
        {
            Vector3 direction =
                new Vector3(
                    pathDirection.x,
                    pathDirection.y,
                    0f
                );


            float angle =
                Mathf.Atan2(
                    direction.y,
                    direction.x
                ) *
                Mathf.Rad2Deg;


            targetRotation =
                Quaternion.Euler(
                    0f,
                    0f,
                    angle
                );
        }


        targetRotation *=
            Quaternion.Euler(
                rotationOffset
            );


        if (
            snapRotation ||
            rotationSpeed <= 0f
        )
        {
            target.rotation =
                targetRotation;
        }
        else
        {
            target.rotation =
                Quaternion.Slerp(
                    target.rotation,
                    targetRotation,
                    Mathf.Clamp01(
                        rotationSpeed *
                        Time.deltaTime
                    )
                );
        }
    }


    private void OnValidate()
    {
        if (rotationSpeed < 0f)
        {
            rotationSpeed = 0f;
        }
    }
}