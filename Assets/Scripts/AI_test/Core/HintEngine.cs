using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// HintEngine.Calculate()의 결과물. 어떤 레벨의 힌트를, 어떤 스텝에 대해,
/// 어떤 톤(direct/indirect)으로 보여줄지에 대한 판단 결과를 담는다.
/// LLM은 이 결과를 자연어로 옮기기만 할 뿐, 이 판단 자체에는 관여하지 않는다.
/// </summary>
public class HintResult
{
    public int hintLevel;        // 1~5
    public string playerStatus;  // 상태명
    public PuzzleStep nextStep;  // 다음 안내할 단계
    public string hintType;      // direct / indirect
    public string puzzleId;      // 현재 씬 퍼즐 ID — PromptBuilder 씬 컨텍스트 조회용
}

/// <summary>
/// 힌트 레벨/상태/다음 스텝을 코드로 결정하는 판단 엔진.
/// "판단은 로직, 표현은 AI" 원칙의 로직 쪽 절반을 담당한다 — LLM이나 Unity 특정 기능에
/// 의존하지 않는 순수 계산 로직이라, PlayerState/PuzzleConfig만 맞춰주면 어느 프로젝트에도 그대로 재사용 가능하다.
/// </summary>
public static class HintEngine
{
    /// <summary>
    /// 플레이어 상태와 퍼즐 설정을 기반으로 힌트 레벨(1~5), 플레이어 상태, 다음 안내 스텝을 계산한다.
    /// 5개 가중치 요소(체류·힌트 요청 이력 35%, 요청강도 25%, 정체시간 20%, 퍼즐진행 15%, 반복조사 5%)를 합산해
    /// 점수를 내고, 그 점수를 5단계로 환산한다.
    /// </summary>
    public static HintResult Calculate(PlayerState state, PuzzleConfig config)
    {
        float score = 0f;
        score += CalcHintHistoryScore(state)  * 0.35f;
        score += CalcRequestScore(state)      * 0.25f;
        score += CalcStagnationScore(state)   * 0.20f;
        score += CalcProgressScore(state, config) * 0.15f;
        score += CalcMisunderstandScore(state) * 0.05f;

        return new HintResult
        {
            hintLevel    = ScoreToLevel(score),
            playerStatus = DetermineStatus(state, config),
            nextStep     = GetNextStep(state, config),
            hintType     = state.hintType,
            puzzleId     = config.puzzleId
        };
    }

    static float CalcHintHistoryScore(PlayerState s)
    {
        float score = 0f;

        if      (s.staySeconds > 300) score += 3f;
        else if (s.staySeconds > 120) score += 1.5f;

        if      (s.hintCount >= 3)    score += 3f;
        else if (s.hintCount >= 2)    score += 1.5f;
        if      (s.failCount >= 5)    score += 2f;
        else if (s.failCount >= 3)    score += 1f;

        return Mathf.Min(score, 8f);
    }

    static float CalcRequestScore(PlayerState s)
        => s.hintType == "direct" ? 8f : 3f;

    static float CalcStagnationScore(PlayerState s)
        => Mathf.Min(s.failCount * 0.8f + (s.staySeconds / 60f) * 0.5f, 8f);

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
        // 힌트 요청 이력 기반 점수를 1점 내외 간격으로 5등분 (제작설계서 기준 재정렬)
        if (score < 2f) return 1;
        if (score < 3f) return 2;
        if (score < 4f) return 3;
        if (score < 5f) return 4;
        return 5;
    }

    static string DetermineStatus(PlayerState s, PuzzleConfig c)
    {
        if (s.failCount >= 5) return "반복 실패";
        if (s.hintCount >= 3) return "포기 직전";

        var nextStep = GetNextStep(s, c);
        if (nextStep == null) return "단서 연결 실패"; // 모든 스텝 완료된 경우 (힌트 요청 자체가 막히긴 함)

        // 지금 안내해야 할 스텝의 "바로 전 단계"가 끝났는지로 판단
        // 전 단계가 안 끝났으면 아직 탐색도 못 한 상태 → 단서 미발견
        bool previousStepDone = nextStep.id == 1 || s.completedSteps.Contains(nextStep.id - 1);
        if (!previousStepDone) return "단서 미발견";

        if (s.completedSteps.Count == 0) return "단서 미이해";
        return "단서 연결 실패";
    }

    static PuzzleStep GetNextStep(PlayerState s, PuzzleConfig c)
    {
        foreach (var step in c.steps)
            if (!s.completedSteps.Contains(step.id)) return step;
        return null;
    }
}