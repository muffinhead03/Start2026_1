using UnityEngine;

public class DollScene_ChangeBrokenLeg : MonoBehaviour
{
    [Header("Doll Scene Game Manager")]
    [SerializeField] private DollScene_GameManager gameManager;


    [Header("Player Camera")]
    [SerializeField] private Player_FixCamera playerFixCamera;


    [Header("Scene UI")]
    [SerializeField] private Scene_UI_Manager sceneUI;


    [Header("Number Lock Camera Point")]
    [SerializeField] private Transform numberLockCameraPoint;


    [Header("Number Lock Fix Object")]
    [SerializeField] private GameObject numberLockObject;


    [Header("Number Lock State")]
    [SerializeField] private bool isCheckingNumberLock = false;


    private void Start()
    {
        if (gameManager == null)
        {
            gameManager =
                GetComponentInParent<DollScene_GameManager>();
        }
    }


    // ================================
    // 자물쇠 상호작용
    // E키 입력 시 호출
    // ================================

    public void InteractNumberLock()
    {
        if (isCheckingNumberLock)
            return;


        if (playerFixCamera == null)
        {
            Debug.LogWarning(
                "[BrokenLeg] Player_FixCamera가 연결되지 않았습니다."
            );

            return;
        }


        if (numberLockCameraPoint == null)
        {
            Debug.LogWarning(
                "[BrokenLeg] NumberLock CameraPoint가 연결되지 않았습니다."
            );

            return;
        }


        if (numberLockObject == null)
        {
            Debug.LogWarning(
                "[BrokenLeg] NumberLock Object가 연결되지 않았습니다."
            );

            return;
        }


        isCheckingNumberLock = true;


        // 카메라를 자물쇠 앞으로 이동
        playerFixCamera.FixCamera(
            numberLockCameraPoint.position,
            numberLockCameraPoint.forward,
            numberLockObject
        );


        // 마우스 커서 사용 가능
        if (sceneUI != null)
        {
            sceneUI.UnlockPointer();

            // 기존 중앙 조준 커서를 사용 중이라면 숨김
            sceneUI.SetActiveCursor(false);
        }


        Debug.Log("[BrokenLeg] 자물쇠 조사 시작");
    }


    // ================================
    // 자물쇠 조사 종료
    // ================================

    public void ExitNumberLock()
    {
        if (!isCheckingNumberLock)
            return;


        isCheckingNumberLock = false;


        // 카메라 원래 위치로
        if (playerFixCamera != null)
        {
            playerFixCamera.UnFixCamera();
        }


        // FPS 조작 상태로 복귀
        if (sceneUI != null)
        {
            sceneUI.LockPointer();
            sceneUI.SetActiveCursor(true);
        }


        Debug.Log("[BrokenLeg] 자물쇠 조사 종료");
    }


    // ================================
    // 자물쇠 검사
    // ================================

    public void CheckNumberLock()
    {
        /*
         * TODO : 자물쇠 숫자 판정
         *
         * 정답 : 3 - 4 - 1 - 5 - 2
         *
         * if (number1 == 3 &&
         *     number2 == 4 &&
         *     number3 == 1 &&
         *     number4 == 5 &&
         *     number5 == 2)
         * {
         *     UnlockHorseArea();
         * }
         */
    }


    // ================================
    // 목마 구역 해제
    // ================================

    private void UnlockHorseArea()
    {
        /*
         * TODO
         *
         * 울타리 자물쇠 해제
         * 울타리 Open
         *
         * 이후 플레이어가
         * 두 번째 / 세 번째 목마 사이에서
         * 인형 다리를 획득할 수 있음.
         */
    }


    // ================================
    // 다리 교체 완료
    // ================================

    public void CompleteLegChange()
    {
        if (gameManager != null)
        {
            gameManager.CompleteBrokenLeg();
        }
    }
}