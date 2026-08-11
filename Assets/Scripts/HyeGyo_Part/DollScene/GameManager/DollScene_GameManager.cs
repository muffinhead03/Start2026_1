using UnityEngine;

public class DollScene_GameManager : MonoBehaviour
{
    [Header("스테이지 진행 상태")]
    [SerializeField] private bool isBrokenLegChanged = false;
    [SerializeField] private bool isBrokenArmChanged = false;
    [SerializeField] private bool isSpringFound = false;
    [SerializeField] private bool isDollRepaired = false;

    [Header("게임 클리어")]
    [SerializeField] private bool isStageCleared = false;


    // ================================
    // 상태 확인용 Property
    // ================================

    public bool IsBrokenLegChanged => isBrokenLegChanged;
    public bool IsBrokenArmChanged => isBrokenArmChanged;
    public bool IsSpringFound => isSpringFound;
    public bool IsDollRepaired => isDollRepaired;
    public bool IsStageCleared => isStageCleared;


    // ================================
    // 부러진 다리
    // ================================

    public void CompleteBrokenLeg()
    {
        if (isBrokenLegChanged)
            return;

        isBrokenLegChanged = true;

        Debug.Log("[DollScene] 부러진 다리 교체 완료");

        CheckDollRepairCondition();
    }


    // ================================
    // 부러진 팔
    // ================================

    public void CompleteBrokenArm()
    {
        if (isBrokenArmChanged)
            return;

        isBrokenArmChanged = true;

        Debug.Log("[DollScene] 부러진 팔 교체 완료");

        CheckDollRepairCondition();
    }


    // ================================
    // 태엽
    // ================================

    public void CompleteFindSpring()
    {
        if (isSpringFound)
            return;

        isSpringFound = true;

        Debug.Log("[DollScene] 태엽 획득 완료");

        CheckDollRepairCondition();
    }


    // ================================
    // 인형 수리 가능 여부
    // ================================

    public bool CanRepairDoll()
    {
        return isBrokenLegChanged
            && isBrokenArmChanged
            && isSpringFound;
    }


    private void CheckDollRepairCondition()
    {
        if (CanRepairDoll())
        {
            Debug.Log("[DollScene] 모든 부품 획득 완료 - 인형 수리 가능");

            // TODO:
            // ChangeDoll 오브젝트에게
            // "이제 인형을 수리할 수 있다"는 상태를 전달하거나
            // 상호작용을 활성화할 예정
        }
    }


    // ================================
    // 인형 수리 완료
    // ================================

    public void CompleteDollRepair()
    {
        if (isDollRepaired)
            return;

        if (!CanRepairDoll())
        {
            Debug.LogWarning(
                "[DollScene] 아직 모든 부품이 준비되지 않았습니다."
            );

            return;
        }

        isDollRepaired = true;

        Debug.Log("[DollScene] 인형 수리 완료");

        // TODO:
        // 인형 태엽 상호작용 활성화
        // 최종 열쇠 획득 이벤트 활성화
    }


    // ================================
    // 최종 클리어
    // ================================

    public void ClearStage()
    {
        if (isStageCleared)
            return;

        isStageCleared = true;

        Debug.Log("[DollScene] Stage Clear");

        // TODO:
        // 플레이어 입력 제한
        // 클리어 연출
        // 클리어 UI
        // 다음 Scene 이동
    }
}