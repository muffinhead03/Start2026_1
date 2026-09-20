using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 힌트 요청 시점의 플레이어 상태를 5+1가지로 분류한 값.
/// 문자열 대신 enum으로 둬서 HintEngine↔PromptBuilder 사이 매직 스트링 오타를 컴파일 타임에 잡는다.
/// </summary>
public enum HintStatus
{
    ClueNotFound,           // 단서 미발견
    ClueMisunderstood,      // 단서 미이해
    ClueConnectFailed,      // 단서 연결 실패
    RepeatedFailure,        // 반복 실패
    AboutToGiveUp,          // 포기 직전
    PossessedItemInProgress // 보유 물품 진행중 — 손/인벤토리 보유 물품 기준으로 순서를 건너뛰고 override된 경우
}

/// <summary>
/// HintEngine.Calculate()의 결과물. 어떤 레벨의 힌트를, 어떤 스텝에 대해,
/// 어떤 톤(direct/indirect)으로 보여줄지에 대한 판단 결과를 담는다.
/// LLM은 이 결과를 자연어로 옮기기만 할 뿐, 이 판단 자체에는 관여하지 않는다.
/// </summary>
public class HintResult
{
    public int hintLevel;        // 1~5
    public HintStatus playerStatus; // 상태
    public PuzzleStep nextStep;  // 다음 안내할 단계
    public string hintType;      // direct / indirect
    public string puzzleId;      // 현재 씬 퍼즐 ID — PromptBuilder 씬 컨텍스트 조회용
    public string debugReason;   // nextStep이 어떤 경로(손/인벤토리/체크리스트)로 선택됐는지 — 콘솔 로그용, 프롬프트엔 안 넣음
    public bool isOverride;      // true면 손/인벤토리 보유 물품 기준으로 순서를 건너뛰고 선택된 것 — PromptBuilder가 "이미 지나쳤다" 문구를 뺄지 판단하는 데 씀
}

/// <summary>
/// 힌트 레벨/상태/다음 스텝을 코드로 결정하는 판단 엔진.
/// "판단은 로직, 표현은 AI" 원칙의 로직 쪽 절반을 담당한다 — LLM이나 Unity 특정 기능에
/// 의존하지 않는 순수 계산 로직이라, PlayerState/PuzzleConfig만 맞춰주면 어느 프로젝트에도 그대로 재사용 가능하다.
/// </summary>
public static class HintEngine
{
    // 점수 계산 임계값 모음. 기본값 = 기존 하드코딩 값과 동일.
    // HintManager 등 외부에서 Inspector로 받은 값을 여기에 주입해서 코드 수정 없이 튜닝 가능.
    public static HintTuningConfig Tuning { get; set; } = new HintTuningConfig();

    // 콘솔 로그 등에서 사람이 읽기 좋은 한국어 라벨이 필요할 때 쓰는 매핑.
    // 판단 로직 자체와는 무관 — 순수 표시용이라 Unity 의존성 없음.
    static readonly Dictionary<HintStatus, string> KoreanLabel = new Dictionary<HintStatus, string>
    {
        { HintStatus.ClueNotFound,             "단서 미발견" },
        { HintStatus.ClueMisunderstood,        "단서 미이해" },
        { HintStatus.ClueConnectFailed,        "단서 연결 실패" },
        { HintStatus.RepeatedFailure,          "반복 실패" },
        { HintStatus.AboutToGiveUp,            "포기 직전" },
        { HintStatus.PossessedItemInProgress,  "보유 물품 진행중" },
    };

    public static string ToKoreanLabel(this HintStatus status)
        => KoreanLabel.TryGetValue(status, out var label) ? label : status.ToString();

    /// <summary>
    /// 플레이어 상태와 퍼즐 설정을 기반으로 힌트 레벨(1~5), 플레이어 상태, 다음 안내 스텝을 계산한다.
    /// 5개 가중치 요소(체류·힌트 요청 이력 35%, 요청강도 25%, 정체시간 20%, 퍼즐진행 15%, 반복조사 5%)를 합산해
    /// 점수를 내고, 그 점수를 5단계로 환산한다.
    /// handObjectName/inventoryObjectNames가 주어지면, 체크리스트 순서보다 "지금 실제로 보유 중인 것"을 우선해서
    /// nextStep을 override한다 (물리적 배치상 스텝 순서 보장이 안 되는 씬 대응용 — 예: 좁은 방에 여러 상호작용 오브젝트가 동시에 노출되는 경우).
    /// </summary>
    public static HintResult Calculate(PlayerState state, PuzzleConfig config,
        string handObjectName = null, List<string> inventoryObjectNames = null)
    {
        float score = 0f;
        score += CalcHintHistoryScore(state)  * 0.35f;
        score += CalcRequestScore(state)      * 0.25f;
        score += CalcStagnationScore(state)   * 0.20f;
        score += CalcProgressScore(state, config) * 0.15f;
        score += CalcMisunderstandScore(state) * 0.05f;

        var nextStep = GetNextStep(state, config, handObjectName, inventoryObjectNames, out string reason, out bool isOverride);

        return new HintResult
        {
            hintLevel    = ScoreToLevel(score),
            playerStatus = DetermineStatus(state, config, handObjectName, inventoryObjectNames),
            nextStep     = nextStep,
            hintType     = state.hintType,
            puzzleId     = config.puzzleId,
            debugReason  = reason,
            isOverride   = isOverride
        };
    }

    static float CalcHintHistoryScore(PlayerState s)
    {
        var t = Tuning;
        float score = 0f;

        if      (s.staySeconds > t.staySecondsHighThreshold) score += t.staySecondsHighBonus;
        else if (s.staySeconds > t.staySecondsMidThreshold)  score += t.staySecondsMidBonus;

        if      (s.hintCount >= 3)    score += 3f;
        else if (s.hintCount >= 2)    score += 1.5f;
        if      (s.failCount >= 5)    score += 2f;
        else if (s.failCount >= 3)    score += 1f;

        return Mathf.Min(score, 8f);
    }

    static float CalcRequestScore(PlayerState s)
        => s.hintType == "direct" ? 8f : 3f;

    static float CalcStagnationScore(PlayerState s)
        => Mathf.Min(s.failCount * 0.8f + (s.staySeconds / Tuning.stagnationStaySecondsDivisor), 8f);

    static float CalcProgressScore(PlayerState s, PuzzleConfig c)
    {
        float ratio = (float)s.completedSteps.Count / Mathf.Max(c.totalSteps, 1);
        return (1f - ratio) * 8f;
    }

    static float CalcMisunderstandScore(PlayerState s)
    {
        int count = 0;
        foreach (var obj in s.repeatedInspections)
            if (obj.count >= 3) count++;
        return Mathf.Min(count * 2.5f, 8f);
    }

    static int ScoreToLevel(float score)
    {
        var t = Tuning;
        if (score < t.level2Cutline) return 1;
        if (score < t.level3Cutline) return 2;
        if (score < t.level4Cutline) return 3;
        if (score < t.level5Cutline) return 4;
        return 5;
    }

    /// <summary>
    /// 힌트 톤/방향을 정하는 상태값을 판정한다.
    /// override로 선택된 스텝이면(previousStepDone 체크가 의미 없어지므로) PossessedItemInProgress로 별도 분기한다.
    /// </summary>
    static HintStatus DetermineStatus(PlayerState s, PuzzleConfig c, string handObjectName, List<string> inventoryObjectNames)
    {
        if (s.failCount >= 5) return HintStatus.RepeatedFailure;
        if (s.hintCount >= 3) return HintStatus.AboutToGiveUp;

        var nextStep = GetNextStep(s, c, handObjectName, inventoryObjectNames, out _, out bool isOverride);
        if (nextStep == null) return HintStatus.ClueConnectFailed; // 모든 스텝 완료된 경우 (힌트 요청 자체가 막히긴 함)

        if (isOverride) return HintStatus.PossessedItemInProgress; // 순서 뒤섞인 채 손/인벤토리 기준으로 뽑힌 경우 — "이전 스텝 완료 여부" 체크가 무의미해서 별도 처리

        bool previousStepDone = nextStep.id == 1 || s.completedSteps.Contains(nextStep.id - 1);
        if (!previousStepDone) return HintStatus.ClueNotFound;

        if (s.completedSteps.Count == 0) return HintStatus.ClueMisunderstood;
        return HintStatus.ClueConnectFailed;
    }

    /// <summary>
    /// 다음 안내할 스텝을 3단계 우선순위로 결정한다:
    /// 1순위 - 지금 손에 쥔 오브젝트와 연결된 미완료 스텝 (가장 확실한 "지금 이거 하는 중" 신호)
    /// 2순위 - 인벤토리에 보관 중인 오브젝트와 연결된 미완료 스텝
    /// 3순위 - 기존 체크리스트 방식(가장 낮은 id의 미완료 스텝)
    /// reason은 콘솔 로그용, isOverride는 PromptBuilder/DetermineStatus가 순서 전제를 깔지 말지 판단하는 데 씀.
    /// </summary>
    static PuzzleStep GetNextStep(PlayerState s, PuzzleConfig c,
        string handObjectName, List<string> inventoryObjectNames,
        out string reason, out bool isOverride)
    {
        // 1순위: 손
        if (!string.IsNullOrEmpty(handObjectName))
        {
            var held = c.steps.Find(st => st.relatedObjectName == handObjectName && !s.completedSteps.Contains(st.id));
            if (held != null)
            {
                reason = $"hand-override: step {held.id} ({handObjectName})";
                isOverride = true;
                return held;
            }
        }

        // 2순위: 인벤토리
        if (inventoryObjectNames != null)
        {
            foreach (var step in c.steps)
                if (!s.completedSteps.Contains(step.id) &&
                    !string.IsNullOrEmpty(step.relatedObjectName) &&
                    inventoryObjectNames.Contains(step.relatedObjectName))
                {
                    reason = $"inventory-override: step {step.id} ({step.relatedObjectName})";
                    isOverride = true;
                    return step;
                }
        }

        // 3순위: 기존 체크리스트 (가장 낮은 id의 미완료 스텝)
        foreach (var step in c.steps)
            if (!s.completedSteps.Contains(step.id))
            {
                reason = $"checklist-fallback: step {step.id}";
                isOverride = false;
                return step;
            }

        reason = "all-completed";
        isOverride = false;
        return null;
    }
}