using UnityEngine;

public class FiveDial_FixCamera : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private Player_FixCamera playerFixCamera;

    [Header("Scene UI")]
    [SerializeField] private Scene_UI_Manager sceneUI;

    [Header("Camera Point")]
    [SerializeField] private Transform cameraPoint;

    [Header("State")]
    [SerializeField] private bool isCameraFixed = false;


    private void Start()
    {
        if (playerFixCamera == null)
        {
            playerFixCamera =
                FindFirstObjectByType<Player_FixCamera>();
        }

        if (sceneUI == null)
        {
            sceneUI =
                FindFirstObjectByType<Scene_UI_Manager>();
        }
    }


    // ================================
    // FiveDial 조사 시작
    // E 상호작용 시 호출
    // ================================

    public void FixCamera()
    {
        if (isCameraFixed)
            return;

        if (playerFixCamera == null)
        {
            Debug.LogWarning(
                "[FiveDial] Player_FixCamera가 없습니다."
            );

            return;
        }

        if (cameraPoint == null)
        {
            Debug.LogWarning(
                "[FiveDial] CameraPoint가 없습니다."
            );

            return;
        }


        isCameraFixed = true;


        // 기존 Player_FixCamera 사용
        playerFixCamera.FixCamera(
            cameraPoint.position,
            cameraPoint.forward,
            gameObject
        );


        // 마우스 커서 사용 가능
        if (sceneUI != null)
        {
            sceneUI.UnlockPointer();

            // 중앙 조준점 숨김
            sceneUI.SetActiveCursor(false);
        }


        Debug.Log("[FiveDial] 자물쇠 조사 시작");
    }


    // ================================
    // FiveDial 조사 종료
    // ================================

    public void UnFixCamera()
    {
        if (!isCameraFixed)
            return;

        isCameraFixed = false;


        if (playerFixCamera != null)
        {
            playerFixCamera.UnFixCamera();
        }


        if (sceneUI != null)
        {
            // 다시 FPS 조작 상태
            sceneUI.LockPointer();

            // 중앙 조준점 표시
            sceneUI.SetActiveCursor(true);
        }


        Debug.Log("[FiveDial] 자물쇠 조사 종료");
    }


    // ================================
    // 현재 조사 중인지
    // ================================

    public bool IsCameraFixed()
    {
        return isCameraFixed;
    }
}