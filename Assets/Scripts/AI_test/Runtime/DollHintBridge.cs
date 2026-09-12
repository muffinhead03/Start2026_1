using UnityEngine;

/// <summary>
/// 인형씬 게임 상태(DollScene_GameManager 등)를 읽기 전용으로 폴링해서
/// 힌트 시스템(HintManager)에 completedSteps / foundClues를 기록하는 브릿지.
/// 혜교님 파트 스크립트는 수정하지 않고 public 프로퍼티만 참조함.
/// </summary>
public class DollHintBridge : MonoBehaviour
{
    [Header("힌트 시스템")]
    [SerializeField] private HintManager hintManager;

    [Header("인형씬 상태 참조 (읽기 전용)")]
    [SerializeField] private DollScene_GameManager gameManager;
    [SerializeField] private DollScene_ChangeBrokenLeg brokenLeg;
    [SerializeField] private RetroGameManager retroGameManager;

    bool step1Reported, step2Reported, step3Reported, step4Reported,
         step5Reported, step6Reported, step8Reported;
    // step7Reported는 조건 확정되면 추가

    void Start()
    {
        if (hintManager == null)
            hintManager = FindFirstObjectByType<HintManager>();

        if (gameManager == null)
            gameManager = FindFirstObjectByType<DollScene_GameManager>();

        if (brokenLeg == null)
            brokenLeg = FindFirstObjectByType<DollScene_ChangeBrokenLeg>();

        if (retroGameManager == null)
            retroGameManager = FindFirstObjectByType<RetroGameManager>();

        if (hintManager == null)
            Debug.LogError("[DollHintBridge] HintManager를 찾을 수 없습니다.");
    }

    void Update()
    {
        if (hintManager == null) return;

        if (!step1Reported && brokenLeg != null && brokenLeg.IsNumberLockSolved)
        {
            step1Reported = true;
            ReportStep(1, "clue_leg_lock_code");
        }

        if (!step2Reported && brokenLeg != null && brokenLeg.IsRepairedLegFound)
        {
            step2Reported = true;
            ReportStep(2, "clue_doll_leg");
        }

        if (!step3Reported && gameManager != null && gameManager.IsCoinFound)
        {
            step3Reported = true;
            ReportStep(3, "clue_coin");
        }

        if (!step4Reported && retroGameManager != null && retroGameManager.IsArmRewardUnlocked)
        {
            step4Reported = true;
            ReportStep(4, "clue_doll_arm");
        }

        if (!step5Reported && gameManager != null && gameManager.IsBrokenArmChanged)
        {
            step5Reported = true;
            ReportStep(5, null);
        }

        if (!step6Reported && gameManager != null && gameManager.IsSpringFound)
        {
            step6Reported = true;
            ReportStep(6, "clue_spring");
        }

        // 7. 태엽 장착 — 혜교님 확인 후 조건 추가 예정
        // if (!step7Reported && /* 조건 */) { step7Reported = true; ReportStep(7, null); }

        if (!step8Reported && gameManager != null && gameManager.IsExitKeyDropped)
        {
            step8Reported = true;
            ReportStep(8, null);
        }
    }

    void ReportStep(int stepId, string clueId)
    {
        if (clueId != null)
            hintManager.currentPlayerState.foundClues.Add(clueId);

        if (!hintManager.currentPlayerState.completedSteps.Contains(stepId))
            hintManager.currentPlayerState.completedSteps.Add(stepId);

        Debug.Log($"[DollHintBridge] step {stepId} 완료 기록");
    }
}