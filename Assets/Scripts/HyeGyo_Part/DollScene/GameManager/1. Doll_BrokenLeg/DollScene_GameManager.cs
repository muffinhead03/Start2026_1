using UnityEngine;


public class DollScene_GameManager : MonoBehaviour
{
    // =========================================================
    // 인형 진행 상태
    // =========================================================

    [Header("스테이지 진행 상태")]

    [SerializeField]
    private bool isBrokenLegChanged = false;

    [SerializeField]
    private bool isBrokenArmChanged = false;

    [SerializeField]
    private bool isSpringFound = false;

    [SerializeField]
    private bool isDollRepaired = false;


    // =========================================================
    // Exit Key
    // =========================================================

    [Header("Exit Key")]

    [SerializeField]
    private bool isExitKeyDropped = false;


    // =========================================================
    // 곰 인형 / 동전
    // =========================================================

    [Header("곰 인형 / 동전")]

    [SerializeField]
    private bool isCoinFound = false;

    [SerializeField]
    private bool isCoinOwned = false;


    // =========================================================
    // 게임 클리어
    // =========================================================

    [Header("게임 클리어")]

    [SerializeField]
    private bool isStageCleared = false;


    // =========================================================
    // 상태 확인 Property
    // =========================================================

    public bool IsBrokenLegChanged =>
        isBrokenLegChanged;

    public bool IsBrokenArmChanged =>
        isBrokenArmChanged;

    public bool IsSpringFound =>
        isSpringFound;

    public bool IsDollRepaired =>
        isDollRepaired;

    public bool IsExitKeyDropped =>
        isExitKeyDropped;

    public bool IsStageCleared =>
        isStageCleared;

    public bool IsCoinFound =>
        isCoinFound;

    public bool IsCoinOwned =>
        isCoinOwned;


    // =========================================================
    // 부러진 다리
    // =========================================================

    public void CompleteBrokenLeg()
    {
        if (isBrokenLegChanged)
        {
            return;
        }


        isBrokenLegChanged =
            true;


        Debug.Log(
            "[DollScene] 부러진 다리 교체 완료"
        );


        CheckDollRepairCondition();
    }


    // =========================================================
    // 부러진 팔
    // =========================================================

    public void CompleteBrokenArm()
    {
        if (isBrokenArmChanged)
        {
            return;
        }


        isBrokenArmChanged =
            true;


        Debug.Log(
            "[DollScene] 부러진 팔 교체 완료"
        );


        CheckDollRepairCondition();
    }


    // =========================================================
    // 태엽
    //
    // SpringProgressManager에서
    // 최종 완료 후 호출
    // =========================================================

    public void CompleteFindSpring()
    {
        if (isSpringFound)
        {
            return;
        }


        isSpringFound =
            true;


        Debug.Log(
            "[DollScene] 태엽 획득 완료"
        );


        CheckDollRepairCondition();
    }


    // =========================================================
    // 인형 수리 가능 여부
    // =========================================================

    public bool CanRepairDoll()
    {
        return
            isBrokenLegChanged &&
            isBrokenArmChanged &&
            isSpringFound;
    }


    // =========================================================
    // 수리 조건 확인
    // =========================================================

    private void CheckDollRepairCondition()
    {
        if (!CanRepairDoll())
        {
            return;
        }


        Debug.Log(
            "[DollScene] 모든 부품 완료 → 인형 수리 가능"
        );
    }


    // =========================================================
    // 인형 수리 완료
    // =========================================================

    public void CompleteDollRepair()
    {
        if (isDollRepaired)
        {
            return;
        }


        if (!CanRepairDoll())
        {
            Debug.LogWarning(
                "[DollScene] 아직 모든 부품이 준비되지 않았습니다."
            );

            return;
        }


        isDollRepaired =
            true;


        Debug.Log(
            "[DollScene] 인형 수리 완료"
        );
    }


    // =========================================================
    // Exit Key 낙하 완료
    // =========================================================

    public void CompleteExitKeyDrop()
    {
        if (isExitKeyDropped)
        {
            return;
        }


        if (!isDollRepaired)
        {
            Debug.LogWarning(
                "[DollScene] 인형 수리 전에는 Exit Key Drop을 완료할 수 없습니다."
            );

            return;
        }


        isExitKeyDropped =
            true;


        Debug.Log(
            "[DollScene] Exit Key 낙하 완료"
        );
    }


    // =========================================================
    // 곰 인형에서 동전 획득
    // =========================================================

    public void CompleteFindCoin()
    {
        if (isCoinFound)
        {
            return;
        }


        isCoinFound =
            true;

        isCoinOwned =
            true;


        Debug.Log(
            "[DollScene] 동전 획득 완료"
        );
    }


    // =========================================================
    // 동전 사용
    // =========================================================

    public bool UseCoin()
    {
        if (!isCoinOwned)
        {
            Debug.LogWarning(
                "[DollScene] 사용할 동전이 없습니다."
            );


            return false;
        }


        isCoinOwned =
            false;


        Debug.Log(
            "[DollScene] 동전 사용"
        );


        return true;
    }


    // =========================================================
    // 동전 반환
    // =========================================================

    public void ReturnCoin()
    {
        if (!isCoinFound)
        {
            return;
        }


        isCoinOwned =
            true;


        Debug.Log(
            "[DollScene] 동전 반환"
        );
    }


    // =========================================================
    // 최종 클리어
    // =========================================================

    public void ClearStage()
    {
        if (isStageCleared)
        {
            return;
        }


        isStageCleared =
            true;


        Debug.Log(
            "[DollScene] Stage Clear"
        );
    }
}