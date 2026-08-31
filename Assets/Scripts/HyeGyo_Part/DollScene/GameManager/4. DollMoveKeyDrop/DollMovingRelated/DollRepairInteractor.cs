using UnityEngine;

public class DollRepairInteractor : MonoBehaviour
{
    // =========================================================
    // Player
    // =========================================================

    [Header("Player")]
    [SerializeField]
    private Player_Grab playerGrab;


    // =========================================================
    // Repair Manager
    // =========================================================

    [Header("Repair Manager")]
    [SerializeField]
    private DollPartRepairManager repairManager;


    // =========================================================
    // Doll State
    // =========================================================

    [Header("Doll State")]
    [SerializeField]
    private DollScene_ChangeDoll changeDoll;


    // =========================================================
    // Item Names
    //
    // Player_Grab.hasKey()가 Regex.IsMatch()를 사용하므로
    // ^ $를 사용해서 정확히 같은 이름만 인식
    // =========================================================

    [Header("Item Names")]

    [SerializeField]
    private string legItemName = "^DollLeg$";

    [SerializeField]
    private string armItemName = "^DollArm$";

    [SerializeField]
    private string springItemName = "^Spring$";


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


        if (repairManager == null)
        {
            repairManager =
                FindFirstObjectByType<DollPartRepairManager>();
        }


        if (changeDoll == null)
        {
            changeDoll =
                FindFirstObjectByType<DollScene_ChangeDoll>();
        }


        if (playerGrab == null)
        {
            Debug.LogError(
                "[DollRepairInteractor] Player_Grab을 찾을 수 없습니다.",
                this
            );
        }


        if (repairManager == null)
        {
            Debug.LogError(
                "[DollRepairInteractor] DollPartRepairManager를 찾을 수 없습니다.",
                this
            );
        }


        if (changeDoll == null)
        {
            Debug.LogError(
                "[DollRepairInteractor] DollScene_ChangeDoll을 찾을 수 없습니다.",
                this
            );
        }
    }


    // =========================================================
    // 인형 상호작용
    //
    // 인형을 바라보고 E를 눌렀을 때
    // 이 함수 하나만 호출
    // =========================================================

    public void TryRepair()
    {
        if (playerGrab == null ||
            repairManager == null ||
            changeDoll == null)
        {
            Debug.LogWarning(
                "[DollRepairInteractor] 필요한 참조가 연결되지 않았습니다.",
                this
            );

            return;
        }


        // =====================================================
        // 다리
        // =====================================================

        if (!changeDoll.IsBrokenLegFixed &&
            playerGrab.hasKey(legItemName))
        {
            Debug.Log(
                "[DollRepairInteractor] DollLeg 확인 → 다리 장착"
            );


            repairManager.FixLeg();

            return;
        }


        // =====================================================
        // 팔
        // =====================================================

        if (!changeDoll.IsBrokenArmFixed &&
            playerGrab.hasKey(armItemName))
        {
            Debug.Log(
                "[DollRepairInteractor] DollArm 확인 → 팔 장착"
            );


            repairManager.FixArm();

            return;
        }


        // =====================================================
        // 태엽
        // =====================================================

        if (!changeDoll.IsSpringFixed &&
            playerGrab.hasKey(springItemName))
        {
            Debug.Log(
                "[DollRepairInteractor] Spring 확인 → 태엽 장착"
            );


            repairManager.FixSpring();

            return;
        }


        // =====================================================
        // 장착 가능한 아이템 없음
        // =====================================================

        Debug.Log(
            "[DollRepairInteractor] 현재 손에 장착 가능한 인형 부품이 없습니다."
        );
    }
    // =========================================================
// 현재 플레이어가 인형 수리용 부품을 들고 있는지
// =========================================================

public bool HasRepairPart()
{
    if (playerGrab == null)
    {
        return false;
    }


    if (playerGrab.hasKey(legItemName))
    {
        return true;
    }


    if (playerGrab.hasKey(armItemName))
    {
        return true;
    }


    if (playerGrab.hasKey(springItemName))
    {
        return true;
    }


    return false;
}
}