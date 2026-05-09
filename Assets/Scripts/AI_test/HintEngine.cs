using System.Collections.Generic;
using UnityEngine;

public class HintResult
{
    public int hintLevel;        // 1~5
    public string playerStatus;  // 상태명
    public PuzzleStep nextStep;  // 다음 안내할 단계
    public string hintType;      // direct / indirect
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
            hintType     = state.hintType
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
        if (score < 2f) return 1;
        if (score < 3f) return 2;
        if (score < 4f) return 3;
        if (score < 5f) return 4;
        return 5;
    }

    static string DetermineStatus(PlayerState s, PuzzleConfig c)
    {
        foreach (var clue in c.requiredClues)
            if (!s.foundClues.Contains(clue)) return "단서 미발견";
        if (s.failCount >= 5)        return "반복 실패";
        if (s.hintCount >= 3)        return "포기 직전";
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
