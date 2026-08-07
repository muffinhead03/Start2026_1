using System.Collections.Generic;
using UnityEngine;

public class HintResult
{
    public int hintLevel;        // 1~4
    public string playerStatus;  // 상태명
    public PuzzleStep nextStep;  // 다음 안내할 단계
    public string hintType;      // direct / indirect
    public string puzzleId;      // 현재 씬 퍼즐 ID — PromptBuilder 씬 컨텍스트 조회용
}

public static class HintEngine
{
    public static HintResult Calculate(PlayerState state, PuzzleConfig config)
    {
        float score = 0f;
        score += CalcEmotionScore(state)      * 0.35f;
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

    static float CalcEmotionScore(PlayerState s)
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
        // 0~8점을 2점 간격으로 4등분 (5단계 → 4단계, 0719 기획 반영)
        if (score < 2f) return 1;
        if (score < 4f) return 2;
        if (score < 6f) return 3;
        return 4;
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