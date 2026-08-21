using UnityEngine;

public class DollScene_ChangeBrokenLeg : MonoBehaviour
{
    [Header("Number Lock Mode")]
    [SerializeField] private GoToNumberLockMode numberLockMode;

    [Header("Number Lock State")]
    [SerializeField] private bool isCheckingNumberLock = false;
    [SerializeField] private bool isNumberLockSolved = false;

    private bool previousSolvedState;


    [Header("Doll Scene Game Manager")]
    [SerializeField] private DollScene_GameManager gameManager;

    [Header("Gate Lock")]
    [SerializeField] private OpenTheGateLock openTheGateLock;

    [Header("Player Camera")]
    [SerializeField] private Player_FixCamera playerFixCamera;


    [Header("Scene UI")]
    [SerializeField] private Scene_UI_Manager sceneUI;


    private void Start()
    {
        previousSolvedState = isNumberLockSolved;
        if (gameManager == null)
        {
            gameManager =
                GetComponentInParent<DollScene_GameManager>();
        }
    }

    private void Update()
    {
        // false → true로 변한 순간만 실행
        if (!previousSolvedState && isNumberLockSolved)
        {
            if (openTheGateLock != null)
            {
                openTheGateLock.OpenLock();
            }
            else
            {
                Debug.LogWarning("[BrokenLeg] OpenTheGateLock이 연결되지 않았습니다.");
            }
        }

        previousSolvedState = isNumberLockSolved;
    }

    // ================================================
    // 자물쇠 상호작용
    // E키 입력 시 호출
    // ================================================

    public void InteractNumberLock()
    {
        // 이미 해결한 자물쇠
        if (isNumberLockSolved)
            return;


        // 이미 조사 중이라면 E를 다시 눌러 종료
        if (isCheckingNumberLock)
        {
            ExitNumberLock();
            return;
        }


        if (numberLockMode == null)
        {
            Debug.LogWarning(
                "[BrokenLeg] GoToNumberLockMode가 없습니다."
            );

            return;
        }


        isCheckingNumberLock = true;


        // 중앙 조준 커서 숨김
        if (sceneUI != null)
        {
            sceneUI.SetActiveCursor(false);

            // 실제 마우스를 사용할 경우
            sceneUI.UnlockPointer();
        }


        // 자물쇠 조사 모드 진입
        numberLockMode.EnterMode();


        Debug.Log(
            "[BrokenLeg] 자물쇠 조사 시작"
        );
    }


    // ================================================
    // 자물쇠 조사 종료
    // ================================================

    public void ExitNumberLock()
    {
        if (!isCheckingNumberLock)
            return;


        isCheckingNumberLock = false;


        // 자물쇠 Mode 상태도 종료
        if (numberLockMode != null) {numberLockMode.ExitMode();}


        // 카메라 원래 위치로
        if (playerFixCamera != null) {playerFixCamera.UnFixCamera();}

        // Object_FixCamera.UnFixCamera와 같은 개념
        if (sceneUI != null)
        {
            // 중앙 UI 커서 다시 표시
            sceneUI.SetActiveCursor(true);
            // 실제 마우스를 다시 FPS 상태로
            sceneUI.LockPointer();
        }


        Debug.Log(
            "[BrokenLeg] 자물쇠 조사 종료"
        );
    }


    // 자물쇠 정답 검사
    public void CheckNumberLock()
    {
        /*
         * NumberLockManager가 생겼으므로
         * 추후 이 역할은 NumberLockManager로 이동 가능.
         *
         * 정답 : 3 - 4 - 1 - 5 - 2
         */
    }

    // 자물쇠 해결 완료
    // 자물쇠 해결 완료
public void CompleteNumberLock()
{
    if (isNumberLockSolved)    return;
    isNumberLockSolved = true;

    Debug.Log("[BrokenLeg] 자물쇠 해결 완료");

    if (openTheGateLock != null)
    {
        openTheGateLock.OpenLock();
    }
    else
    {
        Debug.LogWarning("[BrokenLeg] OpenTheGateLock이 연결되지 않았습니다.");
    }


    ExitNumberLock();

    UnlockHorseArea();
}

    // 목마 구역 해제
    private void UnlockHorseArea()
    {
        /*
         * TODO
         *
         * 울타리 자물쇠 해제
         * 울타리 Open
         */
    }


    public void CompleteLegChange()
    {
        if (gameManager != null)
        {
            gameManager.CompleteBrokenLeg();
        }
    }
}