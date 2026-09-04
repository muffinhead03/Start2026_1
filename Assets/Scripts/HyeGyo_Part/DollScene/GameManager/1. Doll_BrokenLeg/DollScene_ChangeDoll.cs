using UnityEngine;

public class DollScene_ChangeDoll : MonoBehaviour
{
    [Header("Doll Scene Game Manager")]
    [SerializeField]
    private DollScene_GameManager gameManager;


    // =========================================================
    // 외부 상태 확인
    // =========================================================

    public bool IsBrokenLegFixed =>
        gameManager != null &&
        gameManager.IsBrokenLegChanged;

    public bool IsBrokenArmFixed =>
        gameManager != null &&
        gameManager.IsBrokenArmChanged;

    public bool IsSpringFixed =>
        gameManager != null &&
        gameManager.IsSpringFound;

    public bool IsDollRepairCompleted =>
        gameManager != null &&
        gameManager.IsDollRepaired;


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


        if (gameManager == null)
        {
            Debug.LogError(
                "[ChangeDoll] DollScene_GameManager를 찾을 수 없습니다.",
                this
            );
        }
    }


    // =========================================================
    // 다리 교체 완료
    // =========================================================

    public void ReportBrokenLegFixed()
    {
        if (gameManager == null)
            return;


        gameManager.CompleteBrokenLeg();


        Debug.Log(
            "[ChangeDoll] 다리 교체 완료 → GameManager 전달"
        );


    }


    // =========================================================
    // 팔 교체 완료
    // =========================================================

    public void ReportBrokenArmFixed()
    {
        if (gameManager == null)
            return;


        gameManager.CompleteBrokenArm();


        Debug.Log(
            "[ChangeDoll] 팔 교체 완료 → GameManager 전달"
        );


    }


    // =========================================================
    // 태엽 설치 완료
    // =========================================================

    public void ReportSpringFixed()
    {
        if (gameManager == null)
            return;


        gameManager.CompleteFindSpring();


        Debug.Log(
            "[ChangeDoll] 태엽 고정 완료 → GameManager 전달"
        );


    }


    // =========================================================
    // 전체 수리 조건 확인
    // =========================================================


    // =========================================================
    // 인형 전체 수리 완료
    // =========================================================

    public void RepairDoll()
    {
        if (gameManager == null)
            return;


        if (gameManager.IsDollRepaired)
        {
            return;
        }


        if (!gameManager.CanRepairDoll())
        {
            Debug.Log(
                "[ChangeDoll] 아직 필요한 부품이 부족합니다."
            );

            return;
        }


        gameManager.CompleteDollRepair();


        Debug.Log(
            "[ChangeDoll] 인형 전체 수리 완료 → GameManager 저장"
        );
    }


    // =========================================================
    // Exit Key 낙하 완료
    //
    // DollKeyDropManager에서 호출
    // =========================================================

    public void ReportExitKeyDropped()
    {
        if (gameManager == null)
        {
            Debug.LogWarning(
                "[ChangeDoll] GameManager가 없어 Key Drop 상태를 전달할 수 없습니다.",
                this
            );

            return;
        }


        gameManager.CompleteExitKeyDrop();


        Debug.Log(
            "[ChangeDoll] Exit Key 낙하 완료 → GameManager 전달"
        );
    }


    // =========================================================
    // 기존 UnityEvent 연결 호환용
    // =========================================================

    public void DropExitKey()
    {
        ReportExitKeyDropped();
    }
}