using UnityEngine;

/// <summary>
/// OrganSceneManager.GetCurrentStep()을 폴링해서 HintManager에 completedSteps를 기록하는 브릿지.
///
/// 주의: OrganSceneManager.CompleteStep(id)는 내부적으로 this.step = id + 1 로 저장한다.
/// 즉 GetCurrentStep()이 반환하는 값은 "완료된 마지막 스텝 id"가 아니라 그보다 1 큰 값
/// ("다음에 진행할 스텝")이다. 그래서 실제로 완료된 마지막 스텝 id는 GetCurrentStep() - 1.
///
/// 1~7번(파이프)은 기존 Scene2_AI 스크립트가 이미 HintManager에 직접 보고하고 있고,
/// 씬 확인 결과 2~6번도 OrganSceneManager.CompleteStep으로 중복 연결돼있음 — 중복이어도
/// HintManager 쪽 Contains() 체크로 안전하니 그대로 둬도 됨. 이 브릿지는 1~17번 전체를
/// OrganSceneManager 기준으로 커버한다.
/// </summary>
public class OrganHintBridge : MonoBehaviour
{
    [Header("힌트 시스템")]
    [SerializeField] private HintManager hintManager;

    [Header("오르간룸 상태 참조 (읽기 전용)")]
    [SerializeField] private OrganSceneManager organSceneManager;

    static readonly (int stepId, string clueId)[] ClueMap =
    {
        (9,  "clue_torn_sheet_pieces"),
        (15, "clue_completed_sheet"),
    };

    int lastReportedStep = 0; // 전체 1~17번 커버

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

        int highestCompleted = organSceneManager.GetCurrentStep() - 1;

        while (lastReportedStep < highestCompleted && lastReportedStep < 17)
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