using UnityEngine;

public class DollPartRepairManager : MonoBehaviour
{
    [Header("Player")]
    [SerializeField]
    private Player_Grab playerGrab;


    [Header("Doll State")]
    [SerializeField]
    private DollScene_ChangeDoll changeDoll;


    [Header("Completion")]
    [SerializeField]
    private DollRepairCompletionManager completionManager;


    // =========================================================
    // 인형에 미리 붙어있는 정상 부품
    // =========================================================

    [Header("Fixed Doll Parts")]

    [SerializeField]
    private GameObject fixedLegObject;

    [SerializeField]
    private GameObject fixedArmObject;

    [SerializeField]
    private GameObject fixedSpringObject;


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


        if (completionManager == null)
        {
            completionManager =
                FindFirstObjectByType<DollRepairCompletionManager>();
        }


        ApplyVisualState();
    }


    // =========================================================
    // 다리
    // =========================================================

    public void FixLeg()
    {
        if (playerGrab == null ||
            changeDoll == null)
        {
            return;
        }


        if (changeDoll.IsBrokenLegFixed)
        {
            Debug.Log(
                "[DollPartRepair] 다리는 이미 수리되었습니다."
            );

            return;
        }


        // 플레이어가 들고 있던 실제 부품 소비
        playerGrab.UseKey();


        // 인형에 붙어있는 정상 다리 활성화
        if (fixedLegObject != null)
        {
            fixedLegObject.SetActive(true);
        }
        else
        {
            Debug.LogWarning(
                "[DollPartRepair] Fixed Leg Object가 없습니다.",
                this
            );
        }


        changeDoll.ReportBrokenLegFixed();


        Debug.Log(
            "[DollPartRepair] ★ 다리 장착 완료 ★"
        );


        CheckCompletion();
    }


    // =========================================================
    // 팔
    // =========================================================

    public void FixArm()
    {
        if (playerGrab == null ||
            changeDoll == null)
        {
            return;
        }


        if (changeDoll.IsBrokenArmFixed)
        {
            Debug.Log(
                "[DollPartRepair] 팔은 이미 수리되었습니다."
            );

            return;
        }


        playerGrab.UseKey();


        if (fixedArmObject != null)
        {
            fixedArmObject.SetActive(true);
        }
        else
        {
            Debug.LogWarning(
                "[DollPartRepair] Fixed Arm Object가 없습니다.",
                this
            );
        }


        changeDoll.ReportBrokenArmFixed();


        Debug.Log(
            "[DollPartRepair] ★ 팔 장착 완료 ★"
        );


        CheckCompletion();
    }


    // =========================================================
    // 태엽
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
                "[DollPartRepair] 태엽은 이미 설치되었습니다."
            );

            return;
        }


        playerGrab.UseKey();


        if (fixedSpringObject != null)
        {
            fixedSpringObject.SetActive(true);
        }
        else
        {
            Debug.LogWarning(
                "[DollPartRepair] Fixed Spring Object가 없습니다.",
                this
            );
        }


        changeDoll.ReportSpringFixed();


        Debug.Log(
            "[DollPartRepair] ★ 태엽 장착 완료 ★"
        );


        CheckCompletion();
    }


    // =========================================================
    // 시작 시 인형 부품 표시 상태
    // =========================================================

    private void ApplyVisualState()
    {
        if (changeDoll == null)
        {
            return;
        }


        if (fixedLegObject != null)
        {
            fixedLegObject.SetActive(
                changeDoll.IsBrokenLegFixed
            );
        }


        if (fixedArmObject != null)
        {
            fixedArmObject.SetActive(
                changeDoll.IsBrokenArmFixed
            );
        }


        if (fixedSpringObject != null)
        {
            fixedSpringObject.SetActive(
                changeDoll.IsSpringFixed
            );
        }
    }


    // =========================================================
    // 전체 수리 체크
    // =========================================================

    private void CheckCompletion()
    {
        if (completionManager == null)
        {
            return;
        }


        completionManager.CheckRepairCompleted();
    }
}