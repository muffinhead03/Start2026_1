using UnityEngine;

public class FiveDial_CameraSetup : MonoBehaviour
{
    [Header("Existing Fix Camera")]
    [SerializeField]
    private Object_FixCamera objectFixCamera;


    [Header("View Setting")]

    // 카메라가 바라볼 자물쇠 중심
    [SerializeField]
    private Transform viewPivot;

    // 카메라가 실제로 이동할 위치
    [SerializeField]
    private Transform cameraPoint;


    private void Awake()
    {
        SetupCameraPosition();
    }


    // ================================================
    // CameraPoint 정보를
    // 기존 Object_FixCamera 방식으로 변환
    // ================================================

    public void SetupCameraPosition()
    {
        if (objectFixCamera == null)
        {
            Debug.LogWarning(
                "[FiveDial_CameraSetup] Object_FixCamera가 없습니다."
            );

            return;
        }


        if (viewPivot == null)
        {
            Debug.LogWarning(
                "[FiveDial_CameraSetup] ViewPivot이 없습니다."
            );

            return;
        }


        if (cameraPoint == null)
        {
            Debug.LogWarning(
                "[FiveDial_CameraSetup] CameraPoint가 없습니다."
            );

            return;
        }


        // Object_FixCamera가 바라볼 중심
        objectFixCamera.pivot = viewPivot;


        // Object_FixCamera는
        //
        // pivot.position + targetPos
        //
        // 로 최종 카메라 위치를 계산하므로
        // CameraPoint 위치와 같아지도록 Offset 계산

        objectFixCamera.targetPos =
            cameraPoint.position -
            viewPivot.position;


        Debug.Log(
            "[FiveDial_CameraSetup] Camera Setting Complete"
        );
    }
}