using UnityEngine;

public class DollMoveManager : MonoBehaviour
{
    [Header("Player")]
    [SerializeField]
    private Player_Grab playerGrab;


    [Header("Change Doll")]
    [SerializeField]
    private DollScene_ChangeDoll changeDoll;


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
    }


    // =========================================================
    // 다리
    // =========================================================

    public void FixBrokenLeg()
    {
        if (playerGrab == null ||
            changeDoll == null)
        {
            return;
        }


        if (changeDoll.IsBrokenLegFixed)
        {
            Debug.Log(
                "[DollMove] 다리는 이미 수리되었습니다."
            );

            return;
        }


        if (!playerGrab.hasKey(legItemName))
        {
            Debug.Log(
                $"[DollMove] 필요한 다리가 없습니다 : {legItemName}"
            );

            return;
        }


        // 가져온 실제 아이템 소비
        playerGrab.UseKey();


        // 인형 내부 정상 다리 활성화
        if (fixedLegObject != null)
        {
            fixedLegObject.SetActive(true);
        }


        changeDoll.ReportBrokenLegFixed();


        Debug.Log(
            "[DollMove] 다리 교체 완료"
        );
    }


    // =========================================================
    // 팔
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
    }
}