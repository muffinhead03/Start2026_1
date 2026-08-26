using UnityEngine;

public class DollScene_ChangeBrokenLeg : MonoBehaviour
{
    [Header("Doll Scene Game Manager")]
    [SerializeField]
    private DollScene_GameManager gameManager;


    [Header("Number Lock")]
    [SerializeField]
    private GoToNumberLockMode numberLockMode;

    [SerializeField]
    private OpenTheGateLock openTheGateLock;


    [Header("Player")]
    [SerializeField]
    private Player_Grab playerGrab;

    [SerializeField]
    private Player_FixCamera playerFixCamera;


    [Header("Scene UI")]
    [SerializeField]
    private Scene_UI_Manager sceneUI;


    [Header("Number Lock State")]
    [SerializeField]
    private bool isCheckingNumberLock = false;

    [SerializeField]
    private bool isNumberLockSolved = false;


    [Header("Leg Item State")]
    [Tooltip("수리용 다리 Object_Grabbable의 objectName")]
    [SerializeField]
    private string repairedLegItemKey = "DollLeftLeg";

    [SerializeField]
    private bool isRepairedLegFound = false;


    public bool IsNumberLockSolved =>
        isNumberLockSolved;

    public bool IsRepairedLegFound =>
        isRepairedLegFound;


    private void Start()
    {
        if (gameManager == null)
        {
            gameManager =
                GetComponentInParent<DollScene_GameManager>();
        }


        if (gameManager == null)
        {
            gameManager =
                FindFirstObjectByType<DollScene_GameManager>();
        }


        if (playerGrab == null)
        {
            playerGrab =
                FindFirstObjectByType<Player_Grab>();
        }
    }


    private void Update()
    {
        CheckRepairedLeg();
    }


    // =============================================
    // 자물쇠 상호작용
    // =============================================

    public void InteractNumberLock()
    {
        // 이미 해결된 자물쇠
        if (isNumberLockSolved)
            return;


        // 이미 조사 중이면 종료
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


        // 중앙 조준점 숨기기 / 마우스 해제
        if (sceneUI != null)
        {
            sceneUI.SetActiveCursor(false);
            sceneUI.UnlockPointer();
        }


        numberLockMode.EnterMode();


        Debug.Log(
            "[BrokenLeg] 자물쇠 조사 시작"
        );
    }


    // =============================================
    // 자물쇠 조사 종료
    // =============================================

    public void ExitNumberLock()
    {
        if (!isCheckingNumberLock)
            return;


        isCheckingNumberLock = false;


        if (numberLockMode != null)
        {
            numberLockMode.ExitMode();
        }


        if (playerFixCamera != null)
        {
            playerFixCamera.UnFixCamera();
        }


        if (sceneUI != null)
        {
            sceneUI.SetActiveCursor(true);
            sceneUI.LockPointer();
        }


        Debug.Log(
            "[BrokenLeg] 자물쇠 조사 종료"
        );
    }


    // =============================================
    // 자물쇠 해결 완료
    // NumberLock 쪽에서 호출
    // =============================================

    public void CompleteNumberLock()
    {
        if (isNumberLockSolved)
            return;


        isNumberLockSolved = true;


        Debug.Log(
            "[BrokenLeg] 자물쇠 해결 완료"
        );


        // 울타리 / 문 열기
        if (openTheGateLock != null)
        {
            openTheGateLock.OpenLock();
        }
        else
        {
            Debug.LogWarning(
                "[BrokenLeg] OpenTheGateLock이 연결되지 않았습니다."
            );
        }


        ExitNumberLock();
    }


    // =============================================
    // 수리용 다리 획득 여부 확인
    // =============================================

    private void CheckRepairedLeg()
    {
        // 이미 획득했다고 보고했으면 끝
        if (isRepairedLegFound)
            return;


        if (playerGrab == null)
            return;


        // 현재 플레이어가 들고 있는 아이템 확인
        if (!playerGrab.hasKey(repairedLegItemKey))
            return;


        CompleteFindRepairedLeg();
    }


    // =============================================
    // 수리용 다리 획득 완료
    // =============================================

    private void CompleteFindRepairedLeg()
    {
        if (isRepairedLegFound)
            return;


        isRepairedLegFound = true;


        Debug.Log(
            "[BrokenLeg] 수리용 인형 다리 획득 완료"
        );


        // DollScene_GameManager에 보고
        if (gameManager != null)
        {
            gameManager.CompleteBrokenLeg();
        }
        else
        {
            Debug.LogWarning(
                "[BrokenLeg] DollScene_GameManager가 없습니다."
            );
        }
    }
}