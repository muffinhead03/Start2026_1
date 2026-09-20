using UnityEngine;

/// <summary>
/// OrganSceneManager.GetCurrentStep()을 폴링해서 HintManager에 completedSteps를 기록하는 브릿지.
/// 8~17번(Phase 2~3) 전용 — 1~7번(파이프)은 기존 Scene2_AI 스크립트가 직접 HintManager를 호출하는
/// 방식을 그대로 유지하므로 여기서는 다루지 않음.
///
/// 전제: 민주님 파트에서 8~17번 완료 지점마다 organSceneManager.CompleteNextStep()
/// (또는 CompleteStep(7~16))을 순서대로 호출 → GetCurrentStep()이 8→9→...→17로 올라감.
/// </summary>
public class OrganHintBridge : MonoBehaviour
{
    [Header("힌트 시스템")]
    [SerializeField] private HintManager hintManager;

    [Header("오르간룸 상태 참조 (읽기 전용)")]
    [SerializeField] private OrganSceneManager organSceneManager;

    // clue가 필요한 스텝만 매핑 (foundClues에 추가할 것 없으면 안 넣으면 됨)
    static readonly (int stepId, string clueId)[] ClueMap =
    {
        (9,  "clue_torn_sheet_pieces"),
        (15, "clue_completed_sheet"),
    };

    int lastReportedStep = 7; // 1~7번은 기존 방식이 이미 처리하므로 8번부터 시작

    void Start()
    {
        if (hintManager == null)
            hintManager = FindFirstObjectByType<HintManager>();

        if (organSceneManager == null)
            organSceneManager = FindFirstObjectByType<OrganSceneManager>();

        if (hintManager == null)
            Debug.LogError("[OrganHintBridge] HintManager를 찾을 수 없습니다.");

        if (organSceneManager == null)
            Debug.LogError("[OrganHintBridge] OrganSceneManager를 찾을 수 없습니다.");
    }

    void Update()
    {
        if (hintManager == null || organSceneManager == null) return;

        int current = organSceneManager.GetCurrentStep();

        // 한 번에 여러 칸 올라가는 경우(스텝 스킵 등)까지 대비해 사이값도 다 보고
        while (lastReportedStep < current && lastReportedStep < 17)
        {
            lastReportedStep++;
            ReportStep(lastReportedStep);
        }
    }

    void ReportStep(int stepId)
    {
        foreach (var (id, clueId) in ClueMap)
        {
            if (id == stepId)
            {
                hintManager.currentPlayerState.foundClues.Add(clueId);
                break;
            }
        }

        if (!hintManager.currentPlayerState.completedSteps.Contains(stepId))
            hintManager.currentPlayerState.completedSteps.Add(stepId);

        Debug.Log($"[OrganHintBridge] step {stepId} 완료 기록");
    }
}
