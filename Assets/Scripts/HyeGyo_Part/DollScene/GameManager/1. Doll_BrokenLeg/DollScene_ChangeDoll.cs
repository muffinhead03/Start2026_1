using UnityEngine;

public class DollScene_ChangeDoll : MonoBehaviour
{
    [Header("Doll Scene Game Manager")]
    [SerializeField]
    private DollScene_GameManager gameManager;


    // =========================================================
    // 외부에서 상태 확인
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
    // 부러진 다리 교체 완료
    //
    // DollMoveManager에서 호출
    // =========================================================

    public void ReportBrokenLegFixed()
    {
        if (gameManager == null)
            return;


        gameManager.CompleteBrokenLeg();


        Debug.Log(
            "[ChangeDoll] 다리 교체 완료 → GameManager 전달"
        );


        CheckRepairComplete();
    }


    // =========================================================
    // 부러진 팔 교체 완료
    // =========================================================

    public void ReportBrokenArmFixed()
    {
        if (gameManager == null)
            return;


        gameManager.CompleteBrokenArm();


        Debug.Log(
            "[ChangeDoll] 팔 교체 완료 → GameManager 전달"
        );


        CheckRepairComplete();
    }


    // =========================================================
    // 태엽 고정 완료
    // =========================================================

    public void ReportSpringFixed()
    {
        if (gameManager == null)
            return;


        /*
         * 현재 GameManager에서는
         * isSpringFound라는 이름을 사용하고 있으므로
         * 기존 CompleteFindSpring()을 그대로 사용합니다.
         *
         * 지금 게임 흐름에서는
         * "태엽 설치까지 완료" 상태로 사용합니다.
         */
        gameManager.CompleteFindSpring();


        Debug.Log(
            "[ChangeDoll] 태엽 고정 완료 → GameManager 전달"
        );


        CheckRepairComplete();
    }


    // =========================================================
    // 세 부품 완료 검사
    // =========================================================

    private void CheckRepairComplete()
    {
        if (gameManager == null)
            return;


        if (!gameManager.CanRepairDoll())
        {
            Debug.Log(
                "[ChangeDoll] 아직 인형 수리 조건이 부족합니다."
            );

            return;
        }


        RepairDoll();
    }


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


        /*
         * =====================================================
         * TODO
         *
         * 이후 DollMoveManager의
         * 인형 자세 복구 연출과 연결할 예정
         *
         * =====================================================
         */
    }


    // =========================================================
    // 현재는 DollFixedCheckManager가 Key를 담당
    // =========================================================

    public void DropExitKey()
    {
        /*
         * Key 낙하는 DollFixedCheckManager에서 담당합니다.
         *
         * 이 함수는 기존 연결 호환성을 위해 남겨둡니다.
         */
    }
}