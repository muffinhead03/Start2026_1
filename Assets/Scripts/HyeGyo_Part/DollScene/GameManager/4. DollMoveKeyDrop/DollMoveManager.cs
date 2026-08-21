using UnityEngine;

public class DollMoveManager : MonoBehaviour
{
    // =========================================================
    // Player
    // =========================================================

    [Header("Player")]
    [SerializeField]
    private Player_Grab playerGrab;


    // =========================================================
    // Change Doll
    // =========================================================

    [Header("Change Doll")]
    [SerializeField]
    private DollScene_ChangeDoll changeDoll;


    // =========================================================
    // Key Drop Manager
    // =========================================================

    [Header("Key Drop Manager")]
    [SerializeField]
    private DollKeyDropManager keyDropManager;


    // =========================================================
    // 다리
    // =========================================================

    [Header("다리")]

    [SerializeField]
    private string legItemName = "DollLeg";

    [Tooltip("인형에 붙어있는 비활성 정상 다리")]
    [SerializeField]
    private GameObject fixedLegObject;


    // =========================================================
    // 팔
    // =========================================================

    [Header("팔")]

    [SerializeField]
    private string armItemName = "DollArm";

    [Tooltip("인형에 붙어있는 비활성 정상 팔")]
    [SerializeField]
    private GameObject fixedArmObject;


    // =========================================================
    // 태엽
    // =========================================================

    [Header("태엽")]

    [SerializeField]
    private string springItemName = "Spring";

    [Tooltip("인형에 붙어있는 비활성 태엽")]
    [SerializeField]
    private GameObject fixedSpringObject;


    // =========================================================
    // State
    // =========================================================

    private bool repairCompletedSent = false;


    // =========================================================
    // Start
    // =========================================================

    private void Start()
    {
        if (playerGrab == null)
        {
            playerGrab =
                FindFirstObjectByType<Player_Grab>();
        }


        if (changeDoll == null)
        {
            changeDoll =
                FindFirstObjectByType<DollScene_ChangeDoll>();
        }


        if (keyDropManager == null)
        {
            keyDropManager =
                FindFirstObjectByType<DollKeyDropManager>();
        }


        Debug.Log(
            "[DollMove] DollMoveManager 초기화 완료"
        );
    }


    // =========================================================
    // 다리 교체
    // =========================================================

    public void FixBrokenLeg()
    {
        if (playerGrab == null ||
            changeDoll == null)
        {
            return;
        }


        // 이미 수리됨
        if (changeDoll.IsBrokenLegFixed)
        {
            Debug.Log(
                "[DollMove] 다리는 이미 수리되었습니다."
            );

            return;
        }


        // 필요한 다리를 들고 있는지 확인
        if (!playerGrab.hasKey(legItemName))
        {
            Debug.Log(
                $"[DollMove] 필요한 다리가 없습니다 : {legItemName}"
            );

            return;
        }


        // 가져온 실제 아이템 소비
        playerGrab.UseKey();


        // 인형 몸에 붙어있는 정상 다리 활성화
        if (fixedLegObject != null)
        {
            fixedLegObject.SetActive(true);
        }


        // ChangeDoll → GameManager에 완료 전달
        changeDoll.ReportBrokenLegFixed();


        Debug.Log(
            "[DollMove] 다리 교체 완료"
        );


        // 전체 수리 상태 확인
        CheckAllRepairCompleted();
    }


    // =========================================================
    // 팔 교체
    // =========================================================

    public void FixBrokenArm()
    {
        if (playerGrab == null ||
            changeDoll == null)
        {
            return;
        }


        if (changeDoll.IsBrokenArmFixed)
        {
            Debug.Log(
                "[DollMove] 팔은 이미 수리되었습니다."
            );

            return;
        }


        if (!playerGrab.hasKey(armItemName))
        {
            Debug.Log(
                $"[DollMove] 필요한 팔이 없습니다 : {armItemName}"
            );

            return;
        }


        playerGrab.UseKey();


        if (fixedArmObject != null)
        {
            fixedArmObject.SetActive(true);
        }


        changeDoll.ReportBrokenArmFixed();


        Debug.Log(
            "[DollMove] 팔 교체 완료"
        );


        CheckAllRepairCompleted();
    }


    // =========================================================
    // 태엽 설치
    // =========================================================

    public void FixSpring()
    {
        if (playerGrab == null ||
            changeDoll == null)
        {
            return;
        }


        if (changeDoll.IsSpringFixed)
        {
            Debug.Log(
                "[DollMove] 태엽은 이미 설치되었습니다."
            );

            return;
        }


        if (!playerGrab.hasKey(springItemName))
        {
            Debug.Log(
                $"[DollMove] 필요한 태엽이 없습니다 : {springItemName}"
            );

            return;
        }


        playerGrab.UseKey();


        if (fixedSpringObject != null)
        {
            fixedSpringObject.SetActive(true);
        }


        changeDoll.ReportSpringFixed();


        Debug.Log(
            "[DollMove] 태엽 설치 완료"
        );


        CheckAllRepairCompleted();
    }


    // =========================================================
    // 전체 수리 완료 체크
    //
    // 다리 + 팔 + 태엽
    // 모두 완료되면 KeyDropManager에게 전달
    // =========================================================

    private void CheckAllRepairCompleted()
    {
        if (changeDoll == null)
        {
            return;
        }


        bool allCompleted =
            changeDoll.IsBrokenLegFixed &&
            changeDoll.IsBrokenArmFixed &&
            changeDoll.IsSpringFixed;


        Debug.Log(
            $"[DollMove] 전체 수리 상태 체크 : {allCompleted}"
        );


        if (!allCompleted)
        {
            return;
        }


        // 중복 전달 방지
        if (repairCompletedSent)
        {
            return;
        }


        repairCompletedSent =
            true;


        Debug.Log(
            "[DollMove] 모든 부품 수리 완료 → KeyDropManager 전달"
        );


        // -----------------------------------------------------
        // GameManager에도 인형 수리 완료 상태 저장
        // -----------------------------------------------------

        changeDoll.RepairDoll();


        // -----------------------------------------------------
        // Pose 복구 + Key Drop 시퀀스 시작
        // -----------------------------------------------------

        if (keyDropManager != null)
        {
            keyDropManager.SetRepairCompleted(
                true
            );
        }
        else
        {
            Debug.LogWarning(
                "[DollMove] DollKeyDropManager가 연결되지 않았습니다.",
                this
            );
        }
    }
}